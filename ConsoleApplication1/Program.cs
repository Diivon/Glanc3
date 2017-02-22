using Glc;
using System;

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
		Glance.BuildSetting.complilerTargets.AddRange(Glance.BuildSetting.libs);

		Glance.BuildSetting.compilerKeys =	@"/EHsc " + " /I" + Glance.BuildSetting.sourceDir + " /I" + Glance.BuildSetting.includeDir;
		Glance.BuildSetting.linkerKeys =	@"/LIBPATH:" + Glance.BuildSetting.libDir;

		Glance.BuildSetting.exeName = "main.exe";

		Glance.BuildSetting.isClearSrcDir = true;
		Glance.BuildSetting.isGenerateCode = true;
		Glance.BuildSetting.isCompile = true;
		Glance.BuildSetting.isRunAppAfterCompiling = true;

		//-------------Client code starts here
		var scene = new Scene();
		var layer = new Layer();
		layer.AddScript(new Glc.Component.Script(@"a.cpp"));

		var obj1 = new RenderableObject(new Vec2(50, 50));
		obj1.ClassName = "Number1";
		obj1.GraphComponent = new Glc.Component.GraphicalComponent.Sprite(@"resources\n\1.jpg");
		obj1.Components.Add(new Glc.Component.Script("num1.cpp"));

		var obj2 = new RenderableObject(new Vec2(50, 100));
		obj2.ClassName = "Number2";
		var anim = new Glc.Component.GraphicalComponent.Animation(Glc.Component.GraphicalComponent.AnimationType.PingPong);
		anim.AddFrame(new SpriteFrame(@"resources\n\1.jpg", 500));
		anim.AddFrame(new SpriteFrame(@"resources\n\2.jpg", 500));
		anim.AddFrame(new SpriteFrame(@"resources\n\3.jpg", 500));
		anim.AddFrame(new SpriteFrame(@"resources\n\4.jpg", 500));

		obj2.GraphComponent = anim;
		obj2.Components.Add(new Glc.Component.Script("num2.cpp"));

		Glance.scenes.Add(scene);
		scene.AddLayer(layer);
		layer.AddObject(obj1);
		layer.AddObject(obj2);
		//-------------Client code ends here

		Glance.Build();
		GC.Collect();
		Console.ReadKey();
    }
}