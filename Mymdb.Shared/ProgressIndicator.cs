using System;

namespace Mymdb
{
	public class ProgressIndicator : Acr.XamForms.UserDialogs.IProgressDialog
	{
		public void SetCancel(Action onCancel, string cancelText = "Cancel") { }
			
		public string Title { get; set; }

		public int PercentComplete { get; set; }

		public bool IsDeterministic { get; set; }

		public bool IsShowing { get; set; }

		public void Dispose() { }

		public void Show() { }

		public void Hide() { }
	}
}