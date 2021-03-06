
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using Mymdb.Core.ViewModels;
using Mymdb.Model;
using Mymdb.Interfaces;
using GoogleAnalytics.iOS;
using Acr.XamForms.UserDialogs;

namespace Mymdb.iOS
{
	public partial class MovieViewController : UIViewController
	{
		MovieViewModel viewModel;
		int id;
		IProgressDialog progress;

		public MovieViewController(IntPtr handle) : base(handle)
		{
		}
		public MovieViewController(int id)
		{
			viewModel = IoC.ServiceContainer.Resolve<MovieViewModel>();
			progress = IoC.ServiceContainer.Resolve<IProgressDialog>();
			this.id = id;
		}

		public void Init(int id)
		{
			viewModel = IoC.ServiceContainer.Resolve<MovieViewModel>();
			progress = IoC.ServiceContainer.Resolve<IProgressDialog>();
			this.id = id;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			GAI.SharedInstance.DefaultTracker.Set(GAIConstants.ClientId, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
			GAI.SharedInstance.DefaultTracker.Set(GAIConstants.ScreenName, "Movie View");
			GAI.SharedInstance.DefaultTracker.Send(GAIDictionaryBuilder.CreateScreenView().Build());

//			Segment.Analytics.Client.Screen(UIDevice.CurrentDevice.IdentifierForVendor.ToString(), "Movie View");
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if(id > 0) {
				progress.Show();
				await viewModel.Init(id);
				progress.Hide();
			}

			btnDelete.TouchUpInside += async (sender, e) =>
			{
				await viewModel.ExecuteDeleteMovieCommand(viewModel.Id);
				NavigationController.PopToRootViewController(true);
			};

			btnSave.TouchUpInside += async (sender, e) =>
			{
				viewModel.IsFavorite = swtFavorite.On;
				//TODO: set image
//				viewModel.Image = imgPoster.Image.;
				await viewModel.ExecuteSaveMovieCommand();
				NavigationController.PopToRootViewController(true);
			};

			btnImdb.TouchUpInside += (sender, e) =>
			{
				var webview = new UIWebView(this.View.Frame);
				webview.LoadRequest(new NSUrlRequest(new NSUrl(string.Format("http://m.imdb.com/title/{0}", viewModel.ImdbId))));
				this.NavigationController.PushViewController(new UIViewController() { webview }, true);
			};

			if (!string.IsNullOrEmpty(viewModel.ImdbId))
			{
				btnImdb.SetTitle(viewModel.ImdbId, UIControlState.Normal);
				btnImdb.TitleLabel.AttributedText = new NSAttributedString(viewModel.ImdbId, underlineStyle: NSUnderlineStyle.Single);
				btnImdb.Enabled = true;
			}

			lblTitle.Text = viewModel.Title;
			swtFavorite.On = viewModel.IsFavorite;

			setImage();
		}

		private void setImage()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine(viewModel.ImagePath);
				if (!string.IsNullOrEmpty(viewModel.ImagePath) && System.IO.File.Exists(viewModel.ImagePath))
				{
					imgPoster.Image = new UIImage(NSData.FromFile(viewModel.ImagePath));
				}
				else if (viewModel.Image != null && viewModel.Image.Length != 0)
					imgPoster.Image = new UIImage(NSData.FromStream(viewModel.Image));
			}
			//just don't load image
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}
	}
}

