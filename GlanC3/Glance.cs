using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace GC
{
    public static partial class Glance
    {
		///<summary>Directory where launch-ready application is</summary>
		public static string outputDir;
		///<summary>Directory, where c++ sources (generated) is</summary>
		public static string sourceDir;
		///<summary>Direstory for #include</summary>
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
		///<summary>collection of code presets for all occasions(Class templates as example)</summary>
		public static Dictionary<string, string> presets;
		///<summary>else settings for building</summary>
		public static Dictionary<string, string> settings;
		///<summary>collection of all SpriteObjects, which must be in app</summary>
		public static List<StaticObject> spriteObjects;


		//												---/CodeGenerator/---
		///<summary>Buid application by rules</summary>
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
		///<summary>return string, which call compiler correctly</summary>
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

			spriteObjects = new List<StaticObject>();
		}
		public static void Init()
		{
			ParseGCS(File.ReadAllLines(settingsDir + "presets.gcs"), ref presets);
			ParseGCS(File.ReadAllLines(settingsDir + "settings.gcs"), ref settings);
		}
		///<summary>parse Glance settings(.gcs) file</summary>
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
		///<summary>kaef</summary>
		internal static string ToCppString(string s)
		{
			return '"' + s.Replace(@"\", @"\\") + '"';
		}
	}//class Glance
}//ns GC
