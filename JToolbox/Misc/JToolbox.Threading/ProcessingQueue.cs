using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JToolbox.Threading
{
    public abstract class ProcessingQueue<TItem, TResult>
    {
        public int TasksCount { get; set; }
        public bool StopOnFirstException { get; set; }

        private BlockingCollection<ProcessingQueueItem<TItem, TResult>> InitializeCollection(List<ProcessingQueueItem<TItem, TResult>> items)
        {
            var collection = new BlockingCollection<ProcessingQueueItem<TItem, TResult>>();
            foreach (var item in items)
            {
                item.Clear();
                collection.Add(item);
            }
            return collection;
        }

        private int GetTasksCount(List<ProcessingQueueItem<TItem, TResult>> item)
        {
            if (TasksCount > 0)
            {
                return TasksCount;
            }
            else
            {
                return item.Count;
            }
        }

        public Task<List<ProcessingQueueItem<TItem, TResult>>> Run(List<TItem> items, CancellationToken cancellationToken = default)
        {
            return Run(items.ConvertAll(s => new ProcessingQueueItem<TItem, TResult>(s)), cancellationToken);
        }

        public async Task<List<ProcessingQueueItem<TItem, TResult>>> Run(List<ProcessingQueueItem<TItem, TResult>> items, CancellationToken cancellationToken = default)
        {
            var collection = InitializeCollection(items);
            var tasksCount = GetTasksCount(items);
            var tasks = new List<Task>();
            using (var internalCancellationTokenSource = new CancellationTokenSource())
            {
                var internalToken = internalCancellationTokenSource.Token;
                for (var i = 0; i < tasksCount; i++)
                {
                    var task = Task.Run(async () =>
                    {
                        while (!internalToken.IsCancellationRequested
                            && !cancellationToken.IsCancellationRequested)
                        {
                            collection.TryTake(out ProcessingQueueItem<TItem, TResult> item);
                            if (item != null)
                            {
                                if (!await RunProcessItem(item, internalCancellationTokenSource))
                                {
                                    return;
                                }

                                await RunReportProgress(item);
                            }
                            else if (item == null && collection.Count == 0)
                            {
                                return;
                            }
                        }
                    }, cancellationToken);
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
            }
            return items;
        }

        private async Task<bool> RunProcessItem(ProcessingQueueItem<TItem, TResult> item, CancellationTokenSource internalCancellationTokenSource)
        {
            item.Processed = true;
            try
            {
                item.Output = await ProcessItem(item.Input);
                return true;
            }
            catch (Exception exc)
            {
                item.Exception = exc;
                if (StopOnFirstException)
                {
                    if (!internalCancellationTokenSource.IsCancellationRequested)
                    {
                        internalCancellationTokenSource.Cancel();
                    }
                    return false;
                }
                return true;
            }
        }

        private async Task RunReportProgress(ProcessingQueueItem<TItem, TResult> item)
        {
            try
            {
                await ReportProgress(item);
            }
            catch { }
        }

        public abstract Task<TResult> ProcessItem(TItem item);

        public abstract Task ReportProgress(ProcessingQueueItem<TItem, TResult> item);
    }
}