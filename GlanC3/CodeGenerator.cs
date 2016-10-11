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
		public static partial class CodeGenerator
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
			///<summary>horror, i know</summary>
			internal static void writeMainCpp(FileStream fs)
			{
				/*
				var temp = FStream;
				FStream = fs;

				WriteLn("#include \"main.h\"");
				WriteLn("int main(){");
				WriteLn("gc::Camera mainCamera(gc::Vec2(0, 0), sf::VideoMode(800,600), \"kek\", sf::Style::Close);");
				WriteLn("float dt = 0.5f;");
				WriteLn(scenes[0].ClassName + ' ' + scenes[0].ObjectName + ";");
				WriteLn("\n");
				WriteLn(scenes[0].ObjectName + ".start();");//start
				WriteLn("while(mainCamera.isOpen()){");
				WriteLn("sf::Event event;");
				WriteLn("while(mainCamera.pollEvent(event)){");
				WriteLn("if(event.type == sf::Event::Closed){mainCamera.close(); continue;}");
				WriteLn("}");//poll event

				WriteLn("mainCamera.clear();");

				WriteLn(scenes[0].ObjectName + ".update(dt);");//update

				WriteLn(scenes[0].ObjectName + ".render(mainCamera);");//render

				WriteLn("mainCamera.display();");
				WriteLn("}");//while window is open
				WriteLn("}");//main end

				FStream = temp;
				*/
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
			///<summary>write SpriteObject template from settings.gcs -> fs</summary>
			internal static void writePhysicalObject(FileStream fs, PhysicalObject PO)
			{
				writeStdInc(fs);
				WriteLnIn(fs, templates["Class:PhysicalObject:FDef"].
									Replace("#ComponentsVariables#", PO.GetComponentsVariables()).
									Replace("#Pos#" , PO.Pos.ToCppCtor()).
									Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors()).
									Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody()).
									Replace("#ComponentsMethods#", PO.GetComponentsMethods()).
									Replace("#OnUpdate#", PO.GetComponentsOnUpdate()).
									Replace("#OnRender#", PO.GetComponentsOnRender()).
									Replace("#OnStart#", PO.GetComponentsOnStart()).
									Replace("#ClassName#", PO.ClassName)
							);
			}
			internal static void writeScene(FileStream fs, Scene S)
			{
				writeStdInc(fs);
				foreach(var obj in S.ObjectList)
				{
					WriteLnIn(fs, "#include \"" + obj.ClassName + ".h\"");
				}
				var objects = new List<string>();
				foreach (var obj in S.ObjectList)
				{
					objects.Add("friend class " + obj.ClassName + ';');
					objects.Add(obj.ClassName + ' ' + obj.ObjectName + ';');
				}//objects filling
				string ctors = "";
				for(int i = 0; i < S.ObjectList.Count - 1; ++i)
					ctors += S.ObjectList[i].ObjectName + "(), ";
				ctors += S.ObjectList[S.ObjectList.Count - 1].ObjectName + "()";
				WriteLnIn(fs, templates["Scene:FDef"].
												Replace("#ClassName#", S.ClassName).
												Replace("#Objects#", Glance.GatherStringList(objects, '\n')).
												Replace("#Ctors#", ctors).
												Replace("#start#", S.GetAllObjectsOnStart()).
												Replace("#update#", S.GetAllObjectsOnUpdate()).
												Replace("#render#", render)
							);
			}
			///<summary>Code generate</summary>
			public static void GenerateCode()
			{
				foreach (var SO in	scenes[0].ObjectList)
				{
					if (SO.FilePath == null)
					{
						var SOfs = File.Create(sourceDir + SO.ClassName + ".h");
						writePhysicalObject(SOfs, SO as PhysicalObject);
						SOfs.Close();
					}
				}

				var Scfs = File.Create(sourceDir + scenes[0].ClassName + ".h");
				writeScene(Scfs, scenes[0]);
				Scfs.Close();

				{
					var mainHfs = File.Create(sourceDir + "main.h");
					writeMainH(mainHfs);
					mainHfs.Close();
				}
				{
					var MainCppfs = File.Create(sourceDir + "main.cpp");
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