
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using Mymdb.Core.ViewModels;

namespace Mymdb.iOS
{
	public partial class MoviesViewController : DialogViewController
	{
		private MoviesViewModel viewModel;

		public MoviesViewController() : base(UITableViewStyle.Plain, null)
		{
			viewModel = IoC.ServiceContainer.Resolve<MoviesViewModel>();
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if(viewModel.NeedsUpdate) {

				await viewModel.ExecuteLoadMoviesCommand(true);

				Root = new RootElement("Movies") {
					new Section() {
						viewModel.Movies.Select(movie => createElement(movie))
					}
				};
			}
		}

		Element createElement(MovieViewModel movie)
		{
			var image = (string.IsNullOrEmpty(movie.ImagePath)) ?
				(UIImage)null :
				new UIImage(NSData.FromUrl(new NSUrl(viewModel.ExecuteGetImageCommand(movie.ImagePath))));

			return new ImageStringElement(movie.Title, () =>
			{
				var controller = AppDelegate.Storyboard.InstantiateViewController("MovieViewController") as MovieViewController;
				controller.Init(movie.Id);
				this.NavigationController.PushViewController(controller, true);
			}, image);
		}
	}
}
