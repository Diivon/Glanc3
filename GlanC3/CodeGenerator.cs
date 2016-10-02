using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GC
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
				if (data[data.Length - 1] == '{') ++TabCount;
				if (data[0] == '}') --TabCount;
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs) + '\n');
				fs.Write(kek, 0, kek.Length);
			}
			///<summary>write data -> fs considering TabCount</summary>
			internal static void WriteIn(FileStream fs, string data)
			{
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs));
				fs.Write(kek, 0, kek.Length);
			}
			///<summary>write "StandartIncludes" from settings.gcs -> fs</summary>
			internal static void writeStdInc(FileStream fs)
			{
				WriteLnIn(fs, presets["StandartIncludes"]);
			}
			///<summary>horror, i know</summary>
			internal static void writeMainCpp(FileStream fs)
			{
				var temp = FStream;
				FStream = fs;

				WriteLn("#include \"main.h\"");
				WriteLn("int main(){");
				WriteLn("sf::RenderWindow window(sf::VideoMode(800,600), \"kek\", sf::Style::Close);");
				WriteLn("gc::Camera mainCamera(gc::Vec2(0, 0), window);");
				WriteLn("float dt = 0.5f;");
				foreach (var SO in PhysicalObjects)//ctors
				{
					WriteLn(SO.Name + " Obj" + SO.Name + '(' +  SO.Pos.ToCppCtor() + ");");
				}
				WriteLn("\n");
				foreach (var SO in PhysicalObjects)//start loop
				{
					WriteLn(SO.ObjName + ".onStart();");
				}
				WriteLn("while(window.isOpen()){");
				WriteLn("sf::Event event;");
				WriteLn("while(window.pollEvent(event)){");
				WriteLn("if(event.type == sf::Event::Closed){window.close(); continue;}");
				WriteLn("}");//poll event

				WriteLn("window.clear();");
				foreach (var SO in PhysicalObjects)//update loop
				{
					WriteLn(SO.ObjName + ".onUpdate(dt);");
				}

				foreach (var SO in PhysicalObjects)//render loop
				{
					WriteLn("mainCamera.render(" + SO.ObjName + ".onRender());");
				}
				WriteLn("window.display();");
				WriteLn("}");//while window is open
				WriteLn("}");//main end

				FStream = temp;
			}
			///<summary>include everything, what must know main.cpp</summary>
			internal static void writeMainH(FileStream fs)
			{
				writeStdInc(fs);
				WriteLnIn(fs, "#include \"GC/Camera.h\"");
				foreach (var SO in PhysicalObjects)
					WriteLnIn(fs, "#include \"" + SO.Name + ".h\"");
			}
			///<summary>write SpriteObject template from settings.gcs -> fs</summary>
			internal static void writePhysicalObject(FileStream fs, PhysicalObject PO)
			{
				writeStdInc(fs);
				WriteLnIn	(fs, presets["PhysicalObject:FDef"].
									Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors()).
									Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody()).
									Replace("#ComponentsVariables#", PO.GetComponentsVariables()).
									Replace("#ComponentsMethods#", PO.GetComponentsMethods()).
									Replace("#OnRender#", PO.GetGraphicalComponentOnRender()).
									Replace("#ClassName#", PO.Name)
							);
			}
			///<summary>Code generate</summary>
			public static void GenerateCode()
			{
				foreach (var SO in PhysicalObjects)
				{
					if (SO.FilePath == null)
					{
						var SOfs = File.Create(sourceDir + SO.Name + ".h");
						writePhysicalObject(SOfs, SO);
						SOfs.Close();
					}
				}
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
