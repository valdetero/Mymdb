using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Mymdb.Interfaces
{
	public interface IStorageService
	{
		Task<Model.Movie> GetMovie(int id);
		Task<List<Model.Movie>> GetMovies();
		Task<int> SaveMovie(Model.Movie movie);
		Task<int> DeleteMovie(int id);
	}
}

