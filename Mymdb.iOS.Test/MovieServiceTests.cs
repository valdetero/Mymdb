
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Mymdb.Interfaces;
using Mymdb.Core.Services;

namespace Mymdb.iOS.Test
{
	[TestFixture]
	public class MovieServiceTests
	{
		[TestFixtureSetUp]
		public void Init()
		{
			IoC.ServiceContainer.Register<IMovieDatabaseService>(() => new MovieDatabaseService());
		}

		[TestFixtureTearDown]
		public void CleanUp()
		{
			IoC.ServiceContainer.Clear();
			GC.Collect();
		}

		[Test, Timeout(Int32.MaxValue)]
		public async Task GetMovie()
		{
			var movieService = IoC.ServiceContainer.Resolve<IMovieDatabaseService>();
			var movie = await movieService.GetMovie(550);

			Assert.IsNotNull(movie);
			Assert.IsTrue(movie.Title == "Fight Club");
		}

		[Test, Timeout(Int32.MaxValue)]
		public async Task GetMoviesNowPlaying()
		{
			var movieService = IoC.ServiceContainer.Resolve<IMovieDatabaseService>();
			var movies = await movieService.GetMoviesNowPlaying();

			Assert.IsNotNull(movies);
			Assert.IsTrue(movies.Count > 0);
		}
	}
}
