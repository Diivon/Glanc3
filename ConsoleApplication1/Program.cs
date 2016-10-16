using Glc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

class Program {
    static void Main(string[] args) {

		Glance.BuildSetting.outputDir =		@"D:\GC\out\";
		Glance.BuildSetting.sourceDir =		@"D:\GC\src\";
		Glance.BuildSetting.includeDir =	@"D:\GC\include\";
		Glance.BuildSetting.libDir =		@"D:\GC\lib\SFML\";
		Glance.BuildSetting.settingsDir =	@"D:\GC\settings\";

		Glance.BuildSetting.libs.Add("sfml-graphics.lib");
		Glance.BuildSetting.libs.Add("sfml-window.lib");
		Glance.BuildSetting.libs.Add("sfml-system.lib");
		Glance.BuildSetting.libs.Add("sfml-audio.lib");
		Glance.BuildSetting.libs.Add("sfml-network.lib");
		Glance.BuildSetting.complilerTargets.AddRange(Glance.BuildSetting.libs);

		Glance.BuildSetting.compilerKeys =	@"/EHsc " + " /I" + Glance.BuildSetting.sourceDir + " /I" + Glance.BuildSetting.includeDir;
		Glance.BuildSetting.linkerKeys =	@"/LIBPATH:" + Glance.BuildSetting.libDir;

		Glance.BuildSetting.exeName = "main.exe";

		Glance.BuildSetting.isClearSrcDir = true;
		Glance.BuildSetting.isGenerateCode = true;
		Glance.BuildSetting.isCompile = true;
		Glance.BuildSetting.isRunAppAfterCompiling = true;
		Glance.Init();
		//-------------Client code starts here

		
		var scene = new Scene();
		
		var Obj = new PhysicalObject(new Vec2(200, 200));
		Obj.GraphComponent = new Glc.Component.GraphicalComponent.StaticSprite(@"resources\n\1.jpg");
		scene.AddObject(Obj);
		/*
		var o1 = new PhysicalObject(new Vec2(10, 20));
		var anim = new Glc.Component.GraphicalComponent.Animation(Glc.Component.AnimationType.Cyclic);
		anim.AddFrame(@"resources\n\1.jpg", 500);
		anim.AddFrame(@"resources\n\2.jpg", 500);
		anim.AddFrame(@"resources\n\3.jpg", 500);
		anim.AddFrame(@"resources\n\4.jpg", 500);
		o1.SetGraphicalComponent(anim);
		o1.AddComponent(new Glc.Component.Script(@"C:\Users\Влад\Desktop\kek.cpp"));
		o1.AddComponent(new Glc.Component.Script(@"C:\Users\Влад\Desktop\jaj.cpp"));
		scene.AddObject(o1);*/

		Glance.scenes.Add(scene);

		//-------------Client code ends here

		Glance.Build();
    }
}