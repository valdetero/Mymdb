using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Mymdb.Interfaces;
using Mymdb.Model;

namespace Mymdb.Core.ViewModels
{
	public class MoviesViewModel : ViewModelBase
	{
		private IMovieDatabaseService movieService;
		private IStorageService storageService;
		private IProgressIndicator progressIndicator;

		public MoviesViewModel()
		{
			movieService = IoC.ServiceContainer.Resolve<IMovieDatabaseService>();
			storageService = IoC.ServiceContainer.Resolve<IStorageService>();
			progressIndicator = IoC.ServiceContainer.Resolve<IProgressIndicator>();

			NeedsUpdate = true;
			Movies = new ObservableCollection<MovieViewModel>();

			this.IsBusyChanged = (busy) => 
			{
				if(busy)
					progressIndicator.Show();
				else
					progressIndicator.Hide();
			};
		}


		public bool NeedsUpdate { get; set; }

		public ObservableCollection<MovieViewModel> Movies { get; set; }

		public async Task ExecuteLoadMoviesCommand(bool loadImages = false)
		{
			if (IsBusy)
				return;

			IsBusy = true;

			Movies.Clear();
			NeedsUpdate = false;
			try
			{
				var movies = await movieService.GetMoviesNowPlaying();
				var savedMovies = await storageService.GetMovies();
				MovieViewModel vm;

				foreach (var movie in movies)
				{
					vm = new MovieViewModel();
					Movie saved = null;
					if(savedMovies != null)
						saved = savedMovies.SingleOrDefault(x => x.ImdbId == movie.ImdbId);
					if(saved != null)
						vm.Init(saved);
					else
						vm.Init(movie);

					if (loadImages && !string.IsNullOrEmpty(movie.ImagePath))
					{
						movie.Image = await movieService.DownloadImage(movie.ImagePath);
					}

					Movies.Add(vm);
				}
			}
			catch (Exception exception)
			{
				Debug.WriteLine("Unable to load movies");
			}
			finally
			{
				IsBusy = false;
			}
		}

		public string ExecuteGetImageCommand(string path)
		{
			if (IsBusy)
				return string.Empty;

			IsBusy = true;
			try
			{
				return movieService.CreateImageUrl(path);
			}
			catch (Exception)
			{
				Debug.WriteLine("Unable to create image url");
			}
			finally
			{
				IsBusy = false;
			}
			return string.Empty;
		}

		public Task<byte[]> ExecuteDownloadImageCommand(string path)
		{
			if (IsBusy)
				return null;

			IsBusy = true;
			try
			{
				var url = movieService.CreateImageUrl(path);
				return movieService.DownloadImage(url);
			}
			catch (Exception)
			{
				Debug.WriteLine("Unable to download image");
			}
			finally
			{
				IsBusy = false;
			}
			return null;
		}
	}
}

