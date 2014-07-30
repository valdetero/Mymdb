using System;

namespace Mymdb.Interfaces
{
	public interface IProgressIndicator
	{
		void Show(string message = "Loading...");
		void Hide();
	}
}

