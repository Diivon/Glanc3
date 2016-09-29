using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC
{
	public class Vec2
	{
		public float x, y;
		public Vec2(float x = 0, float y = 0)
		{
			this.x = x;
			this.y = y;
		}
		public string ToCppCtor()
		{
			return "gc::Vec2(" + x + ", " + y + ')';
		}
	}
}
