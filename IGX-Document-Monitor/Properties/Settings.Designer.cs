﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IGX_Document_Monitor.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<DocumentSettings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <DocumentSetting>
    <Name>Page Version</Name>
    <Regex>^PageVersion_x(\d)+.*$</Regex>
    <TrackChanges>true</TrackChanges>
    <VersionLimit>10</VersionLimit>
  </DocumentSetting>
  <DocumentSetting>
    <Name>Assets</Name>
    <Regex>^a/(\d)+$</Regex>
    <TrackChanges>false</TrackChanges>
    <VersionLimit>0</VersionLimit>
  </DocumentSetting>
</DocumentSettings>")]
        public global::System.Xml.XmlDocument DocumentSettings {
            get {
                return ((global::System.Xml.XmlDocument)(this["DocumentSettings"]));
            }
            set {
                this["DocumentSettings"] = value;
            }
        }
    }
}
