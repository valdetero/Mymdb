using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Mymdb.Interfaces;
using Mymdb.Model;
using System.Windows.Input;

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
			Movies = new ObservableCollection<Movie>();

			this.IsBusyChanged = (busy) => 
			{
				if(busy)
					progressIndicator.Show();
				else
					progressIndicator.Hide();
			};
		}


		public bool NeedsUpdate { get; set; }

		public ObservableCollection<Movie> Movies { get; set; }

		private RelayCommand<bool> loadMoviesCommand;
		public ICommand LoadMoviesCommand
		{
			get { return loadMoviesCommand ?? (loadMoviesCommand = new RelayCommand<bool>(async (loadImages) => await ExecuteLoadMoviesCommand(loadImages))); }
		}
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

				Movie movieToAdd = null;
				foreach (var movie in movies)
				{
					if(savedMovies != null)
						movieToAdd = savedMovies.SingleOrDefault(x => x.ImdbId == movie.ImdbId);

					if(movieToAdd == null)
						movieToAdd = movie;

					Movies.Add(movieToAdd);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to load movies: " + ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private RelayCommand<string> getImageCommand;
		public ICommand GetImageCommand
		{
			get { return getImageCommand ?? (getImageCommand = new RelayCommand<string>((item) => ExecuteGetImageCommand(item))); }
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
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to create image url: " + ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
			return string.Empty;
		}

		private RelayCommand<string> downloadImageCommand;
		public ICommand DownloadImageCommand
		{
			get { return downloadImageCommand ?? (downloadImageCommand = new RelayCommand<string>((item) => ExecuteDownloadImageCommand(item))); }
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
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to download image: " + ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
			return null;
		}
	}
}

