﻿using Glc;
using System;
using System.Collections.Generic;

class Program {
    static void Main(string[] args)
	{
		Glance.BuildSetting.outputDir = @"D:\Glc\out\";
		Glance.BuildSetting.sourceDir = @"D:\Glc\src\";
		Glance.BuildSetting.includeDir = @"D:\Glc\include\";
		Glance.BuildSetting.libDir = @"D:\Glc\lib\SFML\";
		Glance.BuildSetting.settingsDir = @"D:\Glc\settings\";
		Glance.BuildSetting.scriptsDir = @"D:\Glc\scripts\";

		Glance.BuildSetting.libs.Add("sfml-graphics.lib");
		Glance.BuildSetting.libs.Add("sfml-window.lib");
		Glance.BuildSetting.libs.Add("sfml-system.lib");
		Glance.BuildSetting.libs.Add("sfml-audio.lib");
		Glance.BuildSetting.libs.Add("sfml-network.lib");

		Glance.BuildSetting.compilerKeys = @"/EHsc " + " /I" + Glance.BuildSetting.sourceDir + " /I" + Glance.BuildSetting.includeDir + @" /Zi";
		Glance.BuildSetting.linkerKeys = @"/LIBPATH:" + Glance.BuildSetting.libDir;

		Glance.BuildSetting.exeName = "main.exe";

		Glance.BuildSetting.isClearSrcDir = true;
		Glance.BuildSetting.isGenerateCode = true;
		Glance.BuildSetting.isRecompile = true;
		Glance.BuildSetting.isRunAppAfterCompiling = true;
		
		Glance.Init();
		//------------------------------------------------

		//script();
		game();
    }
	static void script()
	{
		Glc.Component.Script.CreateFile(@"D:\a.gcs");
	}
	static void game()
	{
		//-------------Client code starts here

		var scene = new Scene();
		var layer1 = new Layer();

		var hero = new RenderableObject(new Vec2());
		hero.ClassName = "Hero";
		hero.GraphComponent = new Glc.Component.GraphicalComponent.Sprite(@"resources\1.jpg");
		hero.AddComponent(new Glc.Component.Script(@"player.gcs"));

		var bullet = new RenderableObject(new Vec2());
		bullet.ClassName = "Bullet";
		bullet.GraphComponent = new Glc.Component.GraphicalComponent.Sprite(@"resources\bullet.jpg");
		bullet.AddComponent(new Glc.Component.Script(@"bullet.gcs"));

		layer1.AddObject(hero);
		layer1.AddObject(bullet);
		scene.AddLayer(layer1);
		Glance.AddScene(scene);
		//-------------Client code ends here
		Glance.Build();

		Console.ReadKey();
	}
}