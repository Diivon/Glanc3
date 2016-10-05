using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glc
{
    public class SpriteFrame
    {
		public string PicName;
		public float Duration; 
		public SpriteFrame(string picName, float dur)
		{
			PicName = picName;
			Duration = dur;
		}
		string GetCppCtor()
		{
			return "::gc::SpriteFrame(" + Glance.ToCppString(PicName) + ", " + Duration + "f);";
		}
    }
}
