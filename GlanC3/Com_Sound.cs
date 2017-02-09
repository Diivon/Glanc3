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
		internal override string[] GetCppVariables()
		{
			return Glance.templates["Com:Sound:Vars"].Split(';');
		}
		internal override string[] GetCppMethodsDeclaration()
		{
			return Glance.templates["Com:Sound:Methods"].Split(';');
		}
		internal override Dictionary<string, string> GetCppMethodsImplementation()
		{
			return new Dictionary<string, string>();
		}
		internal override string GetCppConstructor()
		{
			return Glance.templates["Com:Sound:Constructor"].Replace("#FileName#", Glance.ToCppString(FileName));
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
