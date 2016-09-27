using GC;
using System.Collections.Generic;
using System.Diagnostics;
using System;

class Program {
    static void Main(string[] args) {
		Glance.isRunAppAfterBuild = true;

		Glance.outputDir =		@"D:\GC\out\";
		Glance.sourceDir =		@"D:\GC\out\src\";
		Glance.includeDir =		@"D:\GC\out\include\";
		Glance.libDir =			@"D:\GC\out\lib\SFML\";
		Glance.settingsDir =	@"D:\GC\out\settings\";

		Glance.libs.Add("sfml-graphics.lib");
		Glance.libs.Add("sfml-window.lib");
		Glance.libs.Add("sfml-system.lib");
		Glance.libs.Add("sfml-audio.lib");
		Glance.libs.Add("sfml-network.lib");
		Glance.complilerTargets.AddRange(Glance.libs);

		Glance.compilerKeys = @"/EHsc " + "/I" + Glance.includeDir;
		Glance.linkerKeys =		@"/LIBPATH:" + Glance.libDir;
		Glance.Init();
		Console.WriteLine(Glance.presets["SpriteObjectAdditionalFields"]);

		Glance.spriteObjects.Add(new SpriteObject(new Vec2(200,200), "kek.jpg", true));


		Process cmd = new Process();
		cmd.StartInfo = new ProcessStartInfo(@"cmd.exe");
		cmd.StartInfo.RedirectStandardInput = true;
		cmd.StartInfo.UseShellExecute = false;
		cmd.Start();
		cmd.StandardInput.WriteLine("vcvars32.bat");
		cmd.StandardInput.WriteLine(Glance.CreateApplication());

		if(Glance.isRunAppAfterBuild)
			cmd.StandardInput.WriteLine(Glance.outputDir + "main.exe");

		Console.ReadKey();
    }
}