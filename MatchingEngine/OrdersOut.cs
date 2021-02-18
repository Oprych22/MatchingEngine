using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MatchingEngine
{
    public static class OrdersOut
    {
        private static readonly ConcurrentQueue<OrderEventArgs> Orders;
        private static Task _task;
        private static readonly Action Action;

        static OrdersOut()
        {
            Orders = new ConcurrentQueue<OrderEventArgs>();
            Action = DequeueOrders;
            _task = new Task(Action);
        }


        public static void AddOrder(OrderEventArgs o)
        {
            Orders.Enqueue(o);
            ProcessOrders();
        }

        public static void ProcessOrders()
        {
            if (_task.IsCompleted)
            {
                _task = new Task(Action);
            }
            _task.Start();
        }

        public static void DequeueOrders()
        {
            while (Orders.TryDequeue(out var o))
            {
                
            }
        }

    }
}