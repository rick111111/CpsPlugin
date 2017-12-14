//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debug;
using Microsoft.VisualStudio.ProjectSystem.VS.Debug;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.ProductionDebug.LaunchProfile
{
    [Export(typeof(IDebugProfileLaunchTargetsProvider))]
    [AppliesTo(ProjectCapabilities.DotNetCoreWeb)]
    [Order(200)]
    internal sealed class DebugProfileLaunchTargetsProvider : IDebugProfileLaunchTargetsProvider, IDisposable
    {
        private ConfiguredProject _configuredProject;
        private IProjectThreadingService _projectThreadingService;

        [Import]
        private SVsServiceProvider ServiceProvider { get; set; }

        [ImportingConstructor]
        public DebugProfileLaunchTargetsProvider(ConfiguredProject configuredProject, IProjectThreadingService projectThreadingService)
        {
            Assumes.ThrowIfNull(configuredProject, nameof(configuredProject));
            Assumes.ThrowIfNull(projectThreadingService, nameof(projectThreadingService));

            _configuredProject = configuredProject;
            _projectThreadingService = projectThreadingService;
        }

        public void Dispose()
        {
        }

        #region IDebugProfileLaunchTargetsProvider

        public Task OnAfterLaunchAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile)
        {
            return Task.CompletedTask;
        }

        public Task OnBeforeLaunchAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile)
        {
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<IDebugLaunchSettings>> QueryDebugTargetsAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile)
        {
            // NOTE: This method is called from the main (UI) thread and must remain on that thread!
            if (profile.IsSnapshotDebuggerProfile())
            {
                IVsDebugger dbg = ServiceProvider.GetService(typeof(IVsDebugger)) as IVsDebugger;
            }

            return new List<IDebugLaunchSettings>();
        }

        public bool SupportsProfile(ILaunchProfile profile)
        {
            return profile.IsSnapshotDebuggerProfile();
        }

        #endregion


    }
}
