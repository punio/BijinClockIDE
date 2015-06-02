using System;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BijinClockIDE
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[Guid("61CD8645-62C1-459F-9475-E99836392E62")]
	public class BijinPackage : Package {
		public BijinPackage() {
		}

		#region Package Members
		protected override void Initialize() {
			Debug.WriteLine(string.Format("Entering Initialize() of: {0}", this.ToString()));
			base.Initialize();

		}
		#endregion
	}
}
