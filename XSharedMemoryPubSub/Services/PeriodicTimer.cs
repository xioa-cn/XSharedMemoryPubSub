using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

namespace XSharedMemoryPubSub.Services
{
    public class PeriodicTimer : IDisposable
    {
        private readonly System.Timers.Timer _timer;
        private TaskCompletionSource<bool> _currentTcs;
        private readonly object _lock = new object();
        private bool _disposed;

        public PeriodicTimer(TimeSpan period)
        {
            if (period <= TimeSpan.Zero)
                throw new ArgumentException("Period must be positive.", nameof(period));

            _timer = new System.Timers.Timer(period.TotalMilliseconds);
            _currentTcs = new TaskCompletionSource<bool>();

            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_disposed) return;

                var tcs = _currentTcs;
                _currentTcs = new TaskCompletionSource<bool>();
                tcs.TrySetResult(true);
            }
        }

        public Task<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                if (_disposed)
                    return Task.FromResult(false);

                var registration = cancellationToken.Register(() =>
                {
                    lock (_lock)
                    {
                        _currentTcs.TrySetCanceled(cancellationToken);
                    }
                });

                return _currentTcs.Task.ContinueWith(t =>
                {
                    registration.Dispose();
                    if (t.IsCanceled)
                        throw new OperationCanceledException(cancellationToken);
                    return t.Result;
                }, TaskScheduler.Default);
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed) return;
                _disposed = true;

                _timer.Stop();
                _timer.Dispose();
                _currentTcs.TrySetResult(false);
            }
        }
    }
}
