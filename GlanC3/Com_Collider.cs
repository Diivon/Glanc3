﻿using System;
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
		public Collider(Type t)
		{
			type = t;
		}

		public Vec2 pos;
		public Vec2 size;
		public Type type;
		public float radius;

		public Collider SetPosition(Vec2 p) { pos = p;  return this; }
		public Collider SetSize(Vec2 s) { if (type == Type.Circle) throw new Exception("An attempt to SetSize to Circle Collider was found!"); size = s; return this; }
		public Collider SetRadius(float r) { if (type == Type.Rectangle) throw new Exception("An attempt to SetRadius to Rectangle Collider was found!"); radius = r;  return this; }

		internal override string[] GetCppConstructor()
		{
			switch (type)
			{
				case Type.Rectangle:
				return Glance.templates["Com:Collider:Constructor"]
								.Replace("#ColliderName#", Glance.NameSetting.ColliderName)
								.Replace("#Pos#", pos.GetCppCtor())
								.Replace("#Size#", size.GetCppCtor())
								.Split(',').gForEach(x => x.Trim())
					;
				case Type.Circle:
					return Glance.templates["Com:Collider:Constructor"]
								.Replace("#ColliderName#", Glance.NameSetting.ColliderName)
								.Replace("#Pos#", pos.GetCppCtor())
								.Replace("#Size#", Glance.floatToString(radius))
								.Split(',').gForEach(x => x.Trim())
					;
				default:
					throw new Exception("Glc.Component.Collider.GetCppConstructor has been hacked, param type is: " + type.ToString());
			}
		}
		internal override string GetCppConstructorBody()
		{
			return Glance.templates["Com:Collider:ConstructorBody"];
		}
		internal override Dictionary<Glance.FieldsAccessType, string[]> GetCppMethodsDeclaration()
		{
            var result = new Dictionary<Glance.FieldsAccessType, string[]>();
            var methods = Glance.templates["Com:Collider:Methods"].Split(';').gForEach(x => x.Trim());
            result.Add(Glance.FieldsAccessType.Public, methods);
            return result;
		}
		internal override Dictionary<Glance.FieldsAccessType, string[]> GetCppVariables()
		{
            /*
			return Glance.templates["Com:Collider:Vars"]
								.Replace("#ColliderType#", TypeToCppType(type))
								.Replace("#ColliderTypeName#", Glance.NameSetting.ColliderType)
								.Replace("#ColliderName#", Glance.NameSetting.ColliderName)
				.Split(';');
            */
            var result = new Dictionary<Glance.FieldsAccessType, string[]>();
            var variables = Glance.templates["Com:Collider:Vars"]
                                .Replace("#ColliderType#", TypeToCppType(type))
                                .Replace("#ColliderTypeName#", Glance.NameSetting.ColliderType)
                                .Replace("#ColliderName#", Glance.NameSetting.ColliderName).Split(';').gForEach(x => x.Trim());
            result.Add(Glance.FieldsAccessType.Public, variables);
            return result;
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
			return Glance.templates["Com:Collider:OnUpdate"].Replace("#ColliderName#", Glance.NameSetting.ColliderName);
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
