using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC
{
	public class StaticObject : GameObject
	{
		public Vec2 Pos;
		public string PicPath;
		public string Name;
		public string ObjName;
		private static uint _count;
		public StaticObject(Vec2 pos, string picPath, bool isRenderable = true) : base()
		{
			Pos = pos;
			PicPath = picPath;
			Name = "StaticObject" + _count++;
			ObjName = "Obj" + Name;
		}
		override public void GenerateFile()
		{
			FilePath = Glance.sourceDir + Name + ".h";
			var fs = System.IO.File.Create(FilePath);
			Glance.CodeGenerator.writeSpriteObject(fs, this);
		}
	}
}
