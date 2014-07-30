using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mymdb.Core.DataLayer;
using Mymdb.Model;
using SQLite.Net.Async;

namespace Mymdb.Core.Services
{
	public class StorageService : Interfaces.IStorageService
	{
		LocalDatabase db = null;
		protected static string dbLocation;

		public StorageService(SQLiteAsyncConnection conn)
		{
			db = new LocalDatabase(conn);
		}

		public Task<List<Movie>> GetMovies()
		{
			return db.GetItems<Movie>();
		}

		public Task<Movie> GetMovie(int id)
		{
			return db.GetItem<Movie>(id);
		}

		public Task<int> SaveMovie(Movie movie)
		{
			return db.SaveItem<Movie>(movie);
		}

		public Task<int> DeleteMovie(int id)
		{
			return db.DeleteItem<Movie>(id);
		}
	}
}