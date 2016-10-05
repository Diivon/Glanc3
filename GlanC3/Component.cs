﻿using System;
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
				List<SpriteFrame> Frames;

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
					string code = "";
					foreach (var i in Frames)
						code += "_anim.emplaceFrame(" + Glance.ToCppString(i.PicName) + ", " + i.Duration.ToString("0.00").Replace(',', '.') + "f);\n"; 
					return Glance.templates["Com:Animation:ConstructorBody"].Replace("#SpriteFrames#", code);
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
			public string file;
			///<summary>do not return signature </summary>
			private string _GetMethod(string sign)
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
				return "";
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
			public Script(string filepath)
			{
				file = filepath;
			}
		}
	}
}