using System;
using System.IO;

using SQLite.Net.Async;
using SQLite.Net.Interop;
using SQLite.Net;

using Mymdb.Interfaces;
using Mymdb.Core.Services;
using Mymdb.Core.ViewModels;

namespace Mymdb
{
	public static class ServiceRegistrar
    {
        public static void Init()
		{
            SQLiteAsyncConnection connection = null;
			ISQLitePlatform platform = null;
			Acr.XamForms.UserDialogs.IProgressDialog dialog;
			string dbLocation = "MymdbDB.db3";

#if __IOS__
			var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var library = Path.Combine(docsPath, "../Library/");
			dbLocation = Path.Combine(library, dbLocation);
			platform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
			dialog = new Acr.XamForms.UserDialogs.iOS.ProgressDialog(){ Title = "Loading..." };
#elif __ANDROID__
			var library = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			dbLocation = Path.Combine(library, dbLocation);
			platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
			dialog = new Acr.XamForms.UserDialogs.Droid.ProgressDialog(){ Title = "Loading..." };
#elif WINDOWS_PHONE // Phone Silverlight
			platform = new SQLite.Net.Platform.WindowsPhone8.SQLitePlatformWP8();
			dbLocation = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbLocation);
			dialog = new Acr.XamForms.UserDialogs.WindowsPhone.ProgressDialog(){ Title = "Loading..." };
#elif WINDOWS_PHONE_APP // Universal
			//no support for SQLite.Net yet
			dialog = new ProgressIndicator();
#elif WINDOWS_APP // RT\Store
			platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
			dbLocation = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbLocation);
			dialog = new ProgressIndicator();
#else //desktop
			platform = new SQLite.Net.Platform.Win32.SQLitePlatformWin32();
			dialog = new ProgressIndicator();
#endif

			var connectionFactory = new Func<SQLiteConnectionWithLock>(() => 
				new SQLiteConnectionWithLock(platform, new SQLiteConnectionString(dbLocation, false)));
			connection = new SQLiteAsyncConnection(connectionFactory);

			IoC.ServiceContainer.Register<IStorageService>(() => new StorageService(connection));
			IoC.ServiceContainer.Register<IMovieDatabaseService>(() => new MovieDatabaseService());
			IoC.ServiceContainer.Register<Acr.XamForms.UserDialogs.IProgressDialog>(() => dialog);
			IoC.ServiceContainer.Register<MoviesViewModel>();
			IoC.ServiceContainer.Register<MovieViewModel>();
		}
    }
}

