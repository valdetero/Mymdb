/*
* 	Taken from http://stackoverflow.com/a/21011273/1134836 
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mymdb.Core
{
	public sealed class AsyncLock
	{
		private readonly Task<IDisposable> releaserTask;
		private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
		private readonly DisposableScope releaser;

		public AsyncLock()
		{
			releaser = new DisposableScope(() => semaphore.Release());
			releaserTask = Task.FromResult(releaser as IDisposable);
		}

		public IDisposable Lock()
		{
			semaphore.Wait();
			return releaser;
		}

		public Task<IDisposable> LockAsync()
		{
			var waitTask = semaphore.WaitAsync();
			return waitTask.IsCompleted
				? releaserTask
				: waitTask.ContinueWith(
					(_, releaser) => (IDisposable)releaser,
					this.releaser,
					CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}
	}

	public sealed class DisposableScope : IDisposable
	{
		private readonly Action closeScopeAction;
		public DisposableScope(Action closeScopeAction)
		{
			this.closeScopeAction = closeScopeAction;
		}

		public void Dispose()
		{
			if(closeScopeAction != null)
				closeScopeAction();
		}
	}
}

