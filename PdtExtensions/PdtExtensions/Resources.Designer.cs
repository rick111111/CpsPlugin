﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.VisualStudio.Debugger.Parallel.Extension.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the Azure App Service to attach the Snapshot Debugger.
        /// </summary>
        internal static string AppServiceDialogDescription {
            get {
                return ResourceManager.GetString("AppServiceDialogDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to App Service.
        /// </summary>
        internal static string AppServiceDialogTitle {
            get {
                return ResourceManager.GetString("AppServiceDialogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Snapshot Debugger.
        /// </summary>
        internal static string DebugConfigDisplayName {
            get {
                return ResourceManager.GetString("DebugConfigDisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Azure App Service (Snapshot Debugging).
        /// </summary>
        internal static string DebugExtensionTargetMenuCommand {
            get {
                return ResourceManager.GetString("DebugExtensionTargetMenuCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Snapshot Debugger.
        /// </summary>
        internal static string DebugPropertyPageTitle {
            get {
                return ResourceManager.GetString("DebugPropertyPageTitle", resourceCulture);
            }
        }
    }
}
