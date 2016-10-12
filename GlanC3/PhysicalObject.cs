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
			Pos = pos;
			ClassName = "PhysicalObject" + _count++;
			ObjectName = "Obj" + ClassName;
		}
		override public void GenerateFile()
		{
			if (_filePath == "" || _filePath == null)
				throw new Exception("file do not exist, when GenerateFile called");
			Console.WriteLine("Glance.CodeGenerator.writePhysicalObject" + _filePath);
			var fs = File.Open(_filePath, FileMode.Append);
			Glance.CodeGenerator.writePhysicalObject(fs, this);
			fs.Close();
		}
	}
}
