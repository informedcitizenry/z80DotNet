﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DotNetAsm {
    using System;
    using System.Reflection;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ConstStrings {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ConstStrings() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("DotNetAsm.ConstStrings", typeof(ConstStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string OPEN_SCOPE {
            get {
                return ResourceManager.GetString("OPEN_SCOPE", resourceCulture);
            }
        }
        
        internal static string CLOSE_SCOPE {
            get {
                return ResourceManager.GetString("CLOSE_SCOPE", resourceCulture);
            }
        }
        
        internal static string SHADOW_SOURCE {
            get {
                return ResourceManager.GetString("SHADOW_SOURCE", resourceCulture);
            }
        }
        
        internal static string VAR_DIRECTIVE {
            get {
                return ResourceManager.GetString("VAR_DIRECTIVE", resourceCulture);
            }
        }
        
        internal static string COMMANDLINE_ARG {
            get {
                return ResourceManager.GetString("COMMANDLINE_ARG", resourceCulture);
            }
        }
    }
}
