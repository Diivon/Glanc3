using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Glc
{
	public static partial class Glance
	{
		internal static partial class CodeGenerator
		{
			///<summary>FileStream for Write/WriteLn</summary>
			internal static FileStream FStream;
			///<summary>write data + '\n' -> fs considering TabCount</summary>
			internal static void WriteLnIn(FileStream fs, string data)
			{
				if (data == null)
					throw new ArgumentNullException();
				char[] charsToTrim = { '\t', ' ' };
				string[] strings = data.Split('\n');	//return lines
				int tabcount = 0;						//code block level(if they are inside '{', '}')
				foreach(var str in strings)				//for every line in data
				{
					if (String.IsNullOrWhiteSpace(str))	//if white space
						continue;
					if (str.Contains('}'))              //if block is ended P.S. it make bugs if '{' and '}' at the same line
						if(!str.Contains('{'))
							--tabcount;

					string finalstr = "";				//string, that will be writed in file
					for (byte i = 0; i < tabcount; ++i)	//add tabs 
						finalstr += '\t';               //for better reading
					finalstr += str.Trim();				//without trash
					byte[] finalBytes = Encoding.Unicode.GetBytes(finalstr + '\n');//line as bytes to write in file
					fs.Write(finalBytes, 0, finalBytes.Length);

					if (str.Contains('{'))              //if block is started P.S. it make bugs if '{' and '}' at the same line
						++tabcount;
				}
			}
			///<summary>write "StandartIncludes" from settings.gcs -> fs</summary>
			internal static void writeStdInc(FileStream fs)
			{
				WriteLnIn(fs, templates["Set:StandartIncludes:Def"]);
			}
			///<summary> </summary>
			internal static void writeMainCpp(FileStream fs)
			{
				string Sub_include = "#include \"main.h\"";
				WriteLnIn(fs,
							templates["File:Main:Def"].
								Replace("#Include#", Sub_include).
								Replace("#FirstSceneObjName#", scenes[0].ObjectName).
								Replace("#FirstSceneClassName#", scenes[0].ClassName)
						);
			}
			///<summary>include everything, what must know main.cpp</summary>
			internal static void writeMainH(FileStream fs)
			{
				writeStdInc(fs);
				foreach (var Scn in scenes)
					WriteLnIn(fs, "#include \"" + Scn.GetDeclarationFileName() + '"');
			}
			///<summary>write header, and implementation of this object</summary>
			internal static void writePhysicalObject(FileStream declaration, FileStream implementation, PhysicalObject PO)
			{

				writeStdInc(declaration);
				WriteLnIn(declaration, templates["Class:PhysicalObject:Declaration"]
					.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
					.Replace("#Pos#" , PO.Pos.GetCppCtor())
					.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
					.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
					.Replace("#ComponentsMethods#", PO.GetComponentsMethodsDeclaration())
					.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
					.Replace("#OnStart#", PO.GetComponentsOnStart())
					.Replace("#ClassName#", PO.ClassName)
					.Replace("#SceneName#", PO._scene.ClassName)
					.Replace("#LayerName#", PO._layer.ClassName)
							);
				WriteLnIn(implementation, templates["Class:PhysicalObject:Implementation"]
					.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
					.Replace("#Pos#", PO.Pos.GetCppCtor())
					.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
					.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
					.Replace("#ComponentsMethods#", PO.GetComponentsMethodsImplementation())
					.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
					.Replace("#OnStart#", PO.GetComponentsOnStart())
					.Replace("#ClassName#", PO.ClassName)
					.Replace("#SceneName#", PO._scene.ClassName)
					.Replace("#LayerName#", PO._layer.ClassName)
							);
			}
			internal static void writeRenderableObject(FileStream declaration, FileStream implementation, RenderableObject PO)
			{
				writeStdInc(declaration);

				WriteLnIn(declaration, templates["Class:RenderableObject:Declaration"]
					.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
					.Replace("#Pos#", PO.Pos.GetCppCtor())
					.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
					.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
					.Replace("#ComponentsMethods#", PO.GetComponentsMethodsDeclaration())
					.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
					.Replace("#OnStart#", PO.GetComponentsOnStart())
					.Replace("#ClassName#", PO.ClassName)
					.Replace("#SceneName#", PO._scene.ClassName)
					.Replace("#LayerName#", PO._layer.ClassName)
							);
				WriteLnIn(implementation, templates["Class:RenderableObject:Implementation"]
					.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
					.Replace("#Pos#", PO.Pos.x.ToString() + ", " + PO.Pos.y.ToString())
					.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
					.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
					.Replace("#ComponentsMethods#", PO.GetComponentsMethodsImplementation())
					.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
					.Replace("#OnStart#", PO.GetComponentsOnStart())
					.Replace("#ClassName#", PO.ClassName)
					.Replace("#SceneName#", PO._scene.ClassName)
					.Replace("#LayerName#", PO._layer.ClassName)
					.Replace("#GetCurrentSprite#", PO.GetCurrentSprite())
							);
			}
			internal static void writeLayer(FileStream declaration, FileStream implementation, Layer l)
			{
				writeStdInc(declaration);
				{
					string getObjects = "public:\n";
					foreach (var i in l._objects)
					{
						getObjects += "template<>\n" + i.ClassName + " & getObject(){\nreturn " + i.ObjectName + ";\n}\n";
						getObjects += "template<>\nconst " + i.ClassName + " & getObject() const{\nreturn " + i.ObjectName + ";\n}\n";
					}

					string objDeclInclude = "";
					foreach (var i in l._objects)
						objDeclInclude += "#include \"" + i.GetDeclarationFileName() + "\"\n";

					string render = "template<>\ninline void ::gc::Renderer::renderLayer(const " + l.ClassName + " & l){\n";
					foreach (var i in l._objects)
						if (i is RenderableObject)
							render += "this->render(l.getObject<" + i.ClassName + ">().getCurrentSprite(), l.getObject<" + i.ClassName + ">().pos);\n";
					render += '}';

					WriteLnIn(declaration, templates["Class:Layer:Declaration"]
										.Replace("#SceneName#", l._scene.ClassName)
										.Replace("#ClassName#", l.ClassName)
										.Replace("#ObjectVariables#", l.GetVariables())
										.Replace("#ComponentsVariables#", "")
										.Replace("#ComponentsMethodsDeclaration#", l.GetMethodsDeclaration())
										.Replace("#getObjects#", getObjects)
										.Replace("#ObjectsDeclarationInclude#", objDeclInclude)
										.Replace("#RenderLayer#", render)
						);
				}//declaration
				{
					string ctorList = "";
					foreach (var i in l._objects)
						ctorList += ", " + i.ObjectName + "(scene, *this)";

					WriteLnIn(implementation, templates["Class:Layer:Implementation"]
										.Replace("#SceneName#", l._scene.ClassName)
										.Replace("#ClassName#", l.ClassName)
										.Replace("#AdditionalConstructorList#", ctorList)
										.Replace("#ConstructorBody#", l.GetConstructorBody())
										.Replace("#OnStart#", l.GetOnStart())
										.Replace("#OnUpdate#", l.GetOnUpdate())
										.Replace("#ComponentsMethodsImplementation#", l.GetMethodsImplementation())
						);
				}//implementation
			}
			internal static void writeScene(FileStream fs, Scene s)
			{
				writeStdInc(fs);
				WriteLnIn(fs, templates["Сlass:Scene:FDef"]
									.Replace("#ClassName#", s.ClassName)
									.Replace("#Layers#", s.GetVariables())
									.Replace("#Ctors#", s.GetConstructors())
									.Replace("#start#", s.GetAllLayersOnStart())
									.Replace("#update#", s.GetAllLayersOnUpdate())
									.Replace("#getLayers#", s.GetGetLayers())
									.Replace("#LayersDeclarationInclude#", s.GetIncludes())
									.Replace("#RenderScene#", s.GetRender())
					);
			}
			///<summary>Code generate</summary>
			internal static void GenerateCode()
			{
				foreach (var scene in scenes)
				{
					foreach (var Lr in  scene.LayerList)//layers
					{
						foreach (var SO in Lr._objects)
						{
							if (SO.ImplementationFilePath == null || SO.ImplementationFilePath == "")
							{
								string implFileName = BuildSetting.sourceDir + SO.GetImplementationFileName();
								File.Create(implFileName).Close();
								SO.ImplementationFilePath = implFileName;
							}
							if (SO.DeclarationFilePath == null || SO.DeclarationFilePath == "")
							{
								string declFileName = BuildSetting.sourceDir + SO.GetDeclarationFileName();
								File.Create(declFileName).Close();
								SO.DeclarationFilePath = declFileName;
							}
							SO.GenerateCode();
						}
					}
				}
				{//layers	
					foreach (var i in scenes)
						foreach(var o in i.LayerList)
						{
							var Lrhfs = File.Create(BuildSetting.sourceDir + o.GetDeclarationFileName());
							var Lrcppfs = File.Create(BuildSetting.sourceDir + o.GetImplementationFileName());
							writeLayer(Lrhfs, Lrcppfs, o);
						}
				}
				{//scenes
					foreach (var i in scenes)
					{
						var Scfs = File.Create(BuildSetting.sourceDir + i.GetDeclarationFileName());
						writeScene(Scfs, i);
						Scfs.Close();
					}
				}
				{//mainh
					var mainHfs = File.Create(BuildSetting.sourceDir + "main.h");
					writeMainH(mainHfs);
					mainHfs.Close();
				}
				{//maincpp
					var MainCppfs = File.Create(BuildSetting.sourceDir + "main.cpp");
					writeMainCpp(MainCppfs);
					MainCppfs.Close();
				}

			}
			static CodeGenerator()
			{
				FStream = null;
			}
		}
	}
}