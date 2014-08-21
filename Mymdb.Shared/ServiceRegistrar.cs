using System;
using System.IO;
//using System.Net.Http;

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
			string dbLocation = "MymdbDB.db3";

			#if __IOS__
			var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var library = Path.Combine(docsPath, "../Library/");
			dbLocation = Path.Combine(library, dbLocation);
			platform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
			#elif __ANDROID__
			var library = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			dbLocation = Path.Combine(library, dbLocation);
			platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
			#elif WINDOWS_PHONE
			platform = new SQLite.Net.Platform.WindowsPhone8.SQLitePlatformWP8();
			dbLocation = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbLocation);
			#elif WINDOWS_PHONE_APP
			//no support for SQLite.Net yet
			#elif WINDOWS_APP
			platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
			dbLocation = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbLocation);
			#else //desktop
			platform = new SQLite.Net.Platform.Win32.SQLitePlatformWin32();
			#endif

			var connectionFactory = new Func<SQLiteConnectionWithLock>(() => 
				new SQLiteConnectionWithLock(platform, new SQLiteConnectionString(dbLocation, false)));
			connection = new SQLiteAsyncConnection(connectionFactory);

			IoC.ServiceContainer.Register<IStorageService>(() => new StorageService(connection));
			IoC.ServiceContainer.Register<IMovieDatabaseService>(() => new MovieDatabaseService());
			IoC.ServiceContainer.Register<IProgressIndicator>(() => new ProgressIndicator());

			IoC.ServiceContainer.Register<MoviesViewModel>();
			IoC.ServiceContainer.Register<MovieViewModel>();
		}
	}
}

