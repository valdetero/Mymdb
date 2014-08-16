using System;
using Xamarin.Forms;

namespace Mymdb.UI
{
	public class ListItemTemplate : ViewCell
	{
		public ListItemTemplate()
		{
			var photo = new Image { 
				HeightRequest = 44, 
				WidthRequest = 44 
			};

			photo.SetBinding<Core.ViewModels.MovieViewModel>(Image.SourceProperty, x => x.Photo);

			var nameLabel = new Label { 
				YAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize (NamedSize.Medium)
			};

			nameLabel.SetBinding<Core.ViewModels.MovieViewModel>(Label.TextProperty, x => x.Title);

			var information = new StackLayout {
				Padding = new Thickness (5, 0, 0, 0),
				Orientation = StackOrientation.Horizontal,
				Children = { nameLabel }
			};

			View = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Children = { photo, information }
			};
		}
	}
}

