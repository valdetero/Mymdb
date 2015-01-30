using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Mymdb.Interfaces;
using Mymdb.Model;
using Mymdb.Core.Helpers;

using Xamarin;
using Xamarin.Forms;
using Acr.XamForms;
using Acr.XamForms.UserDialogs;

namespace Mymdb.Core.ViewModels
{
	public class MoviesViewModel : ViewModelBase
	{
		private IMovieDatabaseService movieService;
		private IStorageService storageService;
		private IProgressDialog progressDialog;

		public MoviesViewModel()
		{
			movieService = IoC.ServiceContainer.Resolve<IMovieDatabaseService>();
			storageService = IoC.ServiceContainer.Resolve<IStorageService>();
			progressDialog = IoC.ServiceContainer.Resolve<IProgressDialog>();

			NeedsUpdate = true;
			Movies = new ObservableCollection<MovieViewModel>();

			this.IsBusyChanged = (busy) => 
			{
				if(busy)
					progressDialog.Show();
				else
					progressDialog.Hide();
			};
		}


		public bool NeedsUpdate { get; set; }

		public ObservableCollection<MovieViewModel> Movies { get; set; }

		private RelayCommand<bool> loadMoviesCommand;
		public ICommand LoadMoviesCommand
		{
			get { return loadMoviesCommand ?? (loadMoviesCommand = new RelayCommand<bool>(async (loadImages) => await ExecuteLoadMoviesCommand(loadImages))); }
		}

        [Insights]
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

				Insights.Track(string.Format("Retrieved {0} movies", movies.Count));

				var savedMovies = await storageService.GetMovies();
				Insights.Track(string.Format("Retrieved {0} movies locally", savedMovies.Count));

                Movie movieToAdd = null;
				foreach (var movie in movies)
				{
					if(savedMovies != null)
					{
						foreach (var savedMovie in savedMovies.OrderBy(x => x.Title).ToList()) 
						{
							//movie from service exists in local db
							if(movie.Id == savedMovie.Id)
							{
								movieToAdd = savedMovie;
								savedMovies.Remove(savedMovie);
								break;
							}
							//local movie doesn't exist in service (since they are ordered by title)
							//add it now so they are still sorted correctly
							if(String.Compare(movie.Title, savedMovie.Title) > 0)
							{
								await addMovie(savedMovie);
								savedMovies.Remove(savedMovie);
							}
						}
					}

					if(movieToAdd == null)
						movieToAdd = movie;

					await addMovie(movieToAdd);

					movieToAdd = null;
				}

				//add any remaining local movies
				foreach(var movie in savedMovies) 
				{
					await addMovie(movie);
				}

                Insights.Track(string.Format("Displaying {0} movies", Movies.Count));
            }

			catch (Exception ex)
			{
                Insights.Report(ex, ReportSeverity.Error);
                Debug.WriteLine("Unable to load movies: " + ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task addMovie(Movie movie)
		{
			var vm = new MovieViewModel();
			await vm.Init(movie);

			Movies.Add(vm);
		}

		private RelayCommand<string> getImageCommand;
		public ICommand GetImageCommand
		{
			get { return getImageCommand ?? (getImageCommand = new RelayCommand<string>((item) => ExecuteGetImageCommand(item))); }
		}

        [Insights]
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
                Insights.Report(ex);
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

        [Insights]
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
                Insights.Report(ex);
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

