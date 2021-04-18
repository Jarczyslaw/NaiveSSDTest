using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace JToolbox.Threading
{
    public class ProducerConsumer<T>
    {
        public delegate void ItemHandled(T item);

        public delegate void ExceptionOccured(Exception exc);

        public event ItemHandled OnItemHandled = delegate { };

        public event ExceptionOccured OnExceptionOccured = delegate { };

        private readonly BlockingCollection<T> items = new BlockingCollection<T>();
        private Task task;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public ProducerConsumer()
        {
            Start();
        }

        public Func<T, Task> Handler { get; set; }

        public int PendingTasks => items.Count;

        private void Start()
        {
            var token = tokenSource.Token;
            task = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var item = items.Take(token);
                        if (Handler != null)
                        {
                            await Handler(item);
                            OnItemHandled(item);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exc)
                    {
                        OnExceptionOccured(exc);
                    }
                }
            }, token);
        }

        public void Add(T item)
        {
            items.Add(item);
        }

        public async Task Cancel()
        {
            if (!tokenSource.IsCancellationRequested)
            {
                tokenSource.Cancel();
                await Task.WhenAll(task);
            }
        }
    }
}