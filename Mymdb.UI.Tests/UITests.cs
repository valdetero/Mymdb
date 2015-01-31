using System;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.IO;
using Xamarin.UITest.iOS;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Mymdb.UI.Tests;

namespace Mymdb.iOS.Test
{
	[TestFixture]
	public class UITests
	{
		public string PathToIPA { get; set; }
		public string PathToAPK { get; set; }
		IScreenQueries _queries;
		IApp _app;

		void ConfigureTest(Platform platform)
		{
			switch(platform) 
			{
				case Platform.Android:
					ConfigureAndroidApp();
					break;
				case Platform.iOS:
					_queries = new iOSQueries();
					ConfigureiOSApp();
					break;
			}
		}

		void ConfigureiOSApp()
		{
			if(TestEnvironment.Platform.Equals(TestPlatform.Local)) 
			{
				_app = ConfigureApp.iOS
					.EnableLocalScreenshots()
					.AppBundle(PathToIPA)
					.StartApp();
			} 
			else 
			{
				_app = ConfigureApp.iOS.StartApp();
			}
		}

		void ConfigureAndroidApp()
		{
			if(TestEnvironment.Platform.Equals(TestPlatform.Local)) 
			{
				_app = ConfigureApp.Android
					.ApkFile(PathToAPK)
					.EnableLocalScreenshots()
					.StartApp();
			} 
			else 
			{
				_app = ConfigureApp.Android.StartApp();
			}
		}

		[TestFixtureSetUp]
		public void Init()
		{
			string currentFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
			FileInfo fi = new FileInfo(currentFile);
			string dir = fi.Directory.Parent.Parent.Parent.FullName;
			PathToIPA = Path.Combine(dir, "Mymdb.iOS", "bin", "iPhoneSimulator", "Debug", "MymdbiOS.app");
			PathToAPK = Path.Combine(dir, "Mymdb.Droid", "bin", "Debug", "Mymdb.Droid.apk");
		}

		[SetUp]
		public void SetUp()
		{
			_app = ConfigureApp.iOS.AppBundle(PathToIPA).StartApp();
		}

		[TestCase(Platform.iOS)]
		[TestCase(Platform.Android)]
		public void Open_Repl(Platform platform)
		{
			_app.Repl();

			AppResult[] results = _app.Query(c => c.All());
		}

		[TestCase(Platform.iOS)]
		[TestCase(Platform.Android)]
		public void Verify_Movies_Shown(Platform platform)
		{
			ConfigureTest(platform);

			_app.Screenshot("App load.");

			AppResult[] result = _app.Query(_queries.MovieNameView); 

			Assert.IsTrue(result.Any(), "No movies returned.");
		}
	}
}
