using System;

namespace MatchingEngine
{
    public class OrderEventArgs : EventArgs
    {
        
        public OrderEventArgs(Order o, OrderOperation oo)
        {
            Order = o;
            OrderOperation = oo;
        }
        
        public Order Order { get; set; }

        public OrderOperation OrderOperation { get; set; }
    }
    
}