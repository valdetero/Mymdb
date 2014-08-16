using System;
using Xamarin.Forms;

namespace Mymdb.UI
{
	public static class App
	{
		public static Page GetMainPage ()
		{
			var movieList = new MovieListView ();

			return new NavigationPage (movieList);
		}
	}
}

