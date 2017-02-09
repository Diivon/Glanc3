using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glc.Component
{
	public abstract partial class GraphicalComponent : Component
	{
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
			internal override string[] GetCppVariables()
			{
				List<string> a = new List<string>();
				a.AddRange(_GetProcessed(Glance.templates["Com:Animation:Vars"]).Split(';'));
				string[] result = new string[a.Count];
				for (int i = 0; i < a.Count; ++i)
					result[i] = a[i].Trim();
				return result;
			}
			internal override string[] GetCppMethodsDeclaration()
			{
				List<string> a = new List<string>();
				a.AddRange(_GetProcessed(Glance.templates["Com:Animation:Methods"]).Split(';'));
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
						.Trim()
						;
			}
		}
	}
}
