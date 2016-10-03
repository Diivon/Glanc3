﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Glc
{
	public abstract class GameObject
	{
		public string ClassName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					ClassName = value;
			}
			get { return ClassName; }
		}
		public string ObjectName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					ObjectName = value;
			}
			get { return ObjectName; }
		}
		public bool IsRenderableAtStart;

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
		/// <summary>return all components necessary Constructors</summary>
		public string GetComponentsConstructors()
		{
			string result = "";
			foreach (var com in _components)
				result += ", " + com.GetCppConstructor();
			return result + ", " + _graphComp.GetCppConstructor();
		}
		/// <summary>return all components necessary Constructor code</summary>
		public string GetComponentsConstructorsBody()
		{
			string result = "";
			foreach (var com in _components)
				result += "\n" + com.GetCppConstructorBody();
			return result + "\n" + _graphComp.GetCppConstructorBody();
		}
		/// <summary>return all components necessary OnRender code</summary>
		public string GetComponentsOnRender()
		{
			string result = "";
			foreach (var com in _components)
				result += "\n" + com.GetCppOnRender();
			return result + "\n" + _graphComp.GetCppOnRender();
		}
		/// <summary>return all components necessary OnUpdate code</summary>
		public string GetComponentsOnUpdate()
		{
			string result = "";
			foreach (var com in _components)
				result += "\n" + com.GetCppOnUpdate();
			return result + "\n" + _graphComp.GetCppOnUpdate();
		}
		/// <summary>return all components necessary OnStart code</summary>
		public string GetComponentsOnStart()
		{
			string result = "";
			foreach (var com in _components)
				result += "\n" + com.GetCppOnStart();
			return result + "\n" + _graphComp.GetCppOnStart();
		}
	}
}
