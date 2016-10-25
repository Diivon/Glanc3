﻿using System;
using System.IO;
using System.Collections.Generic;

namespace Glc
{
	public class PhysicalObject : GameObject
	{
		public Vec2 Pos;
		public PhysicalObject(Vec2 pos) : base()
		{
			Scn = null;
			Pos = pos;
			ClassName = "PhysicalObject" + _count++;
			ObjectName = "Obj" + ClassName;
		}
		static PhysicalObject() { _count = 0; }

		override internal void GenerateCode()
		{
			if (_implementationfilePath == "" || _implementationfilePath == null)
				throw new Exception("File implementation do not exist for " + ClassName + ", when GenerateCode called");
			if (_declarationfilePath == "" || _declarationfilePath == null)
				throw new Exception("File declaration do not exist for " + ClassName + ", when GenerateCode called");
			if (Scn == null)
				throw new Exception("Object " + ClassName + " haven't Scene, when GenerateCode called");
			var impl = File.Open(_implementationfilePath, FileMode.Truncate);
			var decl = File.Open(_declarationfilePath, FileMode.Truncate);
			Glance.CodeGenerator.writePhysicalObject(decl, impl, this);
			impl.Close();
			decl.Close();
		}
		private static uint _count;
		internal override string GetComponentsVariables()
		{
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
			string result = "";
			foreach (var com in Components)
			{
				result += Glance.GatherStringList(new List<string>(com.GetCppMethodsDeclaration()), ";\n") + '\n';
			}
			if (result != "")//if no methods, no need for '\n'
				result += '\n';
			return result;
		}
		internal override string GetComponentsMethodsImplementation()
		{
			string result = "";
			var functions = new Dictionary<string, string>();
			foreach (var i in Components)
				Glance.MergeDictionary(ref functions, i.GetCppMethodsImplementation());
			/*
			foreach (var i in functions)
				Console.WriteLine("~~~~~~~~!" + i.Key + '!' + i.Value + '!');
			*/
			foreach (var i in functions)
				result += Glance.GetRetTypeFromSignature(i.Key) + ' ' + ClassName + "::" + Glance.GetSignatureWithoutRetType(i.Key) + '{' + i.Value + '}' + '\n';
			Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
			Console.WriteLine(result);
			Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
			return result;
		}
		internal override string GetComponentsConstructors()
		{
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