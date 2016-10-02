using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC
{
	public abstract class GameObject
	{
		/// <summary>Components of this object</summary>
		protected List<Component.Component> _components;
		/// <summary>Graphical component of this object</summary>
		protected Component.GraphicalComponent _graphComp;
		/// <summary>Path to .h file of this Object</summary>
		public string FilePath { get; protected set; }

		protected GameObject()
		{
			FilePath = null;
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
				result += com.GetCppVariables() + '\n';
			return result + _graphComp.GetCppVariables() + '\n';
		}
		/// <summary>return all components necessary Methods</summary>
		public string GetComponentsMethods()
		{
			string result = "";
			foreach (var com in _components)
				result += com.GetCppMethods() + '\n';
			return result + _graphComp.GetCppMethods() + '\n';
		}
		public string GetComponentsConstructors()
		{
			string result = "";
			foreach (var com in _components)
				result += ", " + com.GetCppConstructor();
			return result + ", " + _graphComp.GetCppConstructor();
		}
		public string GetComponentsConstructorsBody()
		{
			string result = "";
			foreach (var com in _components)
				result += "\n" + com.GetCppConstructorBody();
			return result + "\n" + _graphComp.GetCppConstructorBody();
		}
		public string GetGraphicalComponentOnRender()
		{
			return _graphComp.GetCppOnRender();
		}
	}
}
