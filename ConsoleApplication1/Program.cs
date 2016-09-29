using GC;
using System.Collections.Generic;
using System.Diagnostics;
using System;

class Program {
    static void Main(string[] args) {

		Glance.outputDir =		@"D:\GC\out\";
		Glance.sourceDir =		@"D:\GC\src\";
		Glance.includeDir =		@"D:\GC\include\";
		Glance.libDir =			@"D:\GC\lib\SFML\";
		Glance.settingsDir =	@"D:\GC\settings\";

		Glance.libs.Add("sfml-graphics.lib");
		Glance.libs.Add("sfml-window.lib");
		Glance.libs.Add("sfml-system.lib");
		Glance.libs.Add("sfml-audio.lib");
		Glance.libs.Add("sfml-network.lib");
		Glance.complilerTargets.AddRange(Glance.libs);

		Glance.compilerKeys =	@"/EHsc " + " /I" + Glance.sourceDir + " /I" + Glance.includeDir;
		Glance.linkerKeys =		@"/LIBPATH:" + Glance.libDir;

		Glance.exeName = "main.exe";

		Glance.isGenerateCode = true;
		Glance.isCompile = true;
		Glance.isRunAppAfterCompiling = true;

		Glance.Init();

		for (uint x = 0; x < 15; ++x)
			for(uint y = 0; y < 15; ++y)
				Glance.spriteObjects.Add(new SpriteObject(new Vec2(x * 50, y * 50), @"resources\Sea.jpg", true));

		Glance.Build();
    }
}