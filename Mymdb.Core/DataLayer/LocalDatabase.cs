using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Async;

using Mymdb.Interfaces;
using Mymdb.Model;

namespace Mymdb.Core.DataLayer
{
	public class LocalDatabase
	{
		static object locker = new object();

		SQLiteAsyncConnection database;

		public LocalDatabase(SQLiteAsyncConnection conn)
		{
			database = conn;
			database.CreateTableAsync<Movie>();
		}

		public Task<List<T>> GetItems<T>() where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return database.Table<T>().ToListAsync();
			}
		}

		public Task<T> GetItem<T>(int id) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return database.Table<T>().Where(x => x.Id == id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
			}
		}

		public async Task<int> SaveItem<T>(T item) where T : IBusinessEntity, new()
		{
			var exists = await database.Table<T>().Where(x => x.Id == item.Id).FirstOrDefaultAsync();

			lock (locker)
			{
				//Id is not an auto-increment
				if (exists != null)
//				if (item.Id != 0)
				{
					database.UpdateAsync(item).ContinueWith(t => t.Result);
				}
				else
				{
					database.InsertAsync(item).ContinueWith(t => t.Result);
				}
				return item.Id;
			}
		}

		public Task<int> DeleteItem<T>(int id) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return database.DeleteAsync<T>(id);
			}
		}
	}
}