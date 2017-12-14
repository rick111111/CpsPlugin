//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.ProjectSystem.Debug;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.ProductionDebug.LaunchProfile
{
    internal sealed class LaunchProfile : ILaunchProfile, IPersistOption
    {
        #region ILaunchProfile Members

        public string Name { get; set; }

        public string CommandName { get; set; }

        public string ExecutablePath { get; set; }

        public string CommandLineArgs { get; set; }

        public string WorkingDirectory { get; set; }

        public bool LaunchBrowser { get; set; }

        public string LaunchUrl { get; set; }

        public ImmutableDictionary<string, string> EnvironmentVariables { get; set; }

        public ImmutableDictionary<string, object> OtherSettings { get; set; }

        #endregion

        #region IPersistOption

        public bool DoNotPersist { get; set; }

        #endregion
    }
}
