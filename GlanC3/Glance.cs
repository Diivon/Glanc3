using System.IO;
using System.Text;
using System;

namespace GC
{
    public static class Glance
    {
		public static string outputDir;
		public static string sourceDir;
		public static string includeDir;
		public static string libDir;
		public static string settingsDir;

		public static string compilerKeys;
		public static string linkerKeys;
		public static bool isRunAppAfterBuild;

		public static System.Collections.Generic.List<string> libs;
		public static System.Collections.Generic.List<string> complilerTargets;
		public static System.Collections.Generic.Dictionary<string, string> presets;
		public static System.Collections.Generic.List<SpriteObject> spriteObjects;

		private static class CodeGenerator
		{
			private static FileStream FStream;
			private static byte TabCount;
			private static void WriteLn(string data)
			{
				if (data == null)
					return;
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs) + '\n');
				FStream.Write(kek, 0, kek.Length);
			}
			private static void Write(string data)
			{
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs));
				FStream.Write(kek, 0, kek.Length);
			}
			private static void WriteLnIn(FileStream fs, string data)
			{
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs) + '\n');
				fs.Write(kek, 0, kek.Length);
			}
			private static void WriteIn(FileStream fs, string data)
			{
				string tabs = "";
				for (byte i = 0; i < TabCount; ++i)
					tabs += '\t';
				byte[] kek = Encoding.Unicode.GetBytes(tabs + data.Replace("\n", '\n' + tabs));
				fs.Write(kek, 0, kek.Length);
			}
			private static void writeStdInc(FileStream fs)
			{
				WriteLnIn(fs, presets["StandartIncludes"]);
				//WriteLnIn(fs, "kek!");
			}
			private static void writeMainCpp(FileStream fs)
			{
				var temp = FStream;
				FStream = fs;

				WriteLn("#include \"main.h\"");
				WriteLn("int main(){");
				WriteLn("sf::RenderWindow window(sf::VideoMode(800,600), \"kek\", sf::Style::Close);");
				WriteLn("gc::Camera mainCamera(gc::Vec2(0, 0), window);");
				WriteLn("float dt = 0.5f;");
				foreach(var SO in spriteObjects)//ctors
				{
					WriteLn(SO.Name + " Obj" + SO.Name + "(gc::Vec2(" + SO.Pos.x + ',' + SO.Pos.y  + "), \"" + SO.PicPath + "\", " + SO.IsRenderableAtStart.ToString().ToLower() + ");");
				}
				foreach (var SO in spriteObjects)//start loop
				{
					WriteLn("Obj" + SO.Name + ".start();");
				}
				WriteLn("while(window.isOpen()){");
				WriteLn("sf::Event event;");
				WriteLn("while(window.pollEvent(event)){");
				WriteLn("if(event.type == sf::Event::Closed){window.close(); continue;}");
				WriteLn("}");//poll event

				foreach (var SO in spriteObjects)//update loop
				{
					WriteLn("Obj" + SO.Name + ".update(dt);");
				}

				foreach (var SO in spriteObjects)//render loop
				{
					WriteLn("mainCamera.render(" + "Obj" + SO.Name + ");");
					WriteLn("Obj" + SO.Name + ".render();");
				}
				WriteLn("window.display();");
				WriteLn("}");//while window is open
				WriteLn("}");//main end

				FStream = temp;
			}
			private static void writeMainH(FileStream fs)
			{
				writeStdInc(fs);
				WriteLnIn(fs, "#include \"GC/Camera.h\"");
				foreach (var SO in spriteObjects)
					WriteLnIn(fs, "#include \"" + SO.Name + ".h\"");
			}
			private static void writeSpriteObject(FileStream fs, SpriteObject SO)
			{
				var temp = FStream;
				FStream = fs;

				writeStdInc(fs);
				WriteLn("class " + SO.Name + "\n{");
				WriteLn("friend class gc::Camera;");
				++TabCount;
				WriteLn(presets["SpriteObject:Def"]);
				--TabCount;
				WriteLn("public:");
				++TabCount;
				WriteLn(presets["SpriteObject:Ctor"].Replace("#ClassName#", SO.Name));
				WriteLn("void start(){");
				++TabCount;
				WriteLn(SO.OnStart);
				--TabCount;
				WriteLn("}");
				WriteLn("void update(const float & dtime){");		//update
				++TabCount;
				WriteLn(SO.OnUpdate);
				--TabCount;
				WriteLn("}");
				WriteLn("void render(){");							//render
				++TabCount;
				WriteLn(SO.OnRender);
				--TabCount;
				WriteLn("}");
				--TabCount;
				WriteLn("};");										//end class description

				FStream = temp;
			}
			public static void GenerateCode(string outputPath)
			{
				foreach(var SO in spriteObjects)
				{
					var SOfs = File.Create(sourceDir + SO.Name + ".h");
					writeSpriteObject(SOfs, SO);
					SOfs.Close();
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
        public static string CreateApplication()
        {
            if (!Directory.Exists(sourceDir))
                Directory.CreateDirectory(sourceDir);
			if (File.Exists(outputDir + "main.exe"))
				File.Delete(outputDir + "main.exe");

			CodeGenerator.GenerateCode(sourceDir);

			return "cl.exe " + compilerKeys + ' ' + @"/Fe" + outputDir + ' ' + sourceDir + @"main.cpp " + SpreadStringList(complilerTargets, ' ') + " /link" + ' ' + linkerKeys;
		}
		public static void PutIn(System.Collections.Generic.List<SpriteObject> SOs)
		{
			spriteObjects.AddRange(SOs);
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
			isRunAppAfterBuild = false;
			libs = new System.Collections.Generic.List<string>();
			complilerTargets = new System.Collections.Generic.List<string>();
			presets = new System.Collections.Generic.Dictionary<string, string>();
			spriteObjects = new System.Collections.Generic.List<SpriteObject>();
		}
		public static void Init()
		{
			//Parse logic
			{
				string[] strings = File.ReadAllLines(settingsDir + "presets.gcs");
				string mkey = "";
				string mvalue = "";
				for (int i = 0; i < strings.Length; ++i)
				{
					if (strings[i][0] != '\t')
					{
						presets.Add(mkey, mvalue);//saving previous pair
						mvalue = "";
						mkey = strings[i];
					}
					else
					{
						mvalue += strings[i].Remove(0, 1).Replace('$', '\n');
					}
				}
				presets.Add(mkey, mvalue);
				presets.Remove("");
				Console.WriteLine(presets.Count);
			}
		}
		public static string SpreadStringList(System.Collections.Generic.List<string> list, char connector)
		{
			string result = "";
			foreach (string str in list)
				result += str + connector;
			return result;
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

	public class SpriteObject
	{
		public Vec2 Pos;
		public string PicPath;
		public bool IsRenderableAtStart;
		public string OnStart;
		public string OnUpdate;
		public string OnRender;
		public string Name { get; }
		private uint _count;
		public SpriteObject(Vec2 pos, string picPath, bool isRenderable = true, string onStart = null, string onUpdate = null, string onRender = null)
		{
			Pos = pos;
			PicPath = picPath;
			IsRenderableAtStart = isRenderable;
			OnStart = onStart;
			OnUpdate = onUpdate;
			OnRender = onRender;
			Name = "SpriteObject" + _count++;
		}

		public SpriteObject()
		{
		}
	}
}//ns GC
