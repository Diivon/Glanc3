using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Glc
{
	public class Scene
	{
		internal List<GameObject> ObjectList;
		private string _className;
		public string ClassName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_className = value;
				else throw new ArgumentException("Name must contain only letters, numbers and cymbol '_'");
			}
			get { return _className; }
		}
		private string _objectName;
		public string ObjectName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_objectName = value;
				else throw new ArgumentException("Name must contain only letters, numbers and cymbol '_'");
			}
			get { return _objectName; }
		}
		private static int _count;
		static Scene() { _count = 0; }
		public Scene()
		{
			ClassName = "Scene" + _count++;
			ObjectName = "Obj" + ClassName;
			ObjectList = new List<GameObject>();
		}
		public string GetAllObjectsOnStart()
		{
			string result = "";
			foreach (var i in ObjectList)
				result += i.ObjectName + ".onStart();\n";
			return result;
		}
		public string GetAllObjectsOnUpdate()
		{
			string result = "";
			foreach (var i in ObjectList)
				result += i.ObjectName + ".onUpdate(dt);\n";
			return result;
		}
		public string GetAllObjectsOnRender()
		{
			string result = "";
			foreach (var i in ObjectList)
				result += i.ObjectName + ".onRender()";
			return result;
		}
		public void AddObject(GameObject go)
		{
			go.Scn = this;
			ObjectList.Add(go);
		}
	}
}