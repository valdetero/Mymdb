using System;
using Xamarin.Forms;

namespace Mymdb.UI
{
	public class App : Application
	{
		public App()
		{
			var movieList = new MovieListView ();

			MainPage = new NavigationPage (movieList);
		}
	}
}

