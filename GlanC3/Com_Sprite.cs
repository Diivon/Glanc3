using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glc.Component
{
	public abstract partial class GraphicalComponent : Component
	{
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
				List<string> a = new List<string>();
				a.AddRange(Glance.templates["Com:Sprite:Vars"].Split(';'));
				string[] result = new string[a.Count];
				for (int i = 0; i < a.Count; ++i)
					result[i] = a[i].Trim();
				return result;
			}
			internal override string[] GetCppMethodsDeclaration()
			{
				List<string> a = new List<string>();
				a.AddRange(Glance.templates["Com:Sprite:Methods"].Split(';'));
				string[] result = new string[a.Count];
				for (int i = 0; i < a.Count; ++i)
					result[i] = a[i].Trim();
				return result;
			}
			internal override Dictionary<string, string> GetCppMethodsImplementation()
			{
				var result = new Dictionary<string, string>();
				foreach (var i in GetCppMethodsDeclaration())
					result.Add(i, "");
				return result;
			}
			internal override string GetCppConstructor()
			{
				return Glance.templates["Com:Sprite:Constructor"].Replace("#FileName#", Glance.ToCppString(FileName)).Trim();
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
