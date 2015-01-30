
using System;
using NUnit.Framework;
using System.Reflection;
using System.IO;
using Xamarin.UITest.iOS;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Mymdb.iOS.Test
{
	[TestFixture]
	public class UITests
	{
		public string PathToIPA { get; set; }
		IApp _app;

		[TestFixtureSetUp]
		public void Init()
		{
			string currentFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
			FileInfo fi = new FileInfo(currentFile);
			string dir = fi.Directory.Parent.Parent.Parent.FullName;
			PathToIPA = Path.Combine(dir, "Mymdb.iOS", "bin", "iPhoneSimulator", "Debug", "MymdbiOS.app");
		}

		[SetUp]
		public void SetUp()
		{
			_app = ConfigureApp.iOS.AppBundle(PathToIPA).StartApp();
		}

		[Test]
		public void Open_Repl()
		{
			_app.Query(c => c.Class("UILabel"));

			_app.Repl();

			AppResult[] results = _app.Query(c => c.All());
		}
	}
}
