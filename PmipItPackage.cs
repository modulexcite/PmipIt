using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace PmipIt
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class PmipItPackage : Package
	{
		public const string PackageGuidString = "0aa8493b-87d6-453f-b5c9-89acdb7a3c93";

		public DTE Dte => (DTE)GetService(typeof(DTE));
		public IVsOutputWindow OutputWindow => GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

		public IVsOutputWindowPane OutputWindowDebugPane
		{
			get
			{
				var debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;
				IVsOutputWindowPane debugPane;
				OutputWindow.GetPane(ref debugPaneGuid, out debugPane);
				return debugPane;
			}
		}

		protected override void Initialize()
		{
			PmipItCommand.Initialize(this);
			base.Initialize();
		}
	}
}
