namespace JToolbox.Threading
{
    public class TasksExecutorState
    {
        public int Threads { get; set; }
        public int WorkingThreads { get; set; }
        public int WaitingThreads { get; set; }

        public int IdleThreads { get; set; }
        public int PendingTasks { get; set; }

        public bool Compare(TasksExecutorState other)
        {
            return Threads == other.Threads
                && WorkingThreads == other.WorkingThreads
                && WaitingThreads == other.WaitingThreads
                && IdleThreads == other.IdleThreads
                && PendingTasks == other.PendingTasks;
        }
    }
}