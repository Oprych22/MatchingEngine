using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace MatchingEngine
{
    public static class MatchingEngines
    {
        private static readonly Dictionary<int, IMatchingEngine> _matchingEngines;
        private static readonly Dictionary<int, Task> tasks;
        private static long _orderId;

        static MatchingEngines()
        {
            _matchingEngines = new Dictionary<int, IMatchingEngine>();
            tasks = new Dictionary<int, Task>();
            _orderId = 0;
        }

        private static long GetOrderId()
        {
            return _orderId++;
        }

        public static void AddOrder(Order o)
        {
            o.OrderId = GetOrderId();
            var action = new Action(() =>   
            {   
                _matchingEngines[o.CommodityId].DequeueOrders();
            });
            if (_matchingEngines.TryAdd(o.CommodityId, new MatchingEngine(o.CommodityId))) ;
            {
                _matchingEngines[o.CommodityId].AddOrder(null ,o);
            }

            if (tasks.TryAdd(o.CommodityId, new Task(action)))
            {
                tasks[o.CommodityId].Start();
            }
            else if (tasks[o.CommodityId].IsCompleted)
            {
                tasks[o.CommodityId] = new Task(action);
                tasks[o.CommodityId].Start();
            }
            
        }

        public static List<Order> AllExistingOrders()
        {
            List<Order> orders = new List<Order>();
            foreach (var o in _matchingEngines.Values)
            {
                orders.AddRange(o.AllOrders());
            }

            return orders;
        }
        
        public static IEnumerable<Trade> AllTrades()
        {
            var trades = new List<Trade>();
            foreach (var o in _matchingEngines.Values)
            {
                trades.AddRange(o.AllTrades());
            }

            return trades;
        }
    }
}