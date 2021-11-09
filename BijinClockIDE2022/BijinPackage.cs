using System;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BijinClockIDE
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid("61CD8645-62C1-459F-9475-E99836392E62")]
	public class BijinPackage : AsyncPackage
	{
		public BijinPackage()
		{
		}

		#region Package Members
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			Debug.WriteLine(string.Format("Entering Initialize() of: {0}", this.ToString()));
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
		}
		#endregion
	}
}
