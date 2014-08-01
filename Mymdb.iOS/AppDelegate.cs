using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using GoogleAnalytics.iOS;

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
			// create a new window instance based on the screen size
			window = new UIWindow(UIScreen.MainScreen.Bounds);

			ServiceRegistrar.Init();
			InitAnalytics();

			// If you have defined a root view controller, set it here:
			window.RootViewController = new UINavigationController(new MoviesViewController());
			
			// make the window visible
			window.MakeKeyAndVisible();
			
			return true;
		}

		private void InitAnalytics()
		{
			// Optional: set Google Analytics dispatch interval to e.g. 20 seconds.
			GAI.SharedInstance.DispatchInterval = 20;

			// Optional: automatically send uncaught exceptions to Google Analytics.
			GAI.SharedInstance.TrackUncaughtExceptions = true;

			// Initialize tracker.
			Tracker = GAI.SharedInstance.GetTracker(Mymdb.Core.Constants.GoogleAnalytics.ApiKey);

			Segment.Analytics.Initialize(Mymdb.Core.Constants.SegmentIO.ApiKey);
		}
	}
}

