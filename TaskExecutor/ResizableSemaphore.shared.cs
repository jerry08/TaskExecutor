﻿using System;
using System.Threading;

namespace TaskExecutor;

public partial class ResizableSemaphore : IDisposable
{
    private readonly object _lock = new();
    private readonly CancellationTokenSource _cts = new();

    private bool _isDisposed;
    private int _maxCount = int.MaxValue;
    private int _count;

    public bool IsBusy => MaxCount > 0;

    public int MaxCount
    {
        get
        {
            lock (_lock)
            {
                return _maxCount;
            }
        }
        set
        {
            lock (_lock)
            {
                _maxCount = value;
                Refresh();
            }
        }
    }

    public IDisposable Acquire() => AcquireAsync().GetAwaiter().GetResult();

    public void Release()
    {
        lock (_lock)
        {
            _count--;
            Refresh();
        }
    }

    public void Dispose()
    {
        _isDisposed = true;
        _cts.Cancel();
        _cts.Dispose();
    }
}

public partial class ResizableSemaphore
{
    private class AcquiredAccess : IDisposable
    {
        private readonly ResizableSemaphore _semaphore;

        public AcquiredAccess(ResizableSemaphore semaphore) =>
            _semaphore = semaphore;

        public void Dispose() => _semaphore.Release();
    }
}