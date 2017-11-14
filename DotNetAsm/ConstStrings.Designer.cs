﻿namespace DotNetAsm
{
    using System;
    using System.Reflection;

    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ConstStrings
    {
        private static System.Resources.ResourceManager resourceMan;

        private static System.Globalization.CultureInfo resourceCulture;

        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ConstStrings()
        {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.Equals(null, resourceMan))
                {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("DotNetAsm.ConstStrings", typeof(Patterns).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to .block
        /// </summary>
        public static string OPEN_SCOPE
        {
            get
            {
                return ResourceManager.GetString("OPEN_SCOPE", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to .endblock
        /// </summary>
        public static string CLOSE_SCOPE
        {
            get
            {
                return ResourceManager.GetString("CLOSE_SCOPE", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to @@__SHADOW__@@
        /// </summary>
        public static string SHADOW_SOURCE
        {
            get
            {
                return ResourceManager.GetString("SHADOW_SOURCE", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to .let
        /// </summary>
        public static string VAR_DIRECTIVE
        {
            get
            {
                return ResourceManager.GetString("VAR_DIRECTIVE", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to @@__CMDARG__@@
        /// </summary>
        public static string COMMANDLINE_ARG
        {
            get
            {
                return ResourceManager.GetString("COMMANDLINE_ARG", resourceCulture);
            }
        }
    }
}
