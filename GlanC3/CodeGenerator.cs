using System;
using System.Collections.Generic;
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
				WriteLnIn(fs, "#include \"GC/Camera.h\"");
				foreach (var Scn in scenes)
					WriteLnIn(fs, "#include \"" + Scn.ClassName + ".h\"");
			}
			///<summary>write header, and implementation of this object</summary>
			internal static void writePhysicalObject(FileStream declaration, FileStream implementation, PhysicalObject PO)
			{

				writeStdInc(declaration);
				WriteLnIn(declaration, templates["Class:PhysicalObject:Declaration"]
									.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
									.Replace("#Pos#" , PO.Pos.ToCppCtor())
									.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
									.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
									.Replace("#ComponentsMethods#", PO.GetComponentsMethodsDeclaration())
									.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
									.Replace("#OnStart#", PO.GetComponentsOnStart())
									.Replace("#ClassName#", PO.ClassName)
									.Replace("#SceneName#", PO._scene.ClassName)
							);
				WriteLnIn(implementation, templates["Class:PhysicalObject:Implementation"]
									.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
									.Replace("#Pos#", PO.Pos.ToCppCtor())
									.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
									.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
									.Replace("#ComponentsMethods#", PO.GetComponentsMethodsImplementation())
									.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
									.Replace("#OnStart#", PO.GetComponentsOnStart())
									.Replace("#ClassName#", PO.ClassName)
									.Replace("#SceneName#", PO._scene.ClassName)
							);
			}
			internal static void writeRenderableObject(FileStream declaration, FileStream implementation, RenderableObject PO)
			{
				writeStdInc(declaration);
				WriteLnIn(declaration, templates["Class:RenderableObject:Declaration"]
									.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
									.Replace("#Pos#", PO.Pos.ToCppCtor())
									.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
									.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
									.Replace("#ComponentsMethods#", PO.GetComponentsMethodsDeclaration())
									.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
									.Replace("#OnRender#", PO.GetComponentsOnRender())
									.Replace("#OnStart#", PO.GetComponentsOnStart())
									.Replace("#ClassName#", PO.ClassName)
									.Replace("#SceneName#", PO._scene.ClassName)
							);
				WriteLnIn(implementation, templates["Class:RenderableObject:Implementation"]
									.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
									.Replace("#Pos#", PO.Pos.ToCppCtor())
									.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
									.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
									.Replace("#ComponentsMethods#", PO.GetComponentsMethodsImplementation())
									.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
									.Replace("#OnRender#", PO.GetComponentsOnRender())
									.Replace("#OnStart#", PO.GetComponentsOnStart())
									.Replace("#ClassName#", PO.ClassName)
									.Replace("#SceneName#", PO._scene.ClassName)
							);
			}
			internal static void writeScene(FileStream fs, Scene S)
			{
				writeStdInc(fs);
				foreach(var obj in S.LayerList)
					WriteLnIn(fs, "#include \"" + obj.ClassName + ".h\"");//includes

				var objects = new List<string>();
				foreach (var obj in S.LayerList)
				{
					objects.Add("friend class " + obj.ClassName + ';');//friend class
					objects.Add(obj.ClassName + ' ' + obj.ObjectName + ';');//obj declarations
				}//objects filling

				string ctors = "";
				for(int i = 0; i < S.LayerList.Count - 1; ++i)//
					ctors += S.LayerList[i].ObjectName + "(*this), ";//
				ctors += S.LayerList[S.LayerList.Count - 1].ObjectName + "(*this)";//constructors

				string render = "";
				foreach (var i in S.LayerList)
					if (i is RenderableObject)
					{
						render += "cam.render(" + i.ObjectName + ".onRender(cam));";//renders
					}

				string getObjects = "";
				foreach (var i in S.LayerList)
					getObjects += "template<>\n" + i.ClassName + " & getObject<" + i.ClassName + ">(){\nreturn " + i.ObjectName + ";\n}\n";

				WriteLnIn(fs, templates["Сlass:Scene:FDef"]
												.Replace("#ClassName#", S.ClassName)
												.Replace("#Objects#", Glance.GatherStringList(objects, '\n'))
												.Replace("#Ctors#", ctors)
												.Replace("#start#", S.GetAllObjectsOnStart())
												.Replace("#update#", S.GetAllObjectsOnUpdate())
												.Replace("#render#", render)
												.Replace("#getObjects#", getObjects)
							);
			}
			///<summary>Code generate</summary>
			internal static void GenerateCode()
			{
				foreach (var SO in	scenes[0].LayerList)//objects
				{
					if (SO.ImplementationFilePath == null || SO.ImplementationFilePath == "")
					{
						string implFileName = BuildSetting.sourceDir + SO.ClassName + ".cpp";
						File.Create(implFileName).Close();
						SO.ImplementationFilePath = implFileName;
					}
					if (SO.DeclarationFilePath == null || SO.DeclarationFilePath == "")
					{
						string declFileName = BuildSetting.sourceDir + SO.ClassName + ".h";
						File.Create(declFileName).Close();
						SO.DeclarationFilePath = declFileName;
					}
					SO.GenerateCode();
				}
				{//scenes
					var Scfs = File.Create(BuildSetting.sourceDir + scenes[0].ClassName + ".h");
					writeScene(Scfs, scenes[0]);
					Scfs.Close();
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