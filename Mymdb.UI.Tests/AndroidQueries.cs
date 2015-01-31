using System;
using Xamarin.UITest.Queries;

namespace Mymdb.UI.Tests
{
	public class AndroidQueries : IScreenQueries
	{
		public Func<AppQuery, AppQuery> MovieNameView { get { return x => x.Marked("lblName"); } }
	}
}

