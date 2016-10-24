using System;
using System.Collections.Generic;

namespace Glc
{
	class RenderableObject : PhysicalObject
	{
		/// <summary>Graphical components of this object</summary>
		public Component.GraphicalComponent GraphComponent;
		public RenderableObject(Vec2 p) : base(p)
		{
			GraphComponent = null;
		}

		internal override string GetComponentsVariables()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
			{
				result += com.GetCppVariables() + '\n';
			}
			if (result != "")//if no variables, no need for '\n'
				result += '\n';
			return result;
		}
		internal override string GetComponentsMethodsDeclaration()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
			{
				result += Glance.GatherStringList(new List<string>(com.GetCppMethodsDeclaration()), ';') + '\n';
			}
			if (result != "")//if no methods, no need for '\n'
				result += '\n';
			return result;
		}
		internal override string GetComponentsMethodsImplementation()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppConstructor() == "")
					continue;
				result += ", " + com.GetCppConstructor();
			}
			return result;
		}
		internal override string GetComponentsConstructors()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppConstructor() == "")
					continue;
				result += ", " + com.GetCppConstructor();
			}
			return result;
		}
		internal override string GetComponentsConstructorsBody()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppConstructorBody() == "")
					continue;
				result += '\n' + com.GetCppConstructorBody();
			}
			if (result != "")
				result += '\n';
			return result;
		}
		internal override string GetComponentsOnRender()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppOnRender() == "")
					continue;
				result += "\n" + com.GetCppOnRender();
			}
			if (result != "")
				result += '\n';
			return result;
		}
		internal override string GetComponentsOnUpdate()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppOnUpdate() == "")
					continue;
				result += "\n" + com.GetCppOnUpdate();
			}
			if (result == "")
				result += '\n';
			return result;
		}
		internal override string GetComponentsOnStart()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
			{
				if (com.GetCppOnStart() == "")
					continue;
				result += "\n" + com.GetCppOnStart();
			}
			if (result == "")
				result += '\n';
			return result;
		}
	}
}
