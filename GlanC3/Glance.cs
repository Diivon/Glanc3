using System;
using System.IO;
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
			///<summary>Directory, where scripts(.gcsc) are</summary>
			public static string scriptsDir;

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
			public static bool isRecompile;
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
		internal static class NameSetting
		{
			public static string SpriteName;
			public static string SpriteType;

			public static string AnimationName;
			public static string AnimationType;

			public static string AnimatorName;
			public static string AnimatorType;

			public static string ColliderName;
			public static string ColliderType;

			public static string ScriptVariablesRegionName;
			public static string ScriptMethodsRegionName;
			public static string ScriptConstructorsRegionName;
			public static string ScriptConstructorBodyRegionName;

			public static string ScriptOnUpdateSignature;
			public static string ScriptOnStartSignature;
			static NameSetting()
			{
				SpriteName = "sprite";
				SpriteType = "sprite_t";
				AnimationName = "animation";
				AnimationType = "animation_t";
				AnimatorName = "animator";
				AnimatorType = "animator_t";
				ColliderName = "collider";
				ColliderType = "collider_t";
				ScriptVariablesRegionName = "variables:";
				ScriptMethodsRegionName = "methods:";
				ScriptConstructorsRegionName = "constructors:";
				ScriptConstructorBodyRegionName = "constructor_body:";
				ScriptOnUpdateSignature = "void onUpdate(const float & dt)";
				ScriptOnStartSignature = "void onStart()";
			}
		}
		/// <summary>Contain all scene of the game</summary>
		private static List<Scene> scenes;
		public static void AddScene(Scene s)
		{
			scenes.Add(s);
		}
		public static void Build()
		{
			Init();
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

			if (BuildSetting.isRecompile)
			{
				foreach (var i in scenes[0].LayerList)
				{
					BuildSetting.complilerTargets.Add(Glance.BuildSetting.sourceDir + i.GetImplementationFileName());
					foreach (var y in i._objects)
						BuildSetting.complilerTargets.Add(Glance.BuildSetting.sourceDir + y.GetImplementationFileName());
				}
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
		}

		private static void Init()
		{
			var files = Directory.GetFiles(BuildSetting.settingsDir, "T_*.gcs");
			foreach (var file in files)
				ParseGCS(File.ReadAllLines(file), ref templates);
			ParseGCS(File.ReadAllLines(BuildSetting.settingsDir + "settings.gcs"), ref settings);
		}
		///<summary>collection of code presets for all occasions(Class templates as example)</summary>
		internal static Dictionary<string, string> templates;
		///<summary>else settings for building</summary>
		internal static Dictionary<string, string> settings;

		///<summary>Build application by rules</summary>
		///<summary>return string, which call compiler correctly</summary>
		internal static string CreateApplication()
        {
            if (!Directory.Exists(BuildSetting.sourceDir))
                Directory.CreateDirectory(BuildSetting.sourceDir);
			if (File.Exists(BuildSetting.outputDir + BuildSetting.exeName))
				File.Delete(BuildSetting.outputDir + BuildSetting.exeName);
			return "cl.exe " + BuildSetting.compilerKeys + ' ' + @"/Fe" + BuildSetting.outputDir + ' ' + BuildSetting.sourceDir + @"main.cpp " + GatherStringList(BuildSetting.complilerTargets, ' ') + " /link" + ' ' + BuildSetting.linkerKeys;
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
			if (dict.ContainsKey(""))
				dict.Remove("");
		}
		/// <summary>transform ({"foo", "bar"}, "!") to "foo!bar"</summary>
		internal static string GatherStringList(List<string> list, string connector)
		{
			return GatherStringList(list.ToArray(), connector);
		}
		/// <summary>transform ({"foo", "bar"}, '!') to "foo!bar"</summary>
		internal static string GatherStringList(List<string> list, char connector)
		{
			return GatherStringList(list, connector.ToString());
		}
		/// <summary>transform ({"foo", "bar"}, '!') to "foo!bar"</summary>
		internal static string GatherStringList(string[] list, char connector)
		{
			string result = "";
			for (var i = 0; i < list.Length - 2; ++i)
				result += list[i] + connector;
			result += list[list.Length - 1];
			return result;
		}
		/// <summary>transform ({"foo", "bar"}, "!") to "foo!bar"</summary>
		internal static string GatherStringList(string[] list, string connector)
		{
			//string result = "";
			//foreach (var i in list)
			//	result += i + connector;
			//return result;
			if (list.Length == 0)
				return "";
			string result = "";
			for (var i = 0; i < list.Length - 2; ++i)
				result += list[i] + connector;
			result += list[list.Length - 1];
			return result;
		}

		internal static string ToCppString(string s)
		{
			return '"' + s.Replace(@"\", @"\\") + '"';
		}
		internal static void MergeDictionary(ref Dictionary<string, string> left, Dictionary<string, string> right)
		{
			foreach (var i in right)
			{
				if (left.ContainsKey(i.Key))
					left[i.Key] += i.Value;
				else
					left.Add(i.Key, i.Value);
			}
		}
		internal static void PrintDict(Dictionary<string, string> a)
		{
			foreach (var i in a)
				Console.WriteLine("Key:" + i.Key + ":Value:" + i.Value + '~');
		}
		internal static void PrintList(List<string> a)
		{
			foreach (var i in a)
				Console.WriteLine("PrintList:" + i);
		}

		internal static string GetRetTypeFromSignature(string signature)
		{
			int pos = signature.LastIndexOf('(');
			pos = signature.Substring(0, pos - 1).LastIndexOf(' ');
			return signature.Substring(0, pos);
		}
		internal static string GetSignatureWithoutRetType(string signature)
		{
			//TODO: parse decltype() too
			int pos = signature.LastIndexOf('(');
			pos = signature.Substring(0, pos - 1).LastIndexOf(' ');
			return signature.Substring(pos).Trim();
		}
		internal static void GetInfoAboutFunction(string method, ref string retType, ref string anotherInfo)
		{
			retType = GetRetTypeFromSignature(method);
			anotherInfo = GetSignatureWithoutRetType(method);
		}
		internal static string floatToString(float d)
		{
			return d.ToString("0.00").Replace(',', '.') + "f";
		}
		internal static int getIndexOfPairedBracket(string line, int bracketPos)
		{
			var result = -1;
			var nestingLevel = -1;
			switch(line[bracketPos])
			{
				case '(':
					for (var i = bracketPos; i < line.Length; ++i)
						if (line[i] == '(')
							++nestingLevel;
						else if (line[i] == ')')
						{
							if (nestingLevel == 0)
							{ result = i; break; }
							else --nestingLevel;
						}
					break;
				case ')':
					for (var i = bracketPos; i > 0; --i)
						if (line[i] == ')')
							++nestingLevel;
						else if (line[i] == '(')
						{
							if (nestingLevel == 0)
							{ result = i; break; }
							else --nestingLevel;
						}
					break;
				default:
					throw new Exception("Exception" + line[bracketPos].ToString() + '!');
			}
			if (result == -1)
				throw new Exception("No Paired Bracket in line \"" + line + '"');
			return result;
		}
		internal static char getLastChar(string line)
		{
			return line[line.Length - 1];
		}
		internal delegate void Del(string i);
		internal static string[] gForEach(this string[] list, Del fun)
		{
			foreach (var i in list)
				fun(i);
			return list;
		}
		static Glance()
		{
			templates = new Dictionary<string, string>();
			settings = new Dictionary<string, string>();
			scenes = new List<Scene>();
		}
	}//class Glance
}//ns GC
