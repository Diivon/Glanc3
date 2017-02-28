using System;
using System.Collections.Generic;

namespace Glc.Component
{
	public class Collider : Component
	{
		public enum Type
		{
			Rectangle,
			Circle
		}
		public Vec2 pos;
		public Vec2 size;
		public Type type;
		public float radius;

		public Collider(Type t, Vec2 p, Vec2 s)
		{
			pos = p;
			size = s;
			type = t;
		}
		internal override string GetCppConstructor()
		{
			switch (type)
			{
				case Type.Rectangle:
				return Glance.templates["Com:Collider:Constructor"]
								.Replace("#ColliderName#", Glance.NameSetting.ColliderName)
								.Replace("#Pos#", pos.GetCppCtor())
								.Replace("#Size#", size.GetCppCtor())
					;
				case Type.Circle:
					return Glance.templates["Com:Collider:Constructor"]
								.Replace("#ColliderName#", Glance.NameSetting.ColliderName)
								.Replace("#Pos#", pos.GetCppCtor())
								.Replace("#Size#", Glance.floatToString(radius))
					;
				default:
					throw new Exception("Glc.Component.Collider.GetCppConstructor has been hacked, param type is: " + type.ToString());
			}
		}
		internal override string GetCppConstructorBody()
		{
			return Glance.templates["Com:Collider:ConstructorBody"];
		}
		internal override string[] GetCppMethodsDeclaration()
		{
			return Glance.templates["Com:Collider:Methods"].Split(';');
		}
		internal override string[] GetCppVariables()
		{
			return Glance.templates["Com:Collider:Vars"]
								.Replace("#ColliderType#", TypeToCppType(type))
								.Replace("#ColliderTypeName#", Glance.NameSetting.ColliderType)
								.Replace("#ColliderName#", Glance.NameSetting.ColliderName)
				.Split(';');
		}
		internal override Dictionary<string, string> GetCppMethodsImplementation()
		{
			return new Dictionary<string, string>();
		}
		internal override string GetCppOnStart()
		{
			return Glance.templates["Com:Collider:OnStart"];
		}
		internal override string GetCppOnUpdate()
		{
			return Glance.templates["Com:Collider:OnUpdate"];
		}
		internal string TypeToCppType(Type t)
		{
			switch (t)
			{
				case Type.Circle:
					return "::gc::ColliderType::Circle";
				case Type.Rectangle:
					return "::gc::ColliderType::Rectangle";
				default:
					throw new Exception("Glc.Component.Collider.TypeToCppType() has been hacked, param type is: " + t.ToString());
			}
		}
	}
}
