using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glc.Component
{
	public class Sound : Component
	{
		public string FileName;
		public Sound(string filePath)
		{
			FileName = filePath;
		}
		internal override Dictionary<Glance.FieldsAccessType, string[]> GetCppVariables()
		{
            var result = new Dictionary<Glance.FieldsAccessType, string[]>();
            result.Add(Glance.FieldsAccessType.Public, Glance.templates["Com:Sound:Vars"].Split(';').gForEach(x => x.Trim()));
            return result;
		}
		internal override Dictionary<Glance.FieldsAccessType, string> GetCppMethodsDeclaration()
		{
            var result = new Dictionary<Glance.FieldsAccessType, string>();
            var methods = Glance.templates["Com:Sound:Methods"].Split(';').gForEach(x => x.Trim());
            foreach (var i in methods)
                result.Add(Glance.FieldsAccessType.Public, i);
            return result;
		}
		internal override Dictionary<string, string> GetCppMethodsImplementation()
		{
			return new Dictionary<string, string>();
		}
		internal override string[] GetCppConstructor()
		{
			return Glance.templates["Com:Sound:Constructor"].Replace("#FileName#", Glance.ToCppString(FileName)).Split(',').gForEach(x => x.Trim());
		}
		internal override string GetCppConstructorBody()
		{
			return Glance.templates["Com:Sound:ConstructorBody"];
		}
		internal override string GetCppOnStart()
		{
			return Glance.templates["Com:Sound:OnStart"];
		}
		internal override string GetCppOnUpdate()
		{
			return Glance.templates["Com:Sound:OnUpdate"];
		}
	}//class Sound
}
