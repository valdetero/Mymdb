using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Mymdb.Interfaces;
using Mymdb.Model;
using Mymdb.Core.Services;
using Mymdb.Core.Helpers;
using Xamarin;
using Xamarin.Forms;

namespace Mymdb.Core.ViewModels
{
	public class MovieViewModel : ViewModelBase
	{
		private IMovieDatabaseService movieService;
		private IStorageService storageService;
		private Movie currentMovie;

		public MovieViewModel()
		{
			movieService = IoC.ServiceContainer.Resolve<IMovieDatabaseService>();
			storageService = IoC.ServiceContainer.Resolve<IStorageService>();
		}

		public async Task Init(int id)
		{
			if (id >= 0)
			{
				currentMovie = 	await storageService.GetMovie(id) ?? 
								await movieService.GetMovie(id);
			}
			else
			{
				currentMovie = null;
			}

			await Init();
		}

		public async Task Init(Movie movie)
		{
			currentMovie = movie;
			await Init();
		}

		public async Task Init()
		{
			if (currentMovie == null)
			{
				Title = string.Empty;
				ImdbId = string.Empty;
				IsFavorite = false;
				Runtime = 0;
				Id = -1;
				ImagePath = string.Empty;
				return;
			}
			else
			{
				Title = currentMovie.Title;
				ImdbId = currentMovie.ImdbId;
				IsFavorite = currentMovie.IsFavorite;
				Runtime = currentMovie.Runtime;
				ReleaseDate = currentMovie.ReleaseDate;
				Id = currentMovie.Id;
				ImagePath = movieService.CreateImageUrl(currentMovie.ImageUrl);
				if (currentMovie.Image == null || currentMovie.Image.Length == 0)
					currentMovie.Image = await movieService.DownloadImage(ImagePath);
				if (currentMovie.Image != null)
					Image = currentMovie.Image.ConvertToStream();
			}
		}

		public int Id { get; set; }
		public string Title { get; set; }
		public string ImdbId { get; set; }
		public int Runtime { get; set; }
		public DateTime ReleaseDate { get; set; }
		public bool IsFavorite { get; set; }
		public string ImagePath { get; set; }
		public System.IO.Stream Image { get; set; }
		public ImageSource Photo 
		{ 
			get {
				if(string.IsNullOrEmpty(ImagePath))
					return null;

				return Device.OnPlatform(
					UriImageSource.FromUri(new Uri(ImagePath)), 
					UriImageSource.FromUri(new Uri(ImagePath)),  
					UriImageSource.FromUri(new Uri(ImagePath))
				);
			} 
		}

		private RelayCommand saveMovieCommand;
		public ICommand SaveMovieCommand
		{
			get { return saveMovieCommand ?? (saveMovieCommand = new RelayCommand(async () => await ExecuteSaveMovieCommand())); }
		}

        [Insights]
		public async Task ExecuteSaveMovieCommand()
		{
			if (IsBusy)
				return;
			if (currentMovie == null)
				currentMovie = new Movie();

            Insights.Track("Saving movie: " + Title);

            currentMovie.Title = Title;
			currentMovie.ImdbId = ImdbId;
			currentMovie.Runtime = Runtime;
			currentMovie.IsFavorite = IsFavorite;
			currentMovie.ImagePath = ImagePath;
			if (Image != null)
				currentMovie.Image = Image.ConvertToByte();

			try
			{
				await storageService.SaveMovie(currentMovie);
				IoC.ServiceContainer.Resolve<MoviesViewModel>().NeedsUpdate = true;
			}
			catch (Exception ex)
			{
                Insights.Report(ex, ReportSeverity.Error);
				Debug.WriteLine("Unable to save movie: " + ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private RelayCommand<int> deleteMovieCommand;
		public ICommand DeleteMovieCommand
		{
			get { return deleteMovieCommand ?? (deleteMovieCommand = new RelayCommand<int>(async (id) => await ExecuteDeleteMovieCommand(id))); }
		}

        [Insights]
        public async Task ExecuteDeleteMovieCommand(int id)
		{
			if (IsBusy)
				return;

			IsBusy = true;

            Insights.Track("Deleting movie: " + Title);

            try
			{
				await storageService.DeleteMovie(id);
			}
			catch (Exception ex)
			{
                Insights.Report(ex, ReportSeverity.Error);
                Debug.WriteLine("Unable to delete movie: " + ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}