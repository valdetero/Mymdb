
using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinIOS;

using Mymdb.Model;
using Mymdb.Interfaces;
using Mymdb.Core.Services;

namespace Mymdb.iOS.Test
{

	[TestFixture]
	public class StorageServiceTests
	{
		SQLiteAsyncConnection connection = null;
		SQLiteConnectionWithLock connLock = null;
		const int MOVIE_ID = 12345;
		string DB_LOCATION 
		{
			get 
			{ 
				var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				var library = Path.Combine(docsPath, "../Library/");
				return Path.Combine(library, "videoDB.db3");
			}
		}

		[TestFixtureSetUp]
		public void Init()
		{
			connLock = new SQLiteConnectionWithLock(new SQLitePlatformIOS(), new SQLiteConnectionString(DB_LOCATION, false));
			var connectionFactory = new Func<SQLiteConnectionWithLock>(() => connLock);
			connection = new SQLiteAsyncConnection(connectionFactory);

			IoC.ServiceContainer.Register<IStorageService>(() => new StorageService(connection));
		}

		[TestFixtureTearDown]
		public void CleanUp()
		{
			if(connLock != null)
				connLock.Close();
			IoC.ServiceContainer.Clear();
			connection = null;
			connLock = null;
			GC.Collect();

			System.IO.File.Delete(DB_LOCATION);
		}

		[Test, Timeout(Int32.MaxValue)]
		public async Task SaveAndDeleteMovie()
		{
			var storageService = IoC.ServiceContainer.Resolve<IStorageService>();

			await storageService.SaveMovie(new Movie { Id = MOVIE_ID });
			var result = await storageService.GetMovie(MOVIE_ID);
			var count = await storageService.DeleteMovie(MOVIE_ID);

			Assert.IsNotNull(result);
			Assert.IsTrue(MOVIE_ID == result.Id);
			Assert.IsTrue(count == 1);
		}
	}

}
