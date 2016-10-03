﻿using Glc;
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

		/*
		var Obj = new PhysicalObject(new Vec2(200, 200));
		//Obj.SetGraphicalComponent(new GC.Component.GraphicalComponent.StaticSprite(@"resources\n\1.jpg"));
		Obj.SetGraphicalComponent(new Glc.Component.GraphicalComponent.Animation(Glc.Component.AnimationType.Single));
		Glance.PhysicalObjects.Add(Obj);
		*/

		Glc.Component.Script scr = new Glc.Component.Script();
		scr.Data = System.IO.File.ReadAllLines(@"C:\Users\Влад\Desktop\kek.cpp");
		var s = scr.GetCppOnStart();
		Console.Write(s);
		Console.ReadKey();

		//-------------Client code ends here
		//Glance.Build();
    }
}