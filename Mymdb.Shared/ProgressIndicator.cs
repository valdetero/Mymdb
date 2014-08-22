using System;

namespace Mymdb
{
	public class ProgressIndicator : Interfaces.IProgressIndicator
	{
		public void Show(string message = "Loading...")
		{
#if __IOS__
			BigTed.BTProgressHUD.Show(message);
#elif __ANDROID__
            var context = IoC.ServiceContainer.Resolve<Xamarin.Forms.Platform.Android.AndroidActivity>();
            AndroidHUD.AndHUD.Shared.Show(context, message);
#endif
		}

		public void Hide()
		{
#if __IOS__
			BigTed.BTProgressHUD.Dismiss();
#elif __ANDROID__
			AndroidHUD.AndHUD.Shared.Dismiss();
#endif
		}
	}
}

