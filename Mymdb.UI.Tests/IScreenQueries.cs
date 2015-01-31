using System;
using Xamarin.UITest.Queries;

namespace Mymdb.UI.Tests
{
	public interface IScreenQueries
	{
		Func<AppQuery, AppQuery> MovieNameView { get; }
	}
}

