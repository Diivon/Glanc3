using System;
using System.IO;

namespace Glc
{
	public class PhysicalObject : GameObject
	{
		public Vec2 Pos;
		public PhysicalObject(Vec2 pos) : base()
		{
			Scn = null;
			Pos = pos;
			ClassName = "PhysicalObject" + _count++;
			ObjectName = "Obj" + ClassName;
		}
		static PhysicalObject() { _count = 0; }

		override internal void GenerateCode()
		{
			if (_implementationfilePath == "" || _implementationfilePath == null)
				throw new Exception("File implementation do not exist for " + ClassName + ", when GenerateCode called");
			if (_declarationfilePath == "" || _declarationfilePath == null)
				throw new Exception("File declaration do not exist for " + ClassName + ", when GenerateCode called");
			if (Scn == null)
				throw new Exception("Object " + ClassName + " haven't Scene, when GenerateCode called");
			var impl = File.Open(_implementationfilePath, FileMode.Truncate);
			var decl = File.Open(_declarationfilePath, FileMode.Truncate);
			Glance.CodeGenerator.writePhysicalObject(decl, impl, this);
			impl.Close();
			decl.Close();
		}
		private static uint _count;
	}
}
