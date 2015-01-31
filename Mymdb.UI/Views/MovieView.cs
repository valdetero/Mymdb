using System;
using Mymdb.Core.ViewModels;
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
		private MovieViewModel viewModel;

		public MovieView()
		{
			photo = new Image 
			{ 
				WidthRequest = IMAGE_SIZE, 
				HeightRequest = IMAGE_SIZE,
				StyleId = "imgPhoto"
			};
			photo.SetBinding (Image.SourceProperty, "Photo");

            photo.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { throw new ArgumentOutOfRangeException("Too many taps"); }),
                NumberOfTapsRequired = 1
            });

            favoriteLabel = new Label { Text = "Favorite?" };

			favoriteSwitch = new Switch 
			{
				StyleId = "swtFavorite"
			};
			favoriteSwitch.SetBinding(Switch.IsToggledProperty, "IsFavorite");

			webView = new WebView();
			imdbLink = new ExtendedLabel
			{
				StyleId = "lnkImdb"
			};
			imdbLink.IsUnderline = true;
			imdbLink.TextColor = Color.Blue;
            imdbLink.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => Navigation.PushAsync(new ContentPage { Content = webView }))
            });

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
				FontSize = Font.SystemFontOfSize(NamedSize.Large).FontSize,
				IsVisible = Device.OS == TargetPlatform.WinPhone,
				StyleId = "lblTitle",
				ClassId = "lblTitle"
			};

			var save = new Button {
				Text = "Save",
				StyleId = "btnSave"
			};
			save.Clicked += async (sender, e) => {
				await viewModel.ExecuteSaveMovieCommand();
                await Navigation.PopAsync();
            };

			var delete = new Button {
				Text = "Delete",
				TextColor = Color.Red,
				StyleId = "btnDelete"
			};
			delete.Clicked += async (sender, e) => {
				await viewModel.ExecuteDeleteMovieCommand(viewModel.Id);
                await Navigation.PopAsync();
            };

            var bad = new Button
            {
                Text = "Throw Exception",
                TextColor = Color.Lime,
				StyleId = "btnThrow"
            };
            bad.Clicked += (s, e) => {
                throw new NotImplementedException("Bad button");
            };

			var buttonView = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Children = { save, delete, bad }
			};

			Content = new StackLayout {
				VerticalOptions = LayoutOptions.StartAndExpand,
				Children = { movieTitle, headerView, buttonView }
			};
		}

        protected async override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var movie = (MovieViewModel)BindingContext;

            await movie.Init(movie.Id);
            viewModel = movie;

            photo.Source = viewModel.Photo;
            imdbLink.Text = viewModel.ImdbId;
            webView.Source = string.Format("http://m.imdb.com/title/{0}", viewModel.ImdbId);
        }
	}
}

