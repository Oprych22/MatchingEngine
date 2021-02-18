using System;
using System.Collections.Generic;
using System.Text;

namespace MatchingEngine
{
    public struct Trade
    {
        public double Price;
        public double Volume;
        public int aggressorCounterparty;
        public int otherCounterparty;
        public int commodityID;
        public DateTime tradeTime;
    }
}
