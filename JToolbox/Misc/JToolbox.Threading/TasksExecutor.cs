using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace JToolbox.Threading
{
    public delegate void TasksExecutorStateChanged(TasksExecutorState state);

    public class TasksExecutor
    {
        private readonly BlockingCollection<ITask> tasks = new BlockingCollection<ITask>();
        private readonly ConcurrentDictionary<int, Thread> threads = new ConcurrentDictionary<int, Thread>();
        private readonly QueuedLock queuedLock = new QueuedLock();

        private int workingThreads;
        private int waitingThreads;
        private int maxThreads = 4;
        private int minThreads = 1;
        private TasksExecutorState lastState;

        public event TasksExecutorStateChanged OnTasksExecutorStateChanged;

        public int WorkingThreads => workingThreads;

        public int WaitingThreads => waitingThreads;

        public int IdleThreads => threads.Count - workingThreads;

        public int MaxThreads
        {
            get { return maxThreads; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Invalid maxThreads value");
                }
                maxThreads = value;
            }
        }

        public int MinThreads
        {
            get { return minThreads; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Invalid minThreads value");
                }
                minThreads = value;
            }
        }

        public TimeSpan? ThreadsTimeout { get; set; }

        public bool Working => workingThreads != 0;

        private void OnStateChanged()
        {
            queuedLock.LockedAction(() =>
            {
                var newState = GetState();
                if (lastState?.Compare(newState) != true)
                {
                    OnTasksExecutorStateChanged?.Invoke(newState);
                }
                lastState = newState;
            });
        }

        public TasksExecutorState GetState()
        {
            return new TasksExecutorState
            {
                WaitingThreads = waitingThreads,
                WorkingThreads = workingThreads,
                IdleThreads = IdleThreads,
                PendingTasks = tasks.Count,
                Threads = threads.Count
            };
        }

        public void Add(ITask task)
        {
            if (tasks.Count + 1 > waitingThreads && threads.Count < maxThreads)
            {
                CreateNewThread();
            }
            tasks.Add(task);
            OnStateChanged();
        }

        private void CreateNewThread()
        {
            var thread = new Thread(PoolForTasks)
            {
                IsBackground = true,
                Name = $"{nameof(TasksExecutor)}_Thread{threads.Count + 1}"
            };
            threads.TryAdd(thread.ManagedThreadId, thread);
            thread.Start(thread);
        }

        private void IncrementCounter(ref int counter)
        {
            Interlocked.Increment(ref counter);
            OnStateChanged();
        }

        private void DecrementCounter(ref int counter)
        {
            Interlocked.Decrement(ref counter);
            OnStateChanged();
        }

        private void PoolForTasks(object state)
        {
            var thread = state as Thread;
            var continueWaiting = false;
            try
            {
                while (true)
                {
                    if (!continueWaiting)
                    {
                        IncrementCounter(ref waitingThreads);
                    }
                    continueWaiting = false;

                    ITask task;
                    bool taken;
                    if (ThreadsTimeout == null)
                    {
                        task = tasks.Take();
                        taken = true;
                    }
                    else
                    {
                        taken = tasks.TryTake(out task, ThreadsTimeout.Value);
                    }

                    if (taken && task == null)
                    {
                        DecrementCounter(ref waitingThreads);
                        return;
                    }
                    else if (taken && task != null)
                    {
                        DecrementCounter(ref waitingThreads);
                        ExecuteTask(task);
                    }
                    else
                    {
                        if (waitingThreads > minThreads)
                        {
                            DecrementCounter(ref waitingThreads);
                            return;
                        }
                        else
                        {
                            continueWaiting = true;
                        }
                    }
                }
            }
            finally
            {
                threads.TryRemove(thread.ManagedThreadId, out _);
                OnStateChanged();
            }
        }

        private void ExecuteTask(ITask task)
        {
            IncrementCounter(ref workingThreads);
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            try
            {
                task.Run(this);
            }
            catch (Exception exc)
            {
                exception = exc;
            }
            finally
            {
                task.Finish(this, exception, stopwatch.Elapsed);
                DecrementCounter(ref workingThreads);
            }
        }
    }
}