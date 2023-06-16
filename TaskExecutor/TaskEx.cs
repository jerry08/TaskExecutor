﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TaskExecutor;

public static class TaskEx
{
    private static readonly TaskFactory _myTaskFactory = new(CancellationToken.None,
        TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        return _myTaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }

    public static void RunSync(Func<Task> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        _myTaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }

    public static Task<TResult[]> Run<TResult>(
        IEnumerable<Func<Task<TResult>>> actions,
        int maxCount = 1,
        IProgress<double>? progress = null)
    {
        return InternalRun(actions.ToArray(), maxCount, progress);
    }

    public static Task Run(
        IEnumerable<Func<Task>> actions,
        int maxCount = 1,
        IProgress<double>? progress = null)
    {
        return InternalRun(actions.ToArray(), maxCount, progress);
    }

    private static Task<TResult[]> InternalRun<TResult>(
        Func<Task<TResult>>[] actions,
        int maxCount = 1,
        IProgress<double>? progress = null)
    {
        var semaphore = new ResizableSemaphore
        {
            MaxCount = maxCount
        };

        var newTasks = Enumerable.Range(0, actions.Length).Select(i =>
            Task.Run(async () =>
            {
                using var access = await semaphore.AcquireAsync();
                var result = await actions[i]();
                progress?.Report(i);
                return result;
            })).ToArray();

        return Task.WhenAll(newTasks);
    }

    private static Task InternalRun(
        Func<Task>[] actions,
        int maxCount = 1,
        IProgress<double>? progress = null)
    {
        var semaphore = new ResizableSemaphore
        {
            MaxCount = maxCount
        };

        var newTasks = Enumerable.Range(0, actions.Length).Select(i =>
            Task.Run(async () =>
            {
                using var access = await semaphore.AcquireAsync();
                await actions[i]();
                progress?.Report(i);
            })).ToArray();

        return Task.WhenAll(newTasks);
    }
}