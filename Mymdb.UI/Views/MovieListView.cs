using System;
using System.Linq;
using Mymdb.Core.ViewModels;
using Xamarin.Forms;

namespace Mymdb.UI
{
	public class MovieListView : ContentPage
	{
		private ListView listView;
		private MoviesViewModel viewModel;

		public MovieListView()
		{
			listView = new ListView () {
				IsGroupingEnabled = false,
				ItemTemplate = new DataTemplate (typeof(ListItemTemplate)),
				StyleId = "lstMovieList"
			};

			listView.ItemTapped += OnItemSelected;
			Content = listView;
			Title = "Movies";

			viewModel = IoC.ServiceContainer.Resolve<MoviesViewModel>();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			if(viewModel == null || viewModel.Movies == null || viewModel.Movies.Count == 0) 
			{
                await viewModel.ExecuteLoadMoviesCommand();
				listView.ItemsSource = viewModel.Movies;
			}
			listView.SelectedItem = null;
		}

		protected async void OnItemSelected(object sender, ItemTappedEventArgs e)
		{
			var item = e.Item as MovieViewModel;

			var selectedMovie = new MovieView 
			{ 
				BindingContext = item
			};

			await Navigation.PushAsync(selectedMovie);
		}
	}
}