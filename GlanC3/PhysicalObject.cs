using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC
{
	public class PhysicalObject : GameObject
	{
		public Vec2 Pos;
		public string Name;
		public bool IsRenderable;
		internal string ObjName;
		private static uint _count;
		public PhysicalObject(Vec2 pos, bool isRenderable = true) : base()
		{
			IsRenderable = isRenderable;
			Pos = pos;
			Name = "PhysicalObject" + _count++;
			ObjName = "Obj" + Name;
		}
		override public void GenerateFile()
		{
			FilePath = Glance.sourceDir + Name + ".h";
			var fs = System.IO.File.Create(FilePath);
			Glance.CodeGenerator.writePhysicalObject(fs, this);
		}
	}
}
