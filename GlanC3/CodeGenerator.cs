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
				++TabCount;
				WriteLn("sf::RenderWindow window(sf::VideoMode(800,600), \"kek\", sf::Style::Close);");
				WriteLn("gc::Camera mainCamera(gc::Vec2(0, 0), window);");
				WriteLn("float dt = 0.5f;");
				foreach (var SO in spriteObjects)//ctors
				{
					WriteLn(SO.Name + " Obj" + SO.Name + '(' +  SO.Pos.ToCppCtor() + ", " + ToCppString(SO.PicPath) + ");");
				}
				WriteLn("\n");
				foreach (var SO in spriteObjects)//start loop
				{
					WriteLn(SO.ObjName + ".OnStart();");
				}
				WriteLn("while(window.isOpen()){");
				++TabCount;
				WriteLn("sf::Event event;");
				++TabCount;
				WriteLn("while(window.pollEvent(event)){");
				WriteLn("if(event.type == sf::Event::Closed){window.close(); continue;}");
				--TabCount;
				WriteLn("}\nwindow.clear();");//poll event
				--TabCount;

				foreach (var SO in spriteObjects)//update loop
				{
					WriteLn(SO.ObjName + ".OnUpdate(dt);");
				}

				foreach (var SO in spriteObjects)//render loop
				{
					WriteLn("mainCamera.render(" + "Obj" + SO.Name + ");");
					WriteLn(SO.ObjName + ".OnRender(mainCamera);");
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
				foreach (var SO in spriteObjects)
					WriteLnIn(fs, "#include \"" + SO.Name + ".h\"");
			}
			///<summary>write SpriteObject template from settings.gcs -> fs</summary>
			internal static void writeSpriteObject(FileStream fs, StaticObject SO)
			{
				writeStdInc(fs);
				WriteLnIn(fs, presets["SpriteObject:FDef"].Replace("#ClassName#", SO.Name));
			}
			///<summary>Code generate</summary>
			public static void GenerateCode(string outputPath)
			{
				foreach (var SO in spriteObjects)
				{
					if (SO.FilePath == null)
					{
						var SOfs = File.Create(sourceDir + SO.Name + ".h");
						writeSpriteObject(SOfs, SO);
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
