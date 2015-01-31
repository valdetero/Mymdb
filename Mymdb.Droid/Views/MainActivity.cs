using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mymdb.UI;

namespace Mymdb.Droid.Views
{
    [Activity(Label = "Mymdb", MainLauncher=true)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
			Xamarin.Insights.Initialize(Mymdb.Core.Constants.Insights.ApiKey, this);

            base.OnCreate(bundle);

            Xamarin.Insights.Initialize(Core.Constants.Insights.ApiKey, this);
            Xamarin.Insights.Identify(getDeviceId(), new Dictionary<string, string>());
            Xamarin.Insights.ForceDataTransmission = true;
            Xamarin.Insights.DisableCollection = false;
            Xamarin.Insights.DisableDataTransmission = false;
            Xamarin.Insights.DisableExceptionCatching = false;

            Xamarin.Forms.Forms.Init(this, bundle);
			Xamarin.Forms.Forms.ViewInitialized += (sender, e) => {
				if(!string.IsNullOrWhiteSpace(e.View.StyleId))
					e.NativeView.ContentDescription = e.View.StyleId;
			};

			IoC.ServiceContainer.Register<Xamarin.Forms.Platform.Android.FormsApplicationActivity>(() => this);
            ServiceRegistrar.Init();

			LoadApplication(new App());
        }

        private string getDeviceId()
        {
            var service = GetSystemService(Context.TelephonyService);
            var mgr = (Android.Telephony.TelephonyManager)service;
            var device = mgr.DeviceId;

            return device;
        }
    }
}