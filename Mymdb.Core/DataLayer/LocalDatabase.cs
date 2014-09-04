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
		private readonly AsyncLock locker = new AsyncLock();

		SQLiteAsyncConnection database;

		public LocalDatabase(SQLiteAsyncConnection conn)
		{
			database = conn;
			database.CreateTableAsync<Movie>();
		}

		public Task<List<T>> GetItems<T>() where T : class, IBusinessEntity, new()
		{
			using (locker.Lock())
			{
				return database.Table<T>().ToListAsync();
			}
		}

		public Task<T> GetItem<T>(int id) where T : class, IBusinessEntity, new()
		{
			using (locker.Lock())
			{
				return database.Table<T>().Where(x => x.Id == id)
                    //.OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();
			}
		}

		public async Task<int> SaveItem<T>(T item) where T : class, IBusinessEntity, new()
		{
			var exists = await database.Table<T>().Where(x => x.Id == item.Id).FirstOrDefaultAsync();

			using (await locker.LockAsync())
			{
				if (exists != null)
				{
					await database.UpdateAsync(item);
				}
				else
				{
					await database.InsertAsync(item);
				}
				return item.Id;
			}
		}

		public Task<int> DeleteItem<T>(int id) where T : class, IBusinessEntity, new()
		{
			using (locker.Lock())
			{
				return database.DeleteAsync<T>(id);
			}
		}
	}
}