//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debug;
using System.Composition;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.ProductionDebug.LaunchProfile
{
    /// <summary>
    /// Represents the provider of custom UI for Connected Environment launch settings (i.e. profiles) within the project properties' Debug tab.
    /// </summary>
    [Export(typeof(ILaunchSettingsUIProvider))]
    [AppliesTo(ProjectCapabilities.DotNetCoreWeb)]
    [Order(150)]
    internal sealed class LaunchSettingsUIProvider : ILaunchSettingsUIProvider
    {
        public LaunchSettingsUIProvider()
        {

        }

        public string CommandName => LaunchProfileInitializer.LaunchProfileCommandName;

        public string FriendlyName => Resources.LaunchProfileUIFriendlyName;

        public UserControl CustomUI => new LaunchSettingUI();

        public void ProfileSelected(IWritableLaunchSettings curSettings)
        {
        }
        
        public bool ShouldEnableProperty(string propertyName)
        {
            return false;
        }
    }
}
