using System;
using System.Threading;

namespace JToolbox.Threading
{
    public sealed class QueuedLock
    {
        private static readonly object innerLock = new object();
        private volatile int ticketsCount;
        private volatile int ticketToRide = 1;

        public void Enter()
        {
            int myTicket = Interlocked.Increment(ref ticketsCount);
            Monitor.Enter(innerLock);
            while (true)
            {
                if (myTicket == ticketToRide)
                {
                    return;
                }
                else
                {
                    Monitor.Wait(innerLock);
                }
            }
        }

        public void Exit()
        {
            Interlocked.Increment(ref ticketToRide);
            Monitor.PulseAll(innerLock);
            Monitor.Exit(innerLock);
        }

        public void LockedAction(Action action)
        {
            try
            {
                Enter();
                action();
            }
            finally
            {
                Exit();
            }
        }
    }
}