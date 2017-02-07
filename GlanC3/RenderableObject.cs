using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Glc
{
	public class RenderableObject : PhysicalObject
	{
		/// <summary>Graphical components of this object</summary>
		public Component.GraphicalComponent GraphComponent;
		public RenderableObject(Vec2 p) : base(p)
		{
			GraphComponent = null;
			ClassName = "RenderableObject" + _count++;
		}
		static RenderableObject() { _count = 0; }

		private static uint _count;
		internal override void GenerateCode()
		{
			if (_implementationfilePath == "" || _implementationfilePath == null)
				throw new Exception("File implementation do not exist for " + ClassName + ", when GenerateCode called");
			if (_declarationfilePath == "" || _declarationfilePath == null)
				throw new Exception("File declaration do not exist for " + ClassName + ", when GenerateCode called");
			if (_scene == null)
				throw new Exception("Object " + ClassName + " haven't Scene, when GenerateCode called");
			if (_layer == null)
				throw new Exception("Object " + ClassName + " haven't Layer, when GenerateCode called");
			var impl = File.Open(_implementationfilePath, FileMode.Truncate);
			var decl = File.Open(_declarationfilePath, FileMode.Truncate);
			Glance.CodeGenerator.writeRenderableObject(decl, impl, this);
			impl.Close();
			decl.Close();
		}
		internal override string GetComponentsVariables()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in Components)
				result += Glance.GatherStringList(com.GetCppVariables(), ";\n");
			result += Glance.GatherStringList(GraphComponent.GetCppVariables(), ";\n");
			if (result != "")//if no variables, no need for '\n'
				result += '\n';
			return result;
		}
		internal override string GetComponentsMethodsDeclaration()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			List<string> result = new List<string>();
			foreach (var i in Components)
				result.AddRange(i.GetCppMethodsDeclaration());
			result.AddRange(GraphComponent.GetCppMethodsDeclaration());
			result = result.Distinct().ToList();
			return Glance.GatherStringList(result, ";\n");
		}
		internal override string GetComponentsMethodsImplementation()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			var functions = new Dictionary<string, string>();
			foreach (var i in Components)
				Glance.MergeDictionary(ref functions, i.GetCppMethodsImplementation());
			Glance.MergeDictionary(ref functions, GraphComponent.GetCppMethodsImplementation());
			foreach (var i in functions)
				if (i.Key != "") 
					result += Glance.GetRetTypeFromSignature(i.Key) + ' ' + ClassName + "::" + Glance.GetSignatureWithoutRetType(i.Key) + '{' + i.Value + '}' + '\n';
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
			if (GraphComponent.GetCppConstructor() != "")
				result += ", " + GraphComponent.GetCppConstructor();
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
			result += GraphComponent.GetCppConstructorBody();
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
			result += GraphComponent.GetCppOnUpdate();
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
			result += GraphComponent.GetCppOnStart();
			if (result == "")
				result += '\n';
			return result;
		}
		internal virtual string GetComponentsOnRender()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("Graphical Component for Object " + ClassName + "is empty");
			return GraphComponent.GetCppOnRender() + '\n';
		}
	}
}
