using System;

namespace MatchingEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            MatchingEngine me = new MatchingEngine(2);
            Order o1 = new Order() { BidOffer = BidOffer.Bid, Price = 5 };
            Order o2 = new Order() { BidOffer = BidOffer.Bid, Price = 4 };
            Order o3 = new Order() { BidOffer = BidOffer.Bid, Price = 6 };
            Order o4 = new Order() { BidOffer = BidOffer.Bid, Price = 3 };
            Order o5 = new Order() { BidOffer = BidOffer.Bid, Price = 4 };
            Order o6 = new Order() { BidOffer = BidOffer.Bid, Price = 2 };


            Order o7 = new Order() { BidOffer = BidOffer.Offer, Price = 5 };
            Order o8 = new Order() { BidOffer = BidOffer.Offer, Price = 4 };
            Order o9 = new Order() { BidOffer = BidOffer.Offer, Price = 6 };
            Order o10 = new Order() { BidOffer = BidOffer.Offer, Price = 3 };
            Order o11= new Order() { BidOffer = BidOffer.Offer, Price = 4 };
            Order o12 = new Order() { BidOffer = BidOffer.Offer, Price = 2 };

            //me.AddOrder(o1);
            //me.AddOrder(o2);
            //me.AddOrder(o3);
            //me.AddOrder(o4);
            //me.AddOrder(o5);
            //me.AddOrder(o6);
            //me.AddOrder(o7);
            //me.AddOrder(o8);
            //me.AddOrder(o9);
            //me.AddOrder(o10);
            //me.AddOrder(o11);
            //me.AddOrder(o12);

            //for (int i = 0; i < 10000; i++)
            //{
            //    me.AddOrder(new Order() { BidOffer = getRandBidOffer(), Price = r.NextDouble(), Volume = r.Next(1, 100), lastUpdateTime = DateTime.UtcNow });
            //}

           

            me.PrintOrders();

            me.PrintTrades();

            Console.ReadLine();

        }
        public static BidOffer getRandBidOffer()
        {

            Random r = new Random();
            return r.Next(0, 2) == 1 ? BidOffer.Offer : BidOffer.Bid;
        }
    }
}
