using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Glc.Component
{
	public class Script : Component
	{
		public string file;
		public void Validate()
		{
			string onUpdateCode = GetCppOnUpdate();
			if (onUpdateCode.Contains(Glance.NameSetting.AnimationName + ".update"))
				throw new Exception("Animation updated in script");
			if (onUpdateCode.Contains(Glance.NameSetting.AnimatorName + ".update"))
				throw new Exception("Animator updated in script");
		}
		public void CreateFile(string path)
		{
			var fs = System.IO.File.Create(path);
			string str = Glance.templates["B:Script"];
			byte[] arr = System.Text.Encoding.Unicode.GetBytes(str);
			fs.Write(arr, 0, arr.Length);
			fs.Close();
		}
		public Script(string filepath)
		{
			file = filepath;
			_data = new _Data(this);
		}
		internal override string[] GetCppVariables()
		{
			return _data.Variables;
		}
		internal override string GetCppConstructor()
		{
			return "";
		}
		internal override string GetCppConstructorBody()
		{
			return "";
		}
		internal override string GetCppOnUpdate()
		{
			return _data.onUpdate;
		}
		internal override string GetCppOnStart()
		{
			return _data.onStart;
		}
		internal override string[] GetCppMethodsDeclaration()
		{
			return _data.MethodsDeclarations;
		}
		internal override Dictionary<string, string> GetCppMethodsImplementation()
		{
			return _data.MethodsImplementations;
		}
		private _Data _data;

		private class _Data
		{
			public _Data(Script s)
			{
				_isInitializated = false;
				_owner = s;
				MethodsImplementations = new Dictionary<string, string>();
				MethodsImplementations.Add(Glance.NameSetting.ScriptOnStartSignature, "");//costili
				MethodsImplementations.Add(Glance.NameSetting.ScriptOnUpdateSignature, "");//KOCTblJlU
			}
			public string[] Variables
			{
				get
				{
					_Init();
					return _variables;
				}
				private set
				{
					_variables = value;
				}
			}
			public string onUpdate { get; private set; }
			public string onStart { get; private set; }
			public string[] MethodsDeclarations { get; private set; }
			public Dictionary<string, string> MethodsImplementations { get; private set; }

			private string[] _variables;

			private Script _owner;
			private bool _isInitializated;
			private void _Init()
			{
				if (_isInitializated)
					return;
				//
				var strings = File.ReadAllLines(_owner.file);
				bool isInsideVariablesRegion = false;
				bool isInsideMethodsRegion = false;
				bool isInsideMethod = false;
				byte tabs = 0; ;
				List<string> variables = new List<string>();
				string currentMethodSignature = null;

				foreach (var i in strings)
				{
					var str = i.Trim();
					if (str == "")
						continue;
					if (str.Length > 2)
						if (str.Substring(0, 2) == "//")
							continue;
					if (str.Contains(Glance.NameSetting.ScriptVariablesRegionName))
					{
						isInsideVariablesRegion = true;
						isInsideMethodsRegion = false;
						continue;
					}
					if (str.Contains(Glance.NameSetting.ScriptMethodsRegionName))
					{
						isInsideMethodsRegion = true;
						isInsideVariablesRegion = false;
						continue;//any else words in this line are illegal, if you find some in it, please call 911
					}

					if (isInsideVariablesRegion)
						variables.Add(str);

					if (isInsideMethodsRegion)
					{
						if (!isInsideMethod)//method after method, without any shit
						{
							currentMethodSignature = str.Substring(0, str.Length - 1);//cut '{'
							isInsideMethod = true;
							++tabs;
							if (MethodsImplementations.ContainsKey(currentMethodSignature))
								MethodsImplementations[currentMethodSignature] += "";
							else
								MethodsImplementations.Add(currentMethodSignature, "");
							continue;
						}
						if (isInsideMethod)
						{
							if (str.Contains("{"))
								++tabs;
							if (str.Contains("}"))
								--tabs;
							if (tabs == 0)
							{
								isInsideMethod = false;
								continue;
							}
							if (MethodsImplementations.ContainsKey(currentMethodSignature))
								MethodsImplementations[currentMethodSignature] += str + '\n';
							else
								MethodsImplementations.Add(currentMethodSignature, str + '\n');

						}
					}
				}//foreach

				Variables = variables.ToArray();
				onUpdate = MethodsImplementations[Glance.NameSetting.ScriptOnUpdateSignature];
				onStart = MethodsImplementations[Glance.NameSetting.ScriptOnStartSignature];

				MethodsImplementations.Remove(Glance.NameSetting.ScriptOnStartSignature);
				MethodsImplementations.Remove(Glance.NameSetting.ScriptOnUpdateSignature);
				if (MethodsImplementations.ContainsKey(""))
					MethodsImplementations.Remove("");

				List<string> temp = new List<string>();
				foreach (var i in MethodsImplementations)
					temp.Add(i.Key);
				MethodsDeclarations = temp.ToArray();

				_isInitializated = true;
			}//_Init()
		}//_Data
	}//class Script
}
