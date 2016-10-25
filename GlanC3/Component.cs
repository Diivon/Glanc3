using System;
using System.Collections.Generic;
using System.Linq;

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
			internal abstract string GetCppVariables();
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
				internal override string GetCppVariables()
				{
					return Glance.templates["Com:StaticSprite:Vars"];
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
				internal override string GetCppVariables()
				{
					return _GetProcessed(Glance.templates["Com:Animation:Vars"]);
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
			public Script(string filepath)
			{
				file = filepath;
			}
			internal override string GetCppVariables()
			{
				//TODO: variables parsing
				return "";
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
				return _GetMethodBody("void onUpdate(const float & dt)"); 
			}
			internal override string GetCppOnRender()
			{
				return _GetMethodBody("const sf::Sprite & onRender(::gc::Camera & cam)"); 
			}
			internal override string GetCppOnStart()
			{
				return _GetMethodBody("void onStart()");
			}
			internal override string[] GetCppMethodsDeclaration()
			{
				return new string[]{ "void On()", "void Off()" };
			}
			internal override Dictionary<string, string> GetCppMethodsImplementation()
			{
				Dictionary<string, string> dict = new Dictionary<string, string>();
				dict.Add("void On()", "std::cout << \"On()\" << std::endl;");
				dict.Add("void Off()", "std::cout << \"Off()\" << std::endl;");
				return dict;
			}
			private string _GetMethodBody(string sign)
			{
				//TODO: DO
			}
		}
	}
}