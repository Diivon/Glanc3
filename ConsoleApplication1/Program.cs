using Glc;
using System;
using System.Collections.Generic;

class Program {
    static void Main(string[] args) {

		Glance.BuildSetting.outputDir =		@"D:\Glc\out\";
		Glance.BuildSetting.sourceDir =		@"D:\Glc\src\";
		Glance.BuildSetting.includeDir =	@"D:\Glc\include\";
		Glance.BuildSetting.libDir =		@"D:\Glc\lib\SFML\";
		Glance.BuildSetting.settingsDir =	@"D:\Glc\settings\";
		Glance.BuildSetting.scriptsDir =	@"D:\Glc\scripts\";

		Glance.BuildSetting.libs.Add("sfml-graphics.lib");
		Glance.BuildSetting.libs.Add("sfml-window.lib");
		Glance.BuildSetting.libs.Add("sfml-system.lib");
		Glance.BuildSetting.libs.Add("sfml-audio.lib");
		Glance.BuildSetting.libs.Add("sfml-network.lib");

		Glance.BuildSetting.compilerKeys =	@"/EHsc " + " /I" + Glance.BuildSetting.sourceDir + " /I" + Glance.BuildSetting.includeDir + @" /Zi";
		Glance.BuildSetting.linkerKeys =	@"/LIBPATH:" + Glance.BuildSetting.libDir;

		Glance.BuildSetting.exeName = "main.exe";

		Glance.BuildSetting.isClearSrcDir = true;
		Glance.BuildSetting.isGenerateCode = true;
		Glance.BuildSetting.isRecompile = true;
		Glance.BuildSetting.isRunAppAfterCompiling = true;

		//-------------Client code starts here
		
		var scene = new Scene();
		var layer1 = new Layer();
		var layer2 = new Layer();
		layer1.AddScript(new Glc.Component.Script(@"layer.cpp"));

		var obj1 = new RenderableObject(new Vec2(50, 50));
		obj1.ClassName = "Number1";
		obj1.GraphComponent = new Glc.Component.GraphicalComponent.Sprite(@"resources\n\1.jpg");
		obj1.AddComponent(new Glc.Component.Script("num1.cpp"));
		obj1.AddComponent(new Glc.Component.Script("num1-detect-collide.cpp"));
		obj1.AddComponent(new Glc.Component.Collider(Glc.Component.Collider.Type.Rectangle).SetPosition(obj1.Pos).SetSize(new Vec2(50, 50)));

		var obj2 = new RenderableObject(new Vec2(50, 100));
		obj2.ClassName = "Number2";
		var anim = new Glc.Component.GraphicalComponent.Animation(Glc.Component.GraphicalComponent.AnimationType.PingPong);
		anim.AddFrame(new SpriteFrame(@"resources\n\1.jpg", 500));
		anim.AddFrame(new SpriteFrame(@"resources\n\2.jpg", 500));
		anim.AddFrame(new SpriteFrame(@"resources\n\3.jpg", 500));
		anim.AddFrame(new SpriteFrame(@"resources\n\4.jpg", 500));
		obj2.GraphComponent = anim;
		obj2.AddComponent(new Glc.Component.Script("num2.cpp"));
		obj2.AddComponent(new Glc.Component.Collider(Glc.Component.Collider.Type.Rectangle).SetPosition(obj2.Pos).SetSize(new Vec2(50, 50)));
		var phys1 = new PhysicalObject(new Vec2(-200, -200));

		//Glc.Component.Script.CreateFile("@MyFile.cpp");
		
		layer1.AddObject(obj1);
		layer1.AddObject(obj2);
		layer2.AddObject(phys1);
		scene.AddLayer(layer1);
		scene.AddLayer(layer2);
		Glance.AddScene(scene);
		//-------------Client code ends here
		Glance.Build();

		Console.ReadKey();
    }
}