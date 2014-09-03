using System;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Controls;

namespace Mymdb.UI
{
	public class MovieView : ContentPage
	{
		private const int IMAGE_SIZE = 150;
		private Label favoriteLabel;
		private Switch favoriteSwitch;
		private Image photo;
		private WebView webView;
		private Label movieTitle;
		private ExtendedLabel imdbLink;
		private Core.ViewModels.MovieViewModel viewModel;

		public MovieView()
		{
			try{
			viewModel = BindingContext as Core.ViewModels.MovieViewModel;

			photo = new Image { WidthRequest = IMAGE_SIZE, HeightRequest = IMAGE_SIZE };
			photo.SetBinding (Image.SourceProperty, "Photo");
			photo.GestureRecognizers.Add(
				new TapGestureRecognizer((view, args) => 
				{
					Xamarin.Insights.Report(new ArgumentNullException());
				}));

			favoriteLabel = new Label { Text = "Favorite?" };

			favoriteSwitch = new Switch ();
			favoriteSwitch.SetBinding<Core.ViewModels.MovieViewModel>(Switch.IsToggledProperty, x => x.IsFavorite);

			webView = new WebView();
			imdbLink = new ExtendedLabel();
			imdbLink.IsUnderline = true;
			imdbLink.TextColor = Color.Blue;
			imdbLink.GestureRecognizers.Add(
				new TapGestureRecognizer((view, args) => 
					Navigation.PushAsync(new ContentPage { Content = webView })));

			var optionsView = new StackLayout { 
				VerticalOptions = LayoutOptions.StartAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { favoriteLabel, favoriteSwitch, imdbLink }
			};

			var headerView = new StackLayout {
				Padding = new Thickness (5, 20, 5, 0),
				HorizontalOptions = LayoutOptions.StartAndExpand,
				Orientation = StackOrientation.Horizontal,
				Children = { photo, optionsView }
			};

			movieTitle = new Label {
				XAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				IsVisible = Device.OS == TargetPlatform.WinPhone
			};

			var save = new Button {
				Text = "Save",
			};
			save.Clicked += (sender, e) => {
				viewModel.ExecuteSaveMovieCommand();
				Navigation.PopToRootAsync();
			};

			var delete = new Button {
				Text = "Delete",
				TextColor = Color.Red
			};
			delete.Clicked += (sender, e) => {
				viewModel.ExecuteDeleteMovieCommand(viewModel.Id);
				Navigation.PopToRootAsync();
			};

			var buttonView = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Children = { save, delete }
			};

			Content = new StackLayout {
				VerticalOptions = LayoutOptions.StartAndExpand,
				Children = { movieTitle, headerView, buttonView }
			};
			}
			catch(Exception ex) {
				Xamarin.Insights.Report(ex);
				Navigation.PopToRootAsync();
			}
		}

		protected async override void OnBindingContextChanged()
		{
			try{			
				base.OnBindingContextChanged();

				var movie = (Core.ViewModels.MovieViewModel)BindingContext;

				using(var handle = Xamarin.Insights.TrackTime("Loading movie")) 
				{
					await movie.Init(movie.Id);
				}
				viewModel = movie;

				photo.Source = viewModel.Photo;
				imdbLink.Text = viewModel.ImdbId;
				webView.Source = string.Format("http://m.imdb.com/title/{0}", viewModel.ImdbId);
			}
			catch(Exception ex) {
				Xamarin.Insights.Report(ex);
			}
		}
	}
}

