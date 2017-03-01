using System;
using System.Collections.Generic;
using System.IO;

namespace Glc.Component
{
		public abstract class Component
		{
			/// <summary>return defined in script variables
			/// one variable per index</summary>
			/// <returns>return variables without semi-colon at the end</returns>
			internal abstract string[] GetCppVariables();
			/// <summary>return defined in component methods</summary>
			/// <returns>return method declaration without semi-colon at the end</returns>
			internal abstract string[] GetCppMethodsDeclaration();
			/// <summary>(declaration, implementation)</summary>
			internal abstract Dictionary<string, string> GetCppMethodsImplementation();

			/// <summary>return constructors for component variables
			/// as Initializaton List(which goes after colon)</summary>
			internal abstract string[] GetCppConstructor();
			/// <summary>return body for constructor</summary>
			internal abstract string GetCppConstructorBody();
			/// <summary>return code, that must be in onUpdate()</summary>
			internal abstract string GetCppOnUpdate();
			/// <summary>return code, that must be in onStart()</summary>
			internal abstract string GetCppOnStart();
		}
}//ns Glc