﻿using System;

namespace Glc
{
    public class SpriteFrame
    {
		/// <summary>Path to picture</summary>
		public string FilePath;
		/// <summary>Duration of this picture</summary>
		public float Duration; 
		public SpriteFrame(string picName, float dur)
		{
			FilePath = picName;
			Duration = dur;
		}
		/// <summary>Returns cpp code which create gc::Vec2</summary>
		/// <returns> ::gc::SpriteFrame(FilePath, Duration) </returns>
		internal string GetCppCtor()
		{
			if (FilePath == null)
				throw new Exception("In SpriteFrame.GetCppCtor(): FilePath != null assertion failed");
			return "::gc::SpriteFrame(::gc::Sprite(" + Glance.ToCppString(FilePath) + "), " + Glance.floatToString(Duration) + ")";
		}
    }
}
