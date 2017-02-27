using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Glc
{
	public class Scene
	{
		public Scene()
		{
			ClassName = "Scene" + _count++;
			ObjectName = "Obj" + ClassName;
			LayerList = new List<Layer>();
		}
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
		public void AddLayer(Layer go)
		{
			go.SetScene(this);
			LayerList.Add(go);
		}
		public string GetDeclarationFileName()
		{
			return ClassName + ".h";
		}
		private string _className;
		private string _objectName;
		private static int _count;
		static Scene() { _count = 0; }
		internal string GetAllLayersOnStart()
		{
			string result = "";
			foreach (var i in LayerList)
				result += i.ObjectName + ".onStart();\n";
			return result;
		}
		internal string GetAllLayersOnUpdate()
		{
			string result = "";
			foreach (var i in LayerList)
				result += i.ObjectName + ".onUpdate(dt);\n";
			return result;
		}
		internal List<Layer> LayerList;
	}
}