using System;
using Xamarin.UITest.Queries;

namespace Mymdb.UI.Tests
{
	public class iOSQueries : IScreenQueries
	{
		public Func<AppQuery, AppQuery> MovieNameView { get { return x => x.Marked("lblName"); } }
	}
}

