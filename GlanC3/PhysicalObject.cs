using System;
using System.IO;

namespace Glc
{
	public class PhysicalObject : GameObject
	{
		public Vec2 Pos;
		private static uint _count;
		static PhysicalObject() { _count = 0; }
		public PhysicalObject(Vec2 pos, bool isRenderable = true) : base()
		{
			IsRenderableAtStart = isRenderable;
			Scn = null;
			Pos = pos;
			ClassName = "PhysicalObject" + _count++;
			ObjectName = "Obj" + ClassName;
		}
		override public void GenerateCode()
		{
			if (_filePath == "" || _filePath == null)
				throw new Exception("File do not exist for " + ClassName + ", when GenerateCode called");
			if (Scn == null)
				throw new Exception("Object " + ClassName + " haven't Scene, when GenerateCode called");
			var fs = File.Open(_filePath, FileMode.Append);
			Glance.CodeGenerator.writePhysicalObject(fs, this);
			fs.Close();
		}
	}
}
