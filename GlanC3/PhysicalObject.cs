using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glc
{
	public class PhysicalObject : GameObject
	{
		public Vec2 Pos;
		private static uint _count;
		public PhysicalObject(Vec2 pos, bool isRenderable = true) : base()
		{
			IsRenderableAtStart = isRenderable;
			Pos = pos;
			ClassName = "PhysicalObject" + _count++;
			ObjectName = "Obj" + ClassName;
		}
		override public void GenerateFile()
		{
			FilePath = Glance.sourceDir + ClassName + ".h";
			var fs = System.IO.File.Create(FilePath);
			Glance.CodeGenerator.writePhysicalObject(fs, this);
		}
	}
}
