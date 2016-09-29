using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC
{
	public abstract class GameObject
	{
		public string FilePath { get; protected set; }
		public GameObject()
		{
			FilePath = null;
		}
		abstract public void GenerateFile();
	}
}
