using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using GoogleAnalytics.iOS;

using Mymdb.UI;

using Xamarin.Forms;

namespace Mymdb.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
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
			ServiceRegistrar.Init();

			window = new UIWindow(UIScreen.MainScreen.Bounds);
//			window.RootViewController = new UINavigationController(new MoviesViewController());
			window.RootViewController = App.GetMainPage().CreateViewController();
			
			// make the window visible
			window.MakeKeyAndVisible();
			
			return true;
		}

		private void InitAnalytics()
		{
			// Initialize tracker.
			Tracker = GAI.SharedInstance.GetTracker(Mymdb.Core.Constants.GoogleAnalytics.ApiKey);

            Xamarin.Insights.Initialize(Core.Constants.Insights.ApiKey);
        }
	}
}