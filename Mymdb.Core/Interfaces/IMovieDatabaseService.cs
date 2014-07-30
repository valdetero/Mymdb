using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Mymdb.Model;

namespace Mymdb.Interfaces
{
	public interface IMovieDatabaseService
	{
		Task<Movie> GetMovie(int id);
		Task<List<Movie>> GetMoviesNowPlaying();
		Task<byte[]> DownloadImage(string path);
		string CreateImageUrl(string imageName);
	}
}

