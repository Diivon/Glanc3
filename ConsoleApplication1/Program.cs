using Glc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

		Glance.isClearSrcDir = true;
		Glance.isGenerateCode = true;
		Glance.isCompile = true;
		Glance.isRunAppAfterCompiling = true;
		Glance.Init();
		//-------------Client code starts here

		
		var scene = new Scene();
		
		var Obj = new PhysicalObject(new Vec2(200, 200));
		Obj.SetGraphicalComponent(new Glc.Component.GraphicalComponent.StaticSprite(@"resources\n\1.jpg"));
		scene.ObjectList.Add(Obj);

		var o1 = new PhysicalObject(new Vec2(10, 20));
		var anim = new Glc.Component.GraphicalComponent.Animation(Glc.Component.AnimationType.Cyclic);
		anim.AddFrame(@"resources\n\1.jpg", 500);
		anim.AddFrame(@"resources\n\2.jpg", 500);
		anim.AddFrame(@"resources\n\3.jpg", 500);
		anim.AddFrame(@"resources\n\4.jpg", 500);
		o1.SetGraphicalComponent(anim);
		o1.AddComponent(new Glc.Component.Script(@"C:\Users\Влад\Desktop\kek.cpp"));
		o1.AddComponent(new Glc.Component.Script(@"C:\Users\Влад\Desktop\jaj.cpp"));
		scene.ObjectList.Add(o1);

		Glance.scenes.Add(scene);

		//-------------Client code ends here

		Glance.Build();
    }
}