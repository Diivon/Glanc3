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
		/// <summary>Scene, where this object is</summary>

		/// <summary>Components of this object</summary>
		public List<Component.Component> Components;
		/// <summary>Graphical components of this object</summary>
		public Component.GraphicalComponent GraphComponent;
		/// <summary>Path to .h file of this Object</summary>
		/// <remarks>Не хакай, плз</remarks>
		public string ImplementationFilePath
		{
			get { return _implementationfilePath; }
			set
			{
				if (File.Exists(value))
					_implementationfilePath = value;
				else throw new Exception("invalid file path");
			}
		}
		public string DeclarationFilePath
		{
			get { return _declarationfilePath; }
			set
			{
				if (File.Exists(value))
					_declarationfilePath = value;
				else throw new Exception("invalid file path");
			}
		}

		internal string ObjectName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_objectName = value;
				else throw new ArgumentException("Name must contain only letters, numbers and cymbol '_'");
			}
			get { return _objectName; }
		}
		internal Scene Scn;
		protected string _objectName;
		protected string _className;
		protected string _implementationfilePath;
		protected string _declarationfilePath;

		protected GameObject()
		{
			_implementationfilePath = null;
			_declarationfilePath = null;
			Components = new List<Component.Component>();
			GraphComponent = null;
		}
		/// <summary>Generate .h and .cpp files for this object</summary>
		internal abstract void GenerateCode();
		/// <summary>return all components necessary Variables</summary>
		internal string GetComponentsVariables()
		{
			if (GraphComponent == null)
				throw new Exception("Object " + ObjectName + " hasn't graphical component");
			string result = "";
			foreach (var com in Components)
			{
				result += com.GetCppVariables() +'\n';
			}
			return result + GraphComponent.GetCppVariables() + '\n';
		}
		/// <summary>return all components necessary Methods</summary>
		internal string GetComponentsMethods()
		{
			if (GraphComponent == null)
				throw new Exception("Object " + ObjectName + " hasn't graphical component");
			string result = "";
			foreach (var com in Components)
			{
				result += com.GetCppMethodsDeclaration() + '\n';
			}
			return result + GraphComponent.GetCppMethodsDeclaration() + '\n';
		}
		/// <summary>return all components necessary Constructors</summary>
		internal string GetComponentsConstructors()
		{
			if (GraphComponent == null)
				throw new Exception("Object " + ObjectName + " hasn't graphical component");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppConstructor() == "")
					continue;
				result += ", " + com.GetCppConstructor();
			}
			return result + ", " + GraphComponent.GetCppConstructor();
		}
		/// <summary>return all components necessary Constructor code</summary>
		internal string GetComponentsConstructorsBody()
		{
			if (GraphComponent == null)
				throw new Exception("Object " + ObjectName + " hasn't graphical component");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppConstructorBody() == "")
					continue;
				result += "\n" + com.GetCppConstructorBody();
			}
			return result + "\n" + GraphComponent.GetCppConstructorBody();
		}
		/// <summary>return all components necessary OnRender code</summary>
		internal string GetComponentsOnRender()
		{
			if (GraphComponent == null)
				throw new Exception("Object " + ObjectName + " hasn't graphical component");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppOnRender() == "")
					continue;
				result += "\n" + com.GetCppOnRender();
			}
			return result + "\n" + GraphComponent.GetCppOnRender();
		}
		/// <summary>return all components necessary OnUpdate code</summary>
		internal string GetComponentsOnUpdate()
		{
			if (GraphComponent == null)
				throw new Exception("Object " + ObjectName + " hasn't graphical component");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppOnUpdate() == "")
					continue;
				result += "\n" + com.GetCppOnUpdate();
			}
			return result + "\n" + GraphComponent.GetCppOnUpdate();
		}
		/// <summary>return all components necessary OnStart code</summary>
		internal string GetComponentsOnStart()
		{
			if (GraphComponent == null)
				throw new Exception("Object " + ObjectName + " hasn't graphical component");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppOnStart() == "")
					continue;
				result += "\n" + com.GetCppOnStart();
			}
			return result + "\n" + GraphComponent.GetCppOnStart();
		}
	}
}
