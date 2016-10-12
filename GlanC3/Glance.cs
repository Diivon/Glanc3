using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Glc
{
    public static partial class Glance
    {
		/// <summary>settings for codeGenerating, compiling, linking</summary>
		public static class BuildSetting
		{
			///<summary>Directory where launch-ready application is</summary>
			public static string outputDir;
			///<summary>Directory, where c++ sources (generated) is</summary>
			public static string sourceDir;
			///<summary>Directory for #include</summary>
			public static string includeDir;
			///<summary>Directory, where .lib files are</summary>
			public static string libDir;
			///<summary>Directory, where Glance settings(.gcs) is</summary>
			public static string settingsDir;

			///<summary>keys for c++ compiler</summary>
			public static string compilerKeys;
			///<summary>keys for c++ linker</summary>
			public static string linkerKeys;

			///<summary>is Glance will generate code for Objects, which already haven't this when Buid() is called</summary>
			public static bool isGenerateCode;
			///<summary>is Glance will clear soure Directory before generate code</summary>
			///<remarks>BE CAREFULL!!!</remarks>
			public static bool isClearSrcDir;
			///<summary>is Glance will recompile sources when Buid() is called</summary>
			public static bool isCompile;
			///<summary>is Glance will run application after compiling</summary>
			public static bool isRunAppAfterCompiling;
			///<summary>name for .exe file of launch-ready application</summary>
			public static string exeName;
			///<summary>libs, which will linked with you code</summary>
			public static List<string> libs;
			///<summary>files, that compiler will compile/link (.cpp/.lib)</summary>
			public static List<string> complilerTargets;
			static BuildSetting()
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
			}
		}
		public static class NameSetting
		{
			public static string SpriteName;
			public static string SpriteType;

			public static string AnimationName;
			public static string AnimationType;

			public static string AnimatorName;
			public static string AnimatorType;

			static NameSetting()
			{
				SpriteName = "sprite";
				SpriteType = "sprite_t";
				AnimationName = "animation";
				AnimationType = "animation_t";
				AnimatorName = "animator";
				AnimatorType = "animator_t";
			}
		}
		///<summary>collection of code presets for all occasions(Class templates as example)</summary>
		internal static Dictionary<string, string> templates;
		///<summary>else settings for building</summary>
		internal static Dictionary<string, string> settings;
		/// <summary>Contain all scene of the game</summary>
		public static List<Scene> scenes;

		///<summary>Buid application by rules</summary>
		public static void Build()
		{
			if (BuildSetting.isGenerateCode)
			{
				if (BuildSetting.isClearSrcDir)
				{
					if (Directory.Exists(BuildSetting.sourceDir))
					{
						string[] files = Directory.GetFiles(BuildSetting.sourceDir);
						foreach (var file in files)
							File.Delete(file);
					}
					else Directory.CreateDirectory(BuildSetting.sourceDir);
				}
				Glance.CodeGenerator.GenerateCode();
			}
			if (BuildSetting.isRunAppAfterCompiling)
			{
				File.Delete(BuildSetting.outputDir + BuildSetting.exeName);
			}

			if (BuildSetting.isCompile)
			{
				Process cmd = new Process();
				cmd.StartInfo = new ProcessStartInfo(@"cmd.exe");
				cmd.StartInfo.RedirectStandardInput = true;
				cmd.StartInfo.UseShellExecute = false;
				cmd.Start();
				cmd.StandardInput.WriteLine(BuildSetting.settingsDir + Glance.settings["B:EnvVarsConfig"]);
				cmd.StandardInput.WriteLine(Glance.CreateApplication());
				if (BuildSetting.isRunAppAfterCompiling)
				{
					cmd.StandardInput.WriteLine("D:");
					cmd.StandardInput.WriteLine("cd " + BuildSetting.outputDir);
					cmd.StandardInput.WriteLine(BuildSetting.exeName);
				}
				cmd.Dispose();
			}
			Console.ReadKey();
		}
		///<summary>return string, which call compiler correctly</summary>
		private static string CreateApplication()
        {
            if (!Directory.Exists(BuildSetting.sourceDir))
                Directory.CreateDirectory(BuildSetting.sourceDir);
			if (File.Exists(BuildSetting.outputDir + BuildSetting.exeName))
				File.Delete(BuildSetting.outputDir + BuildSetting.exeName);
			return "cl.exe " + BuildSetting.compilerKeys + ' ' + @"/Fe" + BuildSetting.outputDir + ' ' + BuildSetting.sourceDir + @"main.cpp " + GatherStringList(BuildSetting.complilerTargets, ' ') + " /link" + ' ' + BuildSetting.linkerKeys;
		}

		static Glance()
		{
			templates = new Dictionary<string, string>();
			settings = new Dictionary<string, string>();

			scenes = new List<Scene>();
		}
		public static void Init()
		{
			var files = Directory.GetFiles(BuildSetting.settingsDir, "T_*.gcs");
			foreach (var file in files)
				ParseGCS(File.ReadAllLines(file), ref templates);
			ParseGCS(File.ReadAllLines(BuildSetting.settingsDir + "settings.gcs"), ref settings);
		}
		///<summary>parse Glance settings(.gcs) file</summary>
		internal static void ParseGCS(string[] strings, ref System.Collections.Generic.Dictionary<string, string> dict)
		{
			string mkey = "";
			string mvalue = "";
			foreach(var str in strings)
			{
				if (str == "") continue;
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
		/// <summary>transform ({"foo", "bar"}, '!') to "foo!bar"</summary>
		internal static string GatherStringList(System.Collections.Generic.List<string> list, char connector)
		{
			string result = "";
			foreach (string str in list)
				result += str + connector;
			return result;
		}
		///<summary>kaef</summary>
		internal static string ToCppString(string s)
		{
			return '"' + s.Replace(@"\", @"\\") + '"';
		}
	}//class Glance
}//ns GC
