using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Glc
{
	public static partial class Glance
	{
		internal static partial class CodeGenerator
		{
			///<summary>FileStream for Write/WriteLn</summary>
			internal static FileStream FStream;
			///
			internal static byte TabCount;
			///<summary>write data + '\n' -> CodeGenerator.FStream considering TabCount</summary>
			internal static void WriteLn(string data)
			{
				if (data == null)
					return;
				if (data[data.Length - 1] == '{') ++TabCount;
				if (data[0] == '}') --TabCount;
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs) + '\n');
				FStream.Write(kek, 0, kek.Length);
			}
			///<summary>write data -> CodeGenerator.FStream considering TabCount</summary>
			internal static void Write(string data)
			{
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs));
				FStream.Write(kek, 0, kek.Length);
			}
			///<summary>write data + '\n' -> fs considering TabCount</summary>
			internal static void WriteLnIn(FileStream fs, string data)
			{
				if (data == null)
					return;
				string[] strings = data.Split('\n');
				byte tabcount = 0;
				foreach(var str in strings)
				{
					if (str.Contains('{'))
						++tabcount;
					if (str.Contains('}'))
						--tabcount;
					string finalstr = "";
					for (byte i = 0; i < tabcount; ++i)
						finalstr += '\t';
					finalstr += str.Trim(new char[] { '\t', ' '});
					byte[] finalBytes = Encoding.Unicode.GetBytes(finalstr + '\n');
					fs.Write(finalBytes, 0, finalBytes.Length);
				}
			}
			///<summary>write data -> fs considering TabCount</summary>
			internal static void WriteIn(FileStream fs, string data)
			{
				byte[] kek = Encoding.Unicode.GetBytes(data);
				fs.Write(kek, 0, kek.Length);
			}
			///<summary>write "StandartIncludes" from settings.gcs -> fs</summary>
			internal static void writeStdInc(FileStream fs)
			{
				WriteLnIn(fs, templates["Set:StandartIncludes:Def"]);
			}
			///<summary></summary>
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
									.Replace("#OnRender#", PO.GetComponentsOnRender())
									.Replace("#OnStart#", PO.GetComponentsOnStart())
									.Replace("#ClassName#", PO.ClassName)
									.Replace("#SceneName#", PO.Scn.ClassName)
							);
				WriteLnIn(implementation, templates["Class:PhysicalObject:Implementation"]
									.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
									.Replace("#Pos#", PO.Pos.ToCppCtor())
									.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
									.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
									.Replace("#ComponentsMethods#", PO.GetComponentsMethodsImplementation())
									.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
									.Replace("#OnRender#", PO.GetComponentsOnRender())
									.Replace("#OnStart#", PO.GetComponentsOnStart())
									.Replace("#ClassName#", PO.ClassName)
									.Replace("#SceneName#", PO.Scn.ClassName)
							);
			}
			internal static void writeRenderableObject(FileStream declaration, FileStream implementation, PhysicalObject PO)
			{
				writeStdInc(declaration);
				WriteLnIn(declaration, templates["Class:RenderableObject:Declaration"]
									.Replace(' ', ' ')
							);
				WriteLnIn(implementation, templates["Class:RenderableObject:Implementation"]
									.Replace(' ', ' ')
							);
			}
			internal static void writeScene(FileStream fs, Scene S)
			{
				writeStdInc(fs);
				foreach(var obj in S.ObjectList)
					WriteLnIn(fs, "#include \"" + obj.ClassName + ".h\"");//includes

				var objects = new List<string>();
				foreach (var obj in S.ObjectList)
				{
					objects.Add("friend class " + obj.ClassName + ';');//friend class
					objects.Add(obj.ClassName + ' ' + obj.ObjectName + ';');//obj declarations
				}//objects filling

				string ctors = "";
				for(int i = 0; i < S.ObjectList.Count - 1; ++i)//
					ctors += S.ObjectList[i].ObjectName + "(*this), ";//
				ctors += S.ObjectList[S.ObjectList.Count - 1].ObjectName + "(*this)";//constructors

				string render = "";
				foreach (var i in S.ObjectList)
					if(i is RenderableObject)
						render += "cam.render(" + i.ObjectName + ".onRender(cam));";//renders

				string getObjects = "";
				foreach (var i in S.ObjectList)
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
				foreach (var SO in	scenes[0].ObjectList)//objects
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
				TabCount = 0;
			}
		}
	}
}