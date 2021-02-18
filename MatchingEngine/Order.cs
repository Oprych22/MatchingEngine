using System;
using System.Collections.Generic;
using System.Text;

namespace MatchingEngine
{
    public struct Order
    {
        public double Price;
        public double Volume;
        public int CounterpartyId;
        public long OrderId;
        public BidOffer BidOffer;
        public int CommodityId;
        public DateTime LastUpdateTime;
    }

    public enum BidOffer
    {
         Bid,
         Offer
    }
}
