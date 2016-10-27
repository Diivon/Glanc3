using System;
using System.Collections.Generic;
using System.IO;

namespace Glc
{
	namespace Component
	{
		public enum AnimationType
		{
			Single,
			Cyclic,
			PingPong
		}

		public abstract class Component
		{
			/// <summary>return defined in script variables
			/// one variable per index</summary>
			/// <returns>return variables without semi-colon at the end</returns>
			internal abstract string[] GetCppVariables();
			/// <summary>return defined in component methods</summary>
			/// <returns>return method declaration without semi-colon at the end</returns>
			internal abstract string[] GetCppMethodsDeclaration();
			/// <summary>(declaration, implementation)</summary>
			internal abstract Dictionary<string, string> GetCppMethodsImplementation();

			/// <summary>return constructors for component variables
			/// as Initializaton List(which goes after colon)</summary>
			internal abstract string GetCppConstructor();
			/// <summary>return body for constructor</summary>
			internal abstract string GetCppConstructorBody();
			/// <summary>return code, that must be in onUpdate()</summary>
			internal abstract string GetCppOnUpdate();
			/// <summary>return code, that must be in onRender()</summary>
			internal abstract string GetCppOnRender();
			/// <summary>return code, that must be in onStart()</summary>
			internal abstract string GetCppOnStart();
		}
		public abstract class GraphicalComponent: Component
		{
			internal static string _AnimationTypeToString(AnimationType t)
			{
				switch (t)
				{
					case AnimationType.Single:
						return "::gc::AnimationType::Single";
					case AnimationType.Cyclic:
						return "::gc::AnimationType::Cyclic";
					case AnimationType.PingPong:
						return "::gc::AnimationType::PingPong";
					default:
						return "an error was occured in _AnimationTypeToString()";
				}
			}
			public class StaticSprite : GraphicalComponent
			{
				string FileName;
				/// <summary></summary>
				/// <param name="fn">File name of picture</param>
				public StaticSprite(string fn)
				{
					FileName = fn;
				}
				internal override string[] GetCppVariables()
				{
					return Glance.templates["Com:StaticSprite:Vars"].Split(';');
				}
				internal override string[] GetCppMethodsDeclaration()
				{
					return Glance.templates["Com:StaticSprite:Methods"].Split(';');
				}
				internal override Dictionary<string, string> GetCppMethodsImplementation()
				{
					return new Dictionary<string, string>();
				}
				internal override string GetCppConstructor()
				{
					return Glance.templates["Com:StaticSprite:Constructor"];
				}
				internal override string GetCppConstructorBody()
				{
					return Glance.templates["Com:StaticSprite:ConstructorBody"].Replace("#FileName#", Glance.ToCppString(FileName));
				}
				internal override string GetCppOnUpdate()
				{
					return Glance.templates["Com:StaticSprite:OnUpdate"];
				}
				internal override string GetCppOnRender()
				{
					return Glance.templates["Com:StaticSprite:OnRender"];
				}
				internal override string GetCppOnStart()
				{
					return Glance.templates["Com:StaticSprite:OnUpdate"];
				}
			}
			public class Animation : GraphicalComponent
			{
				AnimationType _AnimationType;
				List<SpriteFrame> Frames;
				
				/// <summary>return processed string for GetCpp... family</summary>
				public Animation(AnimationType t)
				{
					_AnimationType = t;
					Frames = new List<SpriteFrame>();
				}
				public void AddFrame(SpriteFrame sf)
				{
					Frames.Add(sf);
				}
				public void AddFrame(string path, float dur)
				{
					Frames.Add(new SpriteFrame(path, dur));
				}
				internal override string[] GetCppVariables()
				{
					return _GetProcessed(Glance.templates["Com:Animation:Vars"]).Split(';');
				}
				internal override string[] GetCppMethodsDeclaration()
				{
					return _GetProcessed(Glance.templates["Com:Animation:Methods"]).Split(';');
				}
				internal override Dictionary<string, string> GetCppMethodsImplementation()
				{
					return new Dictionary<string, string>();
				}
				internal override string GetCppConstructor()
				{
					return _GetProcessed(Glance.templates["Com:Animation:Constructor"]);
				}
				internal override string GetCppConstructorBody()
				{
					string code = "";
					foreach (var i in Frames)
						code += Glance.NameSetting.AnimationName + ".emplaceFrame(" + Glance.ToCppString(i.PicName) + ", " + i.Duration.ToString("0.00").Replace(',', '.') + "f);\n"; 
					return _GetProcessed(Glance.templates["Com:Animation:ConstructorBody"].Replace("#SpriteFrames#", code));
				}
				internal override string GetCppOnUpdate()
				{
					return _GetProcessed(Glance.templates["Com:Animation:OnUpdate"]);
				}
				internal override string GetCppOnRender()
				{
					return _GetProcessed(Glance.templates["Com:Animation:OnRender"]);
				}
				internal override string GetCppOnStart()
				{
					return _GetProcessed(Glance.templates["Com:Animation:OnStart"]);
				}
				private string _GetProcessed(string a)
				{
					return a
							.Replace("#AnimationType#", _AnimationTypeToString(_AnimationType))
							.Replace("#AnimationTypeName#", Glance.NameSetting.AnimationType)
							.Replace("#AnimationName#", Glance.NameSetting.AnimationName)
							;
				}
			}
			public class Animator
			{
				//TODO : class Animator
			}
		}
		public class Script: Component
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
			internal override string GetCppOnRender()
			{
				return ""; 
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
				public string onUpdate { get;	private set; }
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
						if(str.Length > 2)
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
							if(!isInsideMethod)//method after method, without any shit
							{
								currentMethodSignature = str.Substring(0, str.Length - 1);//cut '{'
								isInsideMethod = true;
								++tabs;
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
								{
									MethodsImplementations[currentMethodSignature] += str;
								}
								else
									MethodsImplementations.Add(currentMethodSignature, str);
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
	}//ns Component
}//ns Glc