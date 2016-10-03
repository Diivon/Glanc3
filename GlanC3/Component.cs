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
			public abstract string GetCppVariables();
			public abstract string GetCppMethods();
			public abstract string GetCppConstructor();
			public abstract string GetCppConstructorBody();
			public abstract string GetCppOnUpdate();
			public abstract string GetCppOnRender();
			public abstract string GetCppOnStart();
		}
		public abstract class GraphicalComponent: Component
		{
			public static string _AnimationTypeToString(AnimationType t)
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
				public StaticSprite(string fn)
				{
					FileName = fn;
				}
				public override string GetCppVariables()
				{
					return Glance.templates["Com:StaticSprite:Vars"];
				}
				public override string GetCppMethods()
				{
					return Glance.templates["Com:StaticSprite:Methods"];
				}
				public override string GetCppConstructor()
				{
					return Glance.templates["Com:StaticSprite:Constructor"];
				}
				public override string GetCppConstructorBody()
				{
					return Glance.templates["Com:StaticSprite:ConstructorBody"].Replace("#FileName#", Glance.ToCppString(FileName));
				}
				public override string GetCppOnUpdate()
				{
					return Glance.templates["Com:StaticSprite:OnUpdate"];
				}
				public override string GetCppOnRender()
				{
					return Glance.templates["Com:StaticSprite:OnRender"];
				}
				public override string GetCppOnStart()
				{
					return Glance.templates["Com:StaticSprite:OnUpdate"];
				}
			}
			public class Animation : GraphicalComponent
			{
				AnimationType _AnimationType;

				public Animation(AnimationType t)
				{
					_AnimationType = t;
				}
				public override string GetCppVariables()
				{
					return Glance.templates["Com:Animation:Vars"].Replace("#AnimationTypeName#", _AnimationTypeToString(_AnimationType));
				}
				public override string GetCppMethods()
				{
					return Glance.templates["Com:Animation:Methods"].Replace("#AnimationTypeName#", _AnimationTypeToString(_AnimationType));
				}
				public override string GetCppConstructor()
				{
					return Glance.templates["Com:Animation:Constructor"];
				}
				public override string GetCppConstructorBody()
				{
					return Glance.templates["Com:Animation:ConstructorBody"];
				}
				public override string GetCppOnUpdate()
				{
					return Glance.templates["Com:Animation:OnUpdate"];
				}
				public override string GetCppOnRender()
				{
					return Glance.templates["Com:Animation:OnRender"];
				}
				public override string GetCppOnStart()
				{
					return Glance.templates["Com:Animation:OnStart"];
				}
			}
		}
		public class Script: Component
		{
			public string[] Data;
			private string _GetMethod(string sign)
			{
				char[] cymbolsToTrim = { ' ', '\t' };
				byte tabs = 0;
				string result = "";
				bool isInsideMethod = false;
				foreach (var i in Data)
				{
					var str = i.Trim(cymbolsToTrim);
					if (str == "" || str == null) continue;
					if (str == sign)
					{
						result += str;
						isInsideMethod = true;
						continue;
					}
					if (str == (sign + '{'))
						isInsideMethod = true;
					if (isInsideMethod)
					{
						if (str[str.Length - 1] == '{')
							++tabs;
						if (str[0] == '}')
							--tabs;
						if (tabs == 0)
							isInsideMethod = false;
						result += str;
					}
				}
				return result;
			}
			public override string GetCppVariables()
			{
				return "";
			}
			public override string GetCppConstructor()
			{
				return "";
			}
			public override string GetCppConstructorBody()
			{
				return "";
			}
			public override string GetCppMethods()
			{
				//here all logic
				return "static_cast(false);";
			}
			public override string GetCppOnUpdate()
			{
				return _GetMethod("void onUpdate(const float & dt)"); 
			}
			public override string GetCppOnRender()
			{
				return _GetMethod("const sf::Sprite & onRender(::gc::Camera & cam)"); 
			}
			public override string GetCppOnStart()
			{
				return _GetMethod("void onStart()");
			}
			public bool Validate()
			{
				return true;
			}
		}
	}
}