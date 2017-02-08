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
			internal static void writeLayer(FileStream declaration, FileStream implementation, Layer l)
			{
				writeStdInc(declaration);
				{
					string getObjects = "";
					foreach (var i in l._objects)
						getObjects += "template<>\n" + i.ClassName + ' ' + "getObject(){\nreturn " + i.ObjectName + ";\n}";

					WriteLnIn(declaration, templates["Class:Layer:Declaration"]
										.Replace("#SceneName#", l._scene.ClassName)
										.Replace("#ClassName#", l.ClassName)
										.Replace("#ObjectVariables#", l.GetVariables())
										.Replace("#ComponentsVariables#", "")
										.Replace("#ComponentsMethodsDeclaration#", l.GetMethodsDeclaration())
										.Replace("#getObjects#", getObjects)
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
				}
			}
			internal static void writeScene(FileStream fs, Scene S)
			{
				writeStdInc(fs);
				foreach(var obj in S.LayerList)                 //includes
					WriteLnIn(fs, "#include \"" + obj.ClassName + ".h\"");

				string layers = "";
				foreach (var i in S.LayerList)
					layers += i.ClassName + ' ' + i.ObjectName;	//variables

				string ctors = "";
				foreach (var i in S.LayerList)
					ctors += S.ObjectName + "(*this), ";		//constructors
				ctors.Remove(ctors.Length - 2, 2);

				string start = "";
				foreach (var i in S.LayerList)
					start += i.ObjectName + ".onStart();\n";    //onStarts

				string update = "";
				foreach (var i in S.LayerList)
					update += i.ObjectName + ".onUpdate(dt);\n";//onUpdates

				string getLayers = "";
				foreach (var i in S.LayerList)					//getLayers
					getLayers += "template<>\n" + i.ClassName + " & getLayer(){\n" + "return " + i.ObjectName + ";\n}";

				WriteLnIn(fs, templates["Сlass:Scene:FDef"]
									.Replace("#ClassName#", S.ClassName)
									.Replace("#Layers#", layers)
									.Replace("#Ctors#", ctors)
									.Replace("#start#", start)
									.Replace("#update#", update)
									.Replace("#getLayers#", getLayers)
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
				{//layers	
					foreach (var i in scenes)
						foreach(var o in i.LayerList)
						{
							var Lrhfs = File.Create(BuildSetting.sourceDir + o.ClassName + ".h");
							var Lrcppfs = File.Create(BuildSetting.sourceDir + o.ClassName + ".cpp");
							writeLayer(Lrhfs, Lrcppfs, o);
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