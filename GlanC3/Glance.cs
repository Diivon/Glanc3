using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace GC
{
    public static class Glance
    {
		///Directory where launch-ready application is
		public static string outputDir;
		///Directory, where c++ sources (generated) is
		public static string sourceDir;
		///Direstory for #include
		public static string includeDir;
		///Directory, where .lib files are
		public static string libDir;
		///Directory, where Glance settings(.gcs) is
		public static string settingsDir;
		
		///keys for c++ compiler
		public static string compilerKeys;
		///keys for c++ linker
		public static string linkerKeys;
		
		///is Glance will generate code for Objects, which already haven't this when Buid() is called
		public static bool isGenerateCode;
		///is Glance will recompile sources when Buid() is called
		public static bool isCompile;
		///is Glance will run application after compiling
		public static bool isRunAppAfterCompiling;
		///name for .exe file of launch-ready application
		public static string exeName;
		
		///libs, which will linked with you code
		public static List<string> libs;
		///files, that compiler will compile/link (.cpp/.lib)
		public static List<string> complilerTargets;
		///collection of code presets for all occasions(Class templates as example)
		public static Dictionary<string, string> presets;
		///else settings for building
		public static Dictionary<string, string> settings;
		///collection of all SpriteObjects, which must be in app
		public static List<SpriteObject> spriteObjects;

		//												----CodeGenerator----
		public static class CodeGenerator
		{
			///FileStream for Write/WriteLn
			internal static FileStream FStream;
			///
			internal static byte TabCount;
			///write data + '\n' -> CodeGenerator.FStream considering TabCount
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
			///write data -> CodeGenerator.FStream considering TabCount
			internal static void Write(string data)
			{
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs));
				FStream.Write(kek, 0, kek.Length);
			}
			///write data + '\n' -> fs considering TabCount
			internal static void WriteLnIn(FileStream fs, string data)
			{
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs) + '\n');
				fs.Write(kek, 0, kek.Length);
			}
			///write data -> fs considering TabCount
			internal static void WriteIn(FileStream fs, string data)
			{
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs));
				fs.Write(kek, 0, kek.Length);
			}
			///write "StandartIncludes" from settings.gcs -> fs
			internal static void writeStdInc(FileStream fs)
			{
				WriteLnIn(fs, presets["StandartIncludes"]);
			}
			///horror, i know
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
				foreach(var SO in spriteObjects)//ctors
				{
					WriteLn(SO.Name + " Obj" + SO.Name + "(gc::Vec2(" + SO.Pos.x + ", " + SO.Pos.y + "), " + ToCppString(SO.PicPath) + ");");
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
			///include everything, what must know main.cpp
			internal static void writeMainH(FileStream fs)
			{
				writeStdInc(fs);
				WriteLnIn(fs, "#include \"GC/Camera.h\"");
				foreach (var SO in spriteObjects)
					WriteLnIn(fs, "#include \"" + SO.Name + ".h\"");
			}
			///write SpriteObject template from settings.gcs -> fs
			internal static void writeSpriteObject(FileStream fs, SpriteObject SO)
			{
				writeStdInc(fs);
				WriteLnIn(fs, presets["SpriteObject:FDef"].Replace("#ClassName#", SO.Name));
			}
			///Code generate
			public static void GenerateCode(string outputPath)
			{
				foreach(var SO in spriteObjects)
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
		//												---/CodeGenerator/---
		///Buid application by rules
		public static void Build()
		{
			if (Glance.isGenerateCode)
			{
				Glance.CodeGenerator.GenerateCode(Glance.outputDir);
			}

			if (Glance.isCompile)
			{
				Process cmd = new Process();
				cmd.StartInfo = new ProcessStartInfo(@"cmd.exe");
				cmd.StartInfo.RedirectStandardInput = true;
				cmd.StartInfo.UseShellExecute = false;
				cmd.Start();
				cmd.StandardInput.WriteLine(Glance.settingsDir + Glance.settings["EnvVarsConfig"]);
				cmd.StandardInput.WriteLine(Glance.CreateApplication());
				if (Glance.isRunAppAfterCompiling)
				{
					cmd.StandardInput.WriteLine("D:");
					cmd.StandardInput.WriteLine("cd " + Glance.outputDir);
					cmd.StandardInput.WriteLine(Glance.exeName);
				}
				cmd.Dispose();
			}
			Console.ReadKey();
		}
		///return string, which call compiler correctly
		private static string CreateApplication()
        {
            if (!Directory.Exists(sourceDir))
                Directory.CreateDirectory(sourceDir);
			if (File.Exists(outputDir + exeName))
				File.Delete(outputDir + exeName);
			return "cl.exe " + compilerKeys + ' ' + @"/Fe" + outputDir + ' ' + sourceDir + @"main.cpp " + GatherStringList(complilerTargets, ' ') + " /link" + ' ' + linkerKeys;
		}

		static Glance()
		{
			outputDir = "";
			sourceDir = "";
			includeDir = "";
			libDir = "";
			settingsDir = "";
			compilerKeys = "";
			linkerKeys = "";
			libs = new List<string>();
			complilerTargets = new List<string>();
			presets = new Dictionary<string, string>();
			settings = new Dictionary<string, string>();

			spriteObjects = new List<SpriteObject>();
		}
		public static void Init()
		{
			ParseGCS(File.ReadAllLines(settingsDir + "presets.gcs"), ref presets);
			ParseGCS(File.ReadAllLines(settingsDir + "settings.gcs"), ref settings);
		}
		///parse Glance settings(.gcs) file
		internal static void ParseGCS(string[] strings, ref System.Collections.Generic.Dictionary<string, string> dict)
		{
			string mkey = "";
			string mvalue = "";
			foreach(var str in strings)
			{
				if (str[0] != '\t')//if string is key
				{
					dict.Add(mkey, mvalue);//saving previous pair
					mvalue = "";
					mkey = str;
				}
				else
				{
					mvalue += str.Remove(0,1) + '\n';//deleting '\t' as first cymbol
				}
			}
			dict.Add(mkey, mvalue);//save last pair
			dict.Remove("");//for guarantee
		}
		internal static string GatherStringList(System.Collections.Generic.List<string> list, char connector)
		{
			string result = "";
			foreach (string str in list)
				result += str + connector;
			return result;
		}
		///kaef
		internal static string ToCppString(string s)
		{
			return '"' + s.Replace(@"\", @"\\") + '"';
		}
	}//class Glance

	public class Vec2
	{
		public float x, y;
		public Vec2(float x = 0, float y = 0)
		{
			this.x = x;
			this.y = y;
		}
	}

	public abstract class GameObject
	{
		public string FilePath { get; protected set; }
		public GameObject()
		{
			FilePath = null;
		}
		abstract public void GenerateCode();
	}

	public class SpriteObject: GameObject
	{
		public Vec2 Pos;
		public string PicPath;
		public string Name;
		public string ObjName;
		private static uint _count;
		public SpriteObject(Vec2 pos, string picPath, bool isRenderable = true): base()
		{
			Pos = pos;
			PicPath = picPath;
			Name = "SpriteObject" + _count++;
			ObjName = "Obj" + Name;
		}
		override public void GenerateCode()
		{
			FilePath = Glance.sourceDir + Name + ".h";
			var fs = File.Create(FilePath);
			Glance.CodeGenerator.writeSpriteObject(fs, this);
		}
	}
}//ns GC
