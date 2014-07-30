using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Mymdb.Model;
using Newtonsoft.Json;

namespace Mymdb.Core.Services
{
	public class MovieDatabaseService : Interfaces.IMovieDatabaseService
	{
		public string CreateImageUrl(string imageName)
		{
			return Constants.TheMovieDatabase.ImagePath + imageName;
		}

		public async System.Threading.Tasks.Task<byte[]> DownloadImage(string path)
		{
			using (var client = new HttpClient())
			{
				return await client.GetByteArrayAsync(path);
			}
		}

		public async System.Threading.Tasks.Task<Movie> GetMovie(int id)
		{
			string url = string.Format("{0}movie/{1}?api_key={2}", 
				Constants.TheMovieDatabase.BasePath, id, Constants.TheMovieDatabase.ApiKey);

			using (var client = new HttpClient())
			{
				var result = await client.GetStringAsync(url);
				return JsonConvert.DeserializeObject<Movie>(result);
			}
		}

		public async System.Threading.Tasks.Task<List<Movie>> GetMoviesNowPlaying()
		{
			string url = string.Format("{0}movie/now_playing?api_key={1}", 
				Constants.TheMovieDatabase.BasePath, Constants.TheMovieDatabase.ApiKey);

			using (var client = new HttpClient())
			{
				var result = await client.GetStringAsync(url);
				return JsonConvert.DeserializeObject<MovieDTO>(result).Movies.OrderBy(m => m.Title).ToList();
			}
		}
	}
}