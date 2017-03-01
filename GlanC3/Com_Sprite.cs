using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glc.Component
{
	public abstract partial class GraphicalComponent : Component
	{
		public class Sprite : GraphicalComponent
		{
			string FileName;
			public Sprite(string fn)
			{
				FileName = fn;
			}
			internal override Dictionary<Glance.FieldsAccessType, string[]> GetCppVariables()
			{
                var result = new Dictionary<Glance.FieldsAccessType, string[]>();
                result.Add(Glance.FieldsAccessType.Public, Glance.templates["Com:Sprite:Vars"].Split(';').gForEach(x => x.Trim()));
                return result;
            }
			internal override Dictionary<Glance.FieldsAccessType, string[]> GetCppMethodsDeclaration()
			{
                var result = new Dictionary<Glance.FieldsAccessType, string[]>();
                var methods = Glance.templates["Com:Sprite:Methods"].Split(';').gForEach(x => x.Trim());
                result.Add(Glance.FieldsAccessType.Public, methods);
                return result;
            }
			internal override Dictionary<string, string> GetCppMethodsImplementation()
			{
                //выкинуть нахуй инфу про доступ, заполнить вэлью пустыми строками
			}
			internal override string[] GetCppConstructor()
			{
				return Glance.templates["Com:Sprite:Constructor"].Replace("#FileName#", Glance.ToCppString(FileName))
							.Trim().Split(',').gForEach(x => x.Trim());
			}
			internal override string GetCppConstructorBody()
			{
				return Glance.templates["Com:Sprite:ConstructorBody"].Replace("#FileName#", Glance.ToCppString(FileName));
			}
			internal override string GetCppOnUpdate()
			{
				return Glance.templates["Com:Sprite:OnUpdate"];
			}
			internal override string GetCppOnStart()
			{
				return Glance.templates["Com:Sprite:OnUpdate"];
			}
		}
	}
}
