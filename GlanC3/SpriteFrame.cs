using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC
{
	public static partial class Glance
	{
		class SpriteFrame
		{
			/// <summary>path to sprite picture</summary>
			public string picPath;
			/// <summary>duration of this sprite in msec</summary>
			public float duration;
			SpriteFrame(string path, float dur)
			{
				picPath = path;
				duration = dur;
			}
		}
	}
}
