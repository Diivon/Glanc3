using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace Glc
{
	public abstract class GameObject
	{
		protected string _className;
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
		protected string _objectName;
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
		public bool IsRenderableAtStart;
		protected string _filePath;

		/// <summary>Components of this object</summary>
		protected List<Component.Component> _components;
		/// <summary>Graphical component of this object</summary>
		protected Component.GraphicalComponent _graphComp;
		/// <summary>Path to .h file of this Object</summary>
		/// <remarks>Не хакай, плз</remarks>
		public string FilePath
		{
			get { return _filePath; }
			set
			{
				if (File.Exists(value))
					_filePath = value;
				else throw new Exception("invalid file path");
			}
		}

		protected GameObject()
		{
			_filePath = null;
			_components = new List<Component.Component>();
		}
		/// <summary>Generate .h file for this object</summary>
		public abstract void GenerateFile();
		/// <summary>Only one graphical component exist for one object</summary>
		public void SetGraphicalComponent(Component.GraphicalComponent c)
		{
			_graphComp = c;
		}
		/// <summary>Add component to this object</summary>
		public void AddComponent(Component.Component c)
		{
			if (c is Component.GraphicalComponent)
				throw new ArgumentException("Must not to give AddComponent() GraphicalComponent, use SetGraphicalComponent() instead");
			_components.Add(c);
		}
		/// <summary>return all components necessary Variables</summary>
		public string GetComponentsVariables()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppVariables() == "")
					continue;
				result += com.GetCppVariables() +'\n';
			}
			return result + _graphComp.GetCppVariables() + '\n';
		}
		/// <summary>return all components necessary Methods</summary>
		public string GetComponentsMethods()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppMethods() == "")
					continue;
				result += com.GetCppMethods() + '\n';
			}
			return result + _graphComp.GetCppMethods() + '\n';
		}
		/// <summary>return all components necessary Constructors</summary>
		public string GetComponentsConstructors()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppConstructor() == "")
					continue;
				result += ", " + com.GetCppConstructor();
			}
			return result + ", " + _graphComp.GetCppConstructor();
		}
		/// <summary>return all components necessary Constructor code</summary>
		public string GetComponentsConstructorsBody()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppConstructorBody() == "")
					continue;
				result += "\n" + com.GetCppConstructorBody();
			}
			return result + "\n" + _graphComp.GetCppConstructorBody();
		}
		/// <summary>return all components necessary OnRender code</summary>
		public string GetComponentsOnRender()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppOnRender() == "")
					continue;
				result += "\n" + com.GetCppOnRender();
			}
			return result + "\n" + _graphComp.GetCppOnRender();
		}
		/// <summary>return all components necessary OnUpdate code</summary>
		public string GetComponentsOnUpdate()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppOnUpdate() == "")
					continue;
				result += "\n" + com.GetCppOnUpdate();
			}
			return result + "\n" + _graphComp.GetCppOnUpdate();
		}
		/// <summary>return all components necessary OnStart code</summary>
		public string GetComponentsOnStart()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppOnStart() == "")
					continue;
				result += "\n" + com.GetCppOnStart();
			}
			return result + "\n" + _graphComp.GetCppOnStart();
		}
	}
}
