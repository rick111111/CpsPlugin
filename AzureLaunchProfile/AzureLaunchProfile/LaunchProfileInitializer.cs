using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debug;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.ProductionDebug.LaunchProfile
{
    [Export(ExportContractNames.Scopes.UnconfiguredProject, typeof(IProjectDynamicLoadComponent))]
    [AppliesTo(ProjectCapabilities.DotNetCoreWeb)]
    internal sealed class LaunchProfileInitializer : OnceInitializedOnceDisposed, IProjectDynamicLoadComponent
    {
        public const string LaunchProfileCommandName = "SnapshotDebugger";
        private const string ActiveDebugProfile = "ActiveDebugProfile";

        private readonly ILaunchSettingsProvider _launchSettingsProvider;
        private readonly UnconfiguredProject _project;
        private ILaunchSettings _currentLaunchSettings;
        private IDisposable _launchSettingsSubscription;

        [ImportingConstructor]
        public LaunchProfileInitializer(UnconfiguredProject project, ILaunchSettingsProvider launchSettingsProvider)
        {
            Assumes.ThrowIfNull(project, nameof(project));
            Assumes.ThrowIfNull(launchSettingsProvider, nameof(launchSettingsProvider));

            _project = project;
            _launchSettingsProvider = launchSettingsProvider;
        }

        #region OnceInitializedOnceDisposed

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _launchSettingsSubscription.Dispose();
            }
        }

        protected override void Initialize()
        {
            InitializeAsync().Forget();
        }

        #endregion

        #region IProjectDynamicLoadComponent

        public Task LoadAsync()
        {
            EnsureInitialized();

            return Task.CompletedTask;
        }

        public Task UnloadAsync()
        {
            Dispose();

            return Task.CompletedTask;
        }

        #endregion

        internal async Task InitializeAsync()
        {
            SubscribeToLaunchSettings();

            ILaunchSettings launchSettings = await _launchSettingsProvider.SourceBlock.ReceiveAsync().ConfigureAwait(false);

            try
            {
                ConfiguredProject configuredProject = await _project.GetSuggestedConfiguredProjectAsync().ConfigureAwait(false);
                IEnumerable<ILaunchProfile> launchProfiles = launchSettings?.Profiles ?? Enumerable.Empty<ILaunchProfile>();

                LaunchProfile profile = await CreateDefaultLaunchProfile(configuredProject, launchProfiles);
                await _launchSettingsProvider.AddOrUpdateProfileAsync(profile, addToFront: false).ConfigureAwait(false);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Assumes.Fail(e.ToString());
            }
        }

        private void SubscribeToLaunchSettings()
        {
            _currentLaunchSettings = _launchSettingsProvider.CurrentSnapshot;

            // Listen to when the launch profile has been changed. Ensure it's configured when our launch profile is selected.
            _launchSettingsSubscription = _launchSettingsProvider.SourceBlock.LinkTo(
                new ActionBlock<ILaunchSettings>(new Func<ILaunchSettings, Task>(
                    async launchSettings =>
                    {
                        try
                        {
                            if (_currentLaunchSettings != null && launchSettings?.ActiveProfile != null && launchSettings.ActiveProfile.IsSnapshotDebuggerProfile())
                            {
                                await EnsureAppServiceIsSelectedAsync(launchSettings).ConfigureAwait(false);
                            }
                        }
                        catch (Exception e) when (!(e is OperationCanceledException))
                        {
                            Assumes.Fail(e.ToString());
                        }
                        finally
                        {
                            _currentLaunchSettings = _launchSettingsProvider.CurrentSnapshot;
                        }
                    })));
        }

        private static async Task<LaunchProfile> CreateDefaultLaunchProfile(ConfiguredProject project, IEnumerable<ILaunchProfile> launchProfiles)
        {
            // Find the currently active debug profile so that we can use some of the same common properties that were defined in that profile.
            ILaunchProfile activeDebugProfile = await GetActiveDebugProfileAsync(project, launchProfiles);

            return new LaunchProfile
            {
                Name = Resources.LaunchProfileName,
                CommandName = LaunchProfileCommandName,
                LaunchBrowser = activeDebugProfile != null ? activeDebugProfile.LaunchBrowser : true,
                LaunchUrl = activeDebugProfile?.LaunchUrl,
                OtherSettings = ImmutableDictionary<string, object>.Empty,
                DoNotPersist = true
            };
        }

        private static async Task<ILaunchProfile> GetActiveDebugProfileAsync(ConfiguredProject project, IEnumerable<ILaunchProfile> launchProfiles)
        {
            var commonProperties = project.Services.UserPropertiesProvider.GetCommonProperties();
            string activeDebugProfile = await commonProperties.GetEvaluatedPropertyValueAsync(ActiveDebugProfile);

            if (String.IsNullOrEmpty(activeDebugProfile))
            {
                return launchProfiles.FirstOrDefault();
            }

            return launchProfiles.FirstOrDefault(p => p.Name == activeDebugProfile);
        }

        private async Task EnsureAppServiceIsSelectedAsync(ILaunchSettings launchSettings)
        {
            // If the profile is configured with our custom launch profile, ensure that it has a app service configured by prompting the user to select a project if it's not set.
            if (launchSettings?.Profiles != null && launchSettings.Profiles.Any(profile => profile.IsSnapshotDebuggerProfile()))
            {
                ILaunchProfile newlaunchProfile = launchSettings.Profiles.First(profile => profile.IsSnapshotDebuggerProfile());
                ILaunchProfile previousLaunchProfile = _currentLaunchSettings?.ActiveProfile;

                // TODO: Popup the UI to select App Service for debugging                
            }
        }
    }
}
