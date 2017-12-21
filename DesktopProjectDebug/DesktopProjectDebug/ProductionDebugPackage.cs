using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

namespace DesktopProjectDebug
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#1110", "#1112", "1.0", IconResourceID = 1400)] // Info on this package for Help/About
    [Guid(ProductionDebugPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class ProductionDebugPackage : Package
    {
        /// <summary>
        /// ProductionDebugPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "f4b0addc-db57-42f6-a914-7136780b51d3";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionDebugPackage"/> class.
        /// </summary>
        public ProductionDebugPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            base.AddOptionKey(SnapshotDebugUserSettings.UserSettingKey);
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        #endregion

        internal static SnapshotDebugConfigManager DebugConfigManager { get; set; } = new SnapshotDebugConfigManager();

        protected override void OnLoadOptions(string key, Stream stream)
        {
            if (key == SnapshotDebugUserSettings.UserSettingKey)
            {
                DebugConfigManager.LoadConfigSettings(stream);
            }

            base.OnLoadOptions(key, stream);
        }

        protected override void OnSaveOptions(string key, Stream stream)
        {
            if (key == SnapshotDebugUserSettings.UserSettingKey)
            {
                DebugConfigManager.SaveConfigSettings(stream);
            }

            base.OnSaveOptions(key, stream);
        }
    }
}
