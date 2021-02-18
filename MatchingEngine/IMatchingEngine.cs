using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MatchingEngine
{
    public interface IMatchingEngine
    {
        public void PrintOrders();
        public void PrintTrades();

        public int TradeCount();

        public int OfferCount();

        public int BidCount();
        
        public IEnumerable<Order> AllOrders();

        public IEnumerable<Trade> AllTrades();

        public void DequeueOrders();

        public void AddOrder(object obj, Order o);

    }
}