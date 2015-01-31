using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using MonoTouch.NUnit.UI;

using GoogleAnalytics.iOS;

using Mymdb.UI;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Mymdb.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		// class-level declarations
//		UIWindow window;
		public static UIStoryboard Storyboard = UIStoryboard.FromName("Movie", null);
		public IGAITracker Tracker;

		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            InitAnalytics();
            Forms.Init();
			Forms.ViewInitialized += (sender, e) => {
				if(e.View.StyleId != null)
					e.NativeView.AccessibilityIdentifier = e.View.StyleId;
			};
#if DEBUG
			Xamarin.Calabash.Start();
#endif

			ServiceRegistrar.Init();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}

		private void InitAnalytics()
		{
			// Initialize tracker.
			Tracker = GAI.SharedInstance.GetTracker(Mymdb.Core.Constants.GoogleAnalytics.ApiKey);

            Xamarin.Insights.Initialize(Core.Constants.Insights.ApiKey);
            Xamarin.Insights.Identify(UIDevice.CurrentDevice.IdentifierForVendor.ToString(), new Dictionary<string,string>());
            Xamarin.Insights.ForceDataTransmission = true;
            Xamarin.Insights.DisableCollection = false;
            Xamarin.Insights.DisableDataTransmission = false;
            Xamarin.Insights.DisableExceptionCatching = false; 

			Xamarin.Insights.Initialize(Mymdb.Core.Constants.Insights.ApiKey);
		}
	}
}