using Glc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

class Program {
    static void Main(string[] args) {

		Glance.BuildSetting.outputDir =		@"D:\Glc\out\";
		Glance.BuildSetting.sourceDir =		@"D:\Glc\src\";
		Glance.BuildSetting.includeDir =	@"D:\Glc\include\";
		Glance.BuildSetting.libDir =		@"D:\Glc\lib\SFML\";
		Glance.BuildSetting.settingsDir =	@"D:\Glc\settings\";

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

		var obj = new RenderableObject(new Vec2(50, 50));
		obj.GraphComponent = new Glc.Component.GraphicalComponent.StaticSprite(@"resources\n\1.jpg");

		layer.AddObject(obj);
		scene.AddLayer(layer);
		Glance.scenes.Add(scene);
		
		//-------------Client code ends here

		Glance.Build();

    }
}