using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			internal abstract string GetCppVariables();
			internal abstract string GetCppMethods();
			internal abstract string GetCppConstructor();
			internal abstract string GetCppConstructorBody();
			internal abstract string GetCppOnUpdate();
			internal abstract string GetCppOnRender();
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
				internal override string GetCppMethods()
				{
					return Glance.templates["Com:StaticSprite:Methods"];
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
				internal override string GetCppMethods()
				{
					return _GetProcessed(Glance.templates["Com:Animation:Methods"]);
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
				//:)
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
			internal override string GetCppMethods()
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
			///<summary>if script is invalid throw exception. that will explain problem</summary>
			/// <summary>Create script, linked to a file</summary>
			///<summary>do not return signature </summary>
			private string _GetMethodBody(string sign)
			{
				//ochen pizdec, sorry
				///cymbols to trim:)
				char[] cymbolsToTrim = { ' ', '\t' };
				///all strings from script file
				string[] Data = System.IO.File.ReadAllLines(file);
				///tabs count on moment, when searched method was found
				byte methodTabs = 0;
				///current tabs count
				byte fileTabs = 0;
				///...
				string result = "";
				///if we currently inside searched method body
				bool isInsideMethod = false;

				foreach (var i in Data)
				{
					var str = i.Trim(cymbolsToTrim);//ready string for processing
					if (str == "" || str == null) continue;//if nothing, skip

					if (str.Contains('{'))
						++fileTabs;
					if (str.Contains('}'))
						--fileTabs;

					if (str == sign)
					{
						isInsideMethod = true;
						methodTabs = fileTabs;
						continue;
					}
					if(str == (sign + '{'))
					{
						isInsideMethod = true;
						methodTabs = (byte)(fileTabs - 1);
						continue;
					}
					if (isInsideMethod)
					{
						if (fileTabs == methodTabs)//if we at the end of method
						{
							isInsideMethod = false;
						}
						else
							result += str;
					}
				}
				return result;
			}
		}
	}
}