using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Glc
{
	class Layer
	{
		public Layer()
		{
			_className = "Layer" + _count;
			_objectName = "layerObject" + _count;
			_implementationFilePath = null;
			_declarationFilePath = null;
		}
		public void AddObject(GameObject o)
		{
			_objects.Add(o);
			o._layer = this;
			o._scene = _scene;
		}
		public void AddScript(Component.Script s)
		{

		}
		public string ClassName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_className = value;
				else throw new ArgumentException("Name must contain only letters, numbers and cymbol '_'");
			}
			get{return _className;}
		}
		/// <summary>Path to .h file of this object</summary>
		public string ImplementationFilePath
		{
			get { return _implementationFilePath; }
			set
			{
				if (File.Exists(value))
					_implementationFilePath = value;
				else throw new ArgumentException("invalid file path");
			}
		}
		public string DeclarationFilePath
		{
			get { return _declarationFilePath; }
			set
			{
				if (File.Exists(value))
					_declarationFilePath = value;
				else throw new ArgumentException("invalid file path");
			}
		}
		internal string ObjectName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_objectName = value;
			}
			get { return _objectName; }
		}
		/// <summary>Generate .h and .cpp files for this object</summary>
		internal void GenerateCode()
		{
			if (this._implementationFilePath == "" || _implementationFilePath == null)
				throw new Exception("File implementation do not exist for " + ClassName + ", when GenerateCode called");
			if (_declarationFilePath == "" || _declarationFilePath == null)
				throw new Exception("File declaration do not exist for " + ClassName + ", when GenerateCode called");
			if (_scene == null)
				throw new Exception("Layer " + ClassName + " haven't Scene, when GenerateCode called");
			var impl = File.Open(_implementationFilePath, FileMode.Truncate);
			var decl = File.Open(_declarationFilePath, FileMode.Truncate);
			Glance.CodeGenerator.writeLayer(decl, impl, this);
			impl.Close();
			decl.Close();
		}
		/// <summary>return all components necessary Variables</summary>
		internal string GetVariables()
		{
			string result = "";
			foreach (var i in _scripts)
				result += Glance.GatherStringList(i.GetCppVariables(), ";\n");
			if (result != "")
				result += '\n';
			return result;
		}
		/// <summary>return all components necessary Methods</summary>
		internal string GetMethodsDeclaration()
		{
			List<string> result = new List<string>();
			foreach (var i in _scripts)
				result.AddRange(i.GetCppMethodsDeclaration());
			result = result.Distinct().ToList();
			return Glance.GatherStringList(result, ";\n");
		}
		/// <summary>return all components necessary Methods declaration</summary>
		internal string GetMethodsImplementation()
		{
			string result = "";
			var functions = new Dictionary<string, string>();
			foreach (var i in _scripts)
				Glance.MergeDictionary(ref functions, i.GetCppMethodsImplementation());
			foreach (var i in functions)
				if (i.Key != "")
					result += Glance.GetRetTypeFromSignature(i.Key) + ' ' + ClassName + "::" + Glance.GetSignatureWithoutRetType(i.Key) + '{' + i.Value + '}' + '\n';
			return result;
		}
		/// <summary>return all components necessary Constructors</summary>
		internal string GetConstructor();
		/// <summary>return all components necessary Constructor code</summary>
		internal string GetConstructorBody()
		{
			return "";
		}
		/// <summary>return all components necessary OnUpdate code</summary>
		internal string GetOnUpdate()
		{
			string result = "";
			foreach (var i in _scripts)
			{
				string temp = i.GetCppOnUpdate();
				if (temp != "")
					result += '\n' + i.GetCppOnUpdate();
			}
			return result;
		}
		/// <summary>return all components necessary OnStart code</summary>
		internal string GetOnStart()
		{
			string result = "";
			foreach (var i in _scripts)
			{
				string temp = i.GetCppOnStart();
				if (temp != "")
					result += '\n' + i.GetCppOnStart();
			}
			return result;
		}

		/// <summary>All GameObjects in this layer</summary>
		protected List<GameObject> _objects;
		protected List<Component.Script> _scripts;
		protected string _className;
		protected string _objectName;
		protected string _implementationFilePath;
		protected string _declarationFilePath;
		protected Scene _scene;
		private static uint _count;
		static Layer() { _count = 0; }
	}
}
