﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KinectWPFGUI {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class KinectSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static KinectSettings defaultInstance = ((KinectSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new KinectSettings())));
        
        public static KinectSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("kinectColorData")]
        public string ColorMemoryMappedFileName {
            get {
                return ((string)(this["ColorMemoryMappedFileName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("kinectDepthMutex")]
        public string DepthMutexName {
            get {
                return ((string)(this["DepthMutexName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Testing {
            get {
                return ((bool)(this["Testing"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("kinectDepthData")]
        public string DepthMemoryMappedFileName {
            get {
                return ((string)(this["DepthMemoryMappedFileName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("kinectColorMutex")]
        public string ColorMutexName {
            get {
                return ((string)(this["ColorMutexName"]));
            }
        }
    }
}
