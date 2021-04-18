using System;
using System.Collections.Generic;
using System.Linq;

namespace JToolbox.Threading
{
    public static class ProcessingQueueItemsListExtensions
    {
        public static bool IsSuccess<TItem, TResult>(this List<ProcessingQueueItem<TItem, TResult>> @this)
        {
            return @this.All(s => s.Processed && s.Exception == null);
        }

        public static List<TResult> GetResults<TItem, TResult>(this List<ProcessingQueueItem<TItem, TResult>> @this)
        {
            return @this.Where(s => s.Processed)
                .Select(s => s.Output)
                .ToList();
        }

        public static Exception GetFirstException<TItem, TResult>(this List<ProcessingQueueItem<TItem, TResult>> @this)
        {
            return @this.FirstOrDefault(s => s.Processed && s.Exception != null)?.Exception;
        }

        public static List<Exception> GetAllExceptions<TItem, TResult>(this List<ProcessingQueueItem<TItem, TResult>> @this)
        {
            return @this.Where(s => s.Processed && s.Exception != null)
                .Select(s => s.Exception)
                .ToList();
        }

        public static List<ProcessingQueueItem<TItem, TResult>> GetUnprocessedItems<TItem, TResult>(this List<ProcessingQueueItem<TItem, TResult>> @this)
        {
            return @this.Where(s => !s.Processed).ToList();
        }

        public static List<ProcessingQueueItem<TItem, TResult>> GetProcessedItems<TItem, TResult>(this List<ProcessingQueueItem<TItem, TResult>> @this)
        {
            return @this.Where(s => s.Processed).ToList();
        }
    }
}