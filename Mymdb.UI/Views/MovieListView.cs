﻿using System;
using System.Linq;
using Xamarin.Forms;

namespace Mymdb.UI
{
	public class MovieListView : ContentPage
	{
		private ListView listView;
		private Core.ViewModels.MoviesViewModel viewModel;

		public MovieListView()
		{
			listView = new ListView () {
				IsGroupingEnabled = false,
				ItemTemplate = new DataTemplate (typeof(ListItemTemplate)),
			};

			listView.ItemTapped += OnItemSelected;
			Content = listView;
			Title = "Movies";

			viewModel = IoC.ServiceContainer.Resolve<Core.ViewModels.MoviesViewModel>();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			if(viewModel == null || viewModel.Movies == null || viewModel.Movies.Count == 0) 
			{
				await viewModel.ExecuteLoadMoviesCommand();
				listView.ItemsSource = viewModel.Movies;
			}
		}

		protected async void OnItemSelected(object sender, ItemTappedEventArgs e)
		{
			var item = e.Item as Core.ViewModels.MovieViewModel;

			var selectedMovie = new MovieView 
			{ 
				BindingContext = item
			};

			await Navigation.PushAsync(selectedMovie);
		}
	}
}