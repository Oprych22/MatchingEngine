using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngine
{
    public class MatchingEngine : IMatchingEngine
    {
        private readonly LinkedList<Order> _bids;
        private readonly LinkedList<Order> _offers;
        private readonly List<Trade> _trades;
        private readonly int _commodityId;
        private readonly ConcurrentQueue<Order> _incomingOrders;
        private bool _running = false;
        
        
        public MatchingEngine(int commodityId)
        {
            _incomingOrders = new ConcurrentQueue<Order>();
            _bids = new LinkedList<Order>();
            _offers = new LinkedList<Order>();
            _trades = new List<Trade>();
            _commodityId = commodityId;
        }

        public void DequeueOrders()
        {
            if (_running == true)
            {
                throw new Exception("Matching Engine Already Running");
            }

            _running = true;
            while (_incomingOrders.TryDequeue(out var o))
            {
                ProcessOrders(o);
            }

            _running = false;
        }

        public void AddOrder(object sender, Order o)
        {
            _incomingOrders.Enqueue(o);
        }

        private void ProcessOrders(Order o)
        {
            if (o.BidOffer == BidOffer.Bid)
            {
                if (_bids.Count == 0)
                {
                    _bids.AddFirst(o);
                }
                else if (_bids.Last != null && _bids.Last.Value.Price >= o.Price)
                {
                    _bids.AddLast(o);
                }
                else
                {
                    var bid = _bids.First;
                    while (bid != null && bid.Value.Price > o.Price)
                    {
                        bid = bid.Next;
                    }
                    if (bid != null)
                        _bids.AddBefore(bid, o);
                }

            }
            else
            {
                if (_offers.Count == 0)
                {
                    _offers.AddFirst(o);
                }
                else if (_offers.Last != null && _offers.Last.Value.Price <= o.Price)
                {
                    _offers.AddLast(o);
                }
                else
                {
                    var offer = _offers.First;
                    while (offer != null && offer.Value.Price < o.Price)
                    {
                        offer = offer.Next;
                    }
                    if (offer != null)
                        _offers.AddBefore(offer, o);
                }
            }
            OrdersOut.AddOrder(new OrderEventArgs(o, OrderOperation.Add));
            
            if (_bids.Count > 0 && _offers.Count > 0)
                MatchOrders();
        }

        private void MatchOrders()
        {
            var bid = _bids.First;
            var offer = _offers.First;

            while (bid != null && offer != null)
            {
                if (bid.Value.CounterpartyId == offer.Value.CounterpartyId)
                {
                    if (bid.Value.LastUpdateTime >= offer.Value.LastUpdateTime)
                    {
                        offer = offer.Next;
                    }
                    else
                    {
                        bid = bid.Next;
                    }    
                }
                else
                {
                    var bidAggressor = bid.Value.LastUpdateTime >= offer.Value.LastUpdateTime ? true : false;
                    var price = bidAggressor ? offer.Value.Price : bid.Value.Price;
                    if (bid.Value.Price >= offer.Value.Price)
                    {
                        var bidVol = bid.Value.Volume;
                        var offerVol = offer.Value.Volume;

                        if (bidVol > offerVol)
                        {
                            BidVolGreater(ref bid, ref offer, price, bidAggressor);
                        }
                        else if (offerVol > bidVol)
                        {
                            OfferVolGreater(ref bid, ref offer, price, bidAggressor);
                        }
                        else if (offerVol.Equals(bidVol))
                        {
                           VolsEqual(ref bid, ref offer, price, bidAggressor);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void BidVolGreater(ref LinkedListNode<Order> bid, ref LinkedListNode<Order> offer, double price, bool bidAggressor)
        {
            _trades.Add(new Trade() { Price = price, Volume = offer.Value.Volume, tradeTime = DateTime.UtcNow , commodityID = _commodityId, aggressorCounterparty = bidAggressor ? bid.Value.CounterpartyId : offer.Value.CounterpartyId, otherCounterparty = bidAggressor ? offer.Value.CounterpartyId : bid.Value.CounterpartyId});
            
            var nextOffer = offer.Next;
            var newBid = new LinkedListNode<Order>(new Order { Price = bid.Value.Price, Volume = bid.Value.Volume - offer.Value.Volume, BidOffer = BidOffer.Bid, LastUpdateTime = DateTime.Now, CommodityId =  _commodityId, CounterpartyId = bid.Value.CounterpartyId});
            bid.Replace(newBid);
            bid = newBid;

            RemoveOrder(offer);
            offer = nextOffer;
        }
        
        private void OfferVolGreater(ref LinkedListNode<Order> bid, ref LinkedListNode<Order> offer, double price, bool bidAggressor)
        {
            _trades.Add(new Trade() { Price = price, Volume = bid.Value.Volume, tradeTime = DateTime.UtcNow , commodityID = _commodityId,  aggressorCounterparty = bidAggressor ? bid.Value.CounterpartyId : offer.Value.CounterpartyId, otherCounterparty = bidAggressor ? offer.Value.CounterpartyId : bid.Value.CounterpartyId});
            
            var nextBid = bid.Next;
            var newOffer = new LinkedListNode<Order>(new Order { Price = offer.Value.Price, Volume = offer.Value.Volume - bid.Value.Volume, BidOffer = BidOffer.Offer, LastUpdateTime = DateTime.Now, CommodityId =  _commodityId, CounterpartyId = offer.Value.CounterpartyId });
            offer.Replace(newOffer);
            offer = newOffer;

            RemoveOrder(bid);
            bid = nextBid;
        }
        
        private void VolsEqual(ref LinkedListNode<Order> bid, ref LinkedListNode<Order> offer, double price, bool bidAggressor)
        {
            _trades.Add(new Trade() { Price = price, Volume = offer.Value.Volume, tradeTime = DateTime.UtcNow, commodityID = _commodityId,  aggressorCounterparty = bidAggressor ? bid.Value.CounterpartyId : offer.Value.CounterpartyId, otherCounterparty = bidAggressor ? offer.Value.CounterpartyId : bid.Value.CounterpartyId});
            
            var newBid = bid.Next;
            var newOffer = offer.Next;
            RemoveOrder(bid);
            RemoveOrder(offer);
            bid = newBid;
            offer = newOffer;
        }

        public IEnumerable<Order> AllOrders()
        {
            return _bids.Union(_offers).ToList();
        }
        public IEnumerable<Trade> AllTrades()
        {
            return _trades;
        }
        public int BidCount()
        {
            return _bids.Count;
        }
        public int OfferCount()
        {
            return _offers.Count;
        }
        public int TradeCount()
        {
            return _trades.Count;
        }

        // private void CreateTrade(Order o1, Order o2)
        // {
        //     
        //     _trades.Add(new Trade() { Price = price, Volume = offerVol, tradeTime = DateTime.UtcNow });
        // }
        
        public void PrintOrders()
        {
            Console.Out.WriteLine("***Bids***");
            foreach(var bid in _bids)
            {
                Console.Out.WriteLine($"Bid - t: {bid.LastUpdateTime} - p: {bid.Price} - v: {bid.Volume}");
            }
            Console.Out.WriteLine("***Offers***");
            foreach (var offer in _offers)
            {
                Console.Out.WriteLine($"Offer - t: {offer.LastUpdateTime} - p: {offer.Price} - v: {offer.Volume}");
            }
        }

        public void PrintTrades()
        {
            Console.Out.WriteLine("***Trades***");
            var newvol = 0.0;
            foreach (var trade in _trades)
            {
                Console.Out.WriteLine($"Trade - t: {trade.tradeTime} - p: {trade.Price} - v: {trade.Volume}");
                newvol += trade.Volume;
            }
        }

        private void RemoveOrder(LinkedListNode<Order> o)
        {
            if (o.Value.BidOffer == BidOffer.Bid)
            {
                _bids.Remove(o);
            }
            else
            {
                _offers.Remove(o);
            }
            OrdersOut.AddOrder(new OrderEventArgs(o.Value, OrderOperation.Delete));
        }

        private void AmendOrder(LinkedListNode<Order> o1, LinkedListNode<Order> o2)
        {
            o1.Replace(o2);
            OrdersOut.AddOrder(new OrderEventArgs(o2.Value, OrderOperation.Amend));
        }
    }
}
