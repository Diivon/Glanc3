using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC
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
			public abstract string GetCppOnStart();
			public abstract string GetCppOnRender();
			public abstract string GetCppOnUpdate();
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
			public abstract string GetCppOnRender();
			public class StaticSprite : GraphicalComponent
			{
				string FileName;
				public StaticSprite(string fn)
				{
					FileName = fn;
				}
				public override string GetCppVariables()
				{
					return Glance.presets["Com:StaticSprite:Vars"];
				}
				public override string GetCppMethods()
				{
					return Glance.presets["Com:StaticSprite:Methods"];
				}
				public override string GetCppConstructor()
				{
					return Glance.presets["Com:StaticSprite:Constructor"];
				}
				public override string GetCppConstructorBody()
				{
					return Glance.presets["Com:StaticSprite:ConstructorBody"].Replace("#FileName#", Glance.ToCppString(FileName)); 
				}
				public override string GetCppOnRender()
				{
					return Glance.presets["Com:StaticSprite:OnRender"];
				}
			}
			public class Animation : GraphicalComponent//не работает :(((
			{
				AnimationType _AnimationType;

				public Animation(AnimationType t)
				{
					_AnimationType = t;
				}
				public override string GetCppVariables()
				{
					return Glance.presets["Com:Animation:Vars"].Replace("#AnimationTypeName#", _AnimationTypeToString(_AnimationType));
				}
				public override string GetCppMethods()
				{
					return Glance.presets["Com:Animation:Methods"].Replace("#AnimationTypeName#", _AnimationTypeToString(_AnimationType));
				}
				public override string GetCppConstructor()
				{
					return Glance.presets["Com:Animation:Constructor"];
				}
				public override string GetCppOnRender()
				{
					return Glance.presets["Com:Animation:OnRender"];
				}
				public override string GetCppConstructorBody()
				{
					return Glance.presets["Com:Animation:ConstructorBody"];
				}
			}
		}
		public class SoundSource : Component
		{
			public SoundSource(){}
			public override string GetCppVariables()
			{
				return Glance.presets["Com:SoundSource:Vars"];
			}
			public override string GetCppMethods()
			{
				return Glance.presets["Com:SoundSource:Methods"];
			}
			public override string GetCppConstructor()
			{
				return Glance.presets["Com:SoundSource:Constructor"];
			}
			public override string GetCppConstructorBody()
			{
				return Glance.presets["Com:SoundSource:ConstructorBody"];
			}
		}
	}
}