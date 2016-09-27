using System.IO;
using System.Text;

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

		public static void PutIn(System.Collections.Generic.List<SpriteObject> arg)
		{
			_spriteObjects.AddRange(arg);
		}

		private static void writeStandardIncludes(FileStream fs)
		{
			//WriteLn(fs, "#include <string>");
			//WriteLn(fs, "#include \"GC/Vec2\"");
			//WriteLn(fs, "#include \"SFML/Audio.hpp\"");
			//WriteLn(fs, "#include \"SFML/Graphics.hpp\"");
			//WriteLn(fs, "#include \"SFML/System.hpp\"");
			//WriteLn(fs, "#include \"SFML/Network.hpp\"");
			//WriteLn(fs, "\n");
			
			string s = "";
			if ((s = presets.TryGetValue("StandartIncludes")) != false)
				WriteLn(fs, s);
			else
				throw new System.Exception("Key \"StandartIncludes\" not found in presets.gsc");
		}

		private static void writeMainCode(FileStream fs)
		{
			WriteLn(fs, "#include \"SFML/Window.hpp\"");
			WriteLn(fs, "#include \"SFML/Graphics.hpp\"");
			WriteLn(fs, "int main(){");
			WriteLn(fs, "\tsf::RenderWindow window(sf::VideoMode(800,600), \"kek\", sf::Style::Close);");
			WriteLn(fs, "\twhile(window.isOpen()){");
			WriteLn(fs, "\t\tsf::Event event;");
			WriteLn(fs, "\t\twhile(window.pollEvent(event)){");
			WriteLn(fs, "\t\t\tif(event.type == sf::Event::Closed){window.close(); continue;}");
			WriteLn(fs, "\t\t}");//poll event
			WriteLn(fs, "");
			WriteLn(fs, "\twindow.draw(sf::CircleShape(200));");
			WriteLn(fs, "\twindow.display();");
			WriteLn(fs, "\t}");//while window is open
			WriteLn(fs, "}");//main end
		}

        private static string callCppCompiler(string param)
        {
			return "cl.exe" + ' ' + param;
        }
        public static string CreateApplication()
        {
            if (!Directory.Exists(sourceDir))
                Directory.CreateDirectory(sourceDir);
			if (File.Exists(outputDir + "main.exe"))
				File.Delete(outputDir + "main.exe");

			foreach(var i in _spriteObjects)
			{
				var sof = File.Create(sourceDir + i.Name + ".h");
				writeStandardIncludes(sof);
				WriteLn(sof, "class " + i.Name + '{');
				WriteLn(sof, _spriteObjectFields);
				WriteLn(sof, "public:");
				WriteLn(sof, "\tSpriteObject" + i + "(const DL::Vec2 & pos, std::string picPath, bool render = true):");
				WriteLn(sof, "\t\t_pos(pos), _picPath(picPath), isRenderable(render)");
				WriteLn(sof, "\t{}");								//ctor
				WriteLn(sof, "\tstart(){");						//start
				WriteLn(sof, i.OnStart);
				WriteLn(sof, "\t}");
				WriteLn(sof, "\tupdate(const float & dtime){");	//update
				WriteLn(sof, i.OnUpdate);
				WriteLn(sof, "\t}");
				WriteLn(sof, "\trender(){");						//render
				WriteLn(sof, i.OnRender);
				WriteLn(sof, "\t}");
				WriteLn(sof, "}");
				sof.Close();
			}

			var f = File.Create(sourceDir + "main.h");
			foreach(var i in _spriteObjects)
			{
				writeStandardIncludes(f);
				WriteLn(f, "#include \"" + i.Name + '"');
			}

            string mainName = sourceDir + "main.cpp";
            FileStream fs =  File.Create(@mainName);
			WriteLn(fs, "#include \"main.h\"");
            writeMainCode(fs);
            fs.Close();

			return callCppCompiler(compilerKeys + ' ' + @"/Fe" + outputDir + ' ' + sourceDir + @"main.cpp " + SpreadStringList(complilerTargets, ' ') + " /link" + ' ' + linkerKeys);
		}

		private static System.Collections.Generic.List<SpriteObject> _spriteObjects;
		private static string _spriteObjectFields;
		static Glance()
		{
			libs = new System.Collections.Generic.List<string>();
			complilerTargets = new System.Collections.Generic.List<string>();
			_spriteObjects = new System.Collections.Generic.List<SpriteObject>();
			presets = new Dictionary<string, string>();
			string[] strings = File.ReadAllLines(settingsDir + "presets.gcs");
			string mkey = "";
			string mvalue = "";
			for(int i = 0; i < strings.Length; ++i)
			{
				if(strings[i][0] != '\t'){
					presets.Add(mkey, mvalue)
					mkey = strings[i];
				}
				else 
					mvalue += strings[i];
			}
			presets.Remove("");
		}
		private static byte[] getBytes(string a)
		{
			return Encoding.Unicode.GetBytes(a);
		}
		private static void WriteLn(FileStream fs, string s)
		{
			byte[] kek = getBytes(s + '\n');
			fs.Write(kek, 0, kek.Length);
		}
		private static void Write(FileStream fs, string s)
		{
			byte[] kek = getBytes(s);
			fs.Write(kek, 0, kek.Length);
		}
		private static string SpreadStringList(System.Collections.Generic.List<string> list, char connector)
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
