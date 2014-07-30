using System;

namespace Mymdb.Core
{
	[PropertyChanged.ImplementPropertyChanged]
	public class ViewModelBase
	{
		private bool isBusy;
		public Action<bool> IsBusyChanged { get; set; }
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				if (IsBusyChanged != null)
					IsBusyChanged(isBusy);
			}
		}
	}
}

