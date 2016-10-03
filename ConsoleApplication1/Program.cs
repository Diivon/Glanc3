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
		//-------------Client code starts here

		var Obj = new GC.PhysicalObject(new Vec2(200, 200));
		Obj.SetGraphicalComponent(new GC.Component.GraphicalComponent.StaticSprite(@"resources\n\1.jpg"));
		Glance.PhysicalObjects.Add(Obj);

		//-------------Client code ends here
		Glance.Build();
    }
}