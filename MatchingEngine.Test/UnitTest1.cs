using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace MatchingEngine.Test
{
    [TestFixture]
    public class Tests
    {
        private Stopwatch _stopWatch;
        [SetUp]
        public void Setup()
        {
            _stopWatch = Stopwatch.StartNew(); 
        }
        
        [TearDown]
        public void Cleanup()
        {
            _stopWatch.Stop();
            Debug.WriteLine($"Excution time for {TestContext.CurrentContext.Test.Name} - {_stopWatch.ElapsedMilliseconds} ms" );
        }

        [Test]
        public void ConsumeAllBids()
        {

            var r = new Random();
            var vol = 0;
            for (var i = 0; i < 1000000; i++)
            {
                var newVol = r.Next(1, 20);
                var newCommodityId = r.Next(1, 200);
                vol += newVol;
                MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Bid, Price = r.Next(50, 100), Volume = newVol, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 });
            }

            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer, Price = 49, Volume = vol, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 2 });

            //Assert.AreEqual(0, MatchingEngines.BidCount() + _matchingEngine.OfferCount());
            //Assert.AreEqual(_matchingEngine.trades.Sum(x => x.Volume), vol);
        }

        [Test]
        public void ConsumeAllOrders()
        {

            var r = new Random();
            var vol = 0;
            for (var i = 0; i < 10000; i++)
            {
                var newVol = r.Next(1, 20);
                vol += newVol;
                MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer, Price = r.Next(1, 49), Volume = newVol, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 });
            }

            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Bid, Price = 51, Volume = vol, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 2 });

            //Assert.AreEqual(0, _matchingEngine.BidCount() + _matchingEngine.OfferCount());
            //Assert.AreEqual(_matchingEngine.trades.Sum(x => x.Volume), vol);
        }
        
        [Test]
        public void OrderThroughput()
        {

            var r = new Random();
            var vol = 0;
            for (var i = 0; i < 1000000; i++)
            {
                var newVol = r.Next(1, 20);
                var newCommodityId = r.Next(1, 500);
                vol += newVol;
                MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer, Price = r.Next(1, 49), Volume = newVol, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 , CommodityId = newCommodityId});
            }

            for (var i = 0; i < 1000000; i++)
            {
                var newVol = r.Next(1, 20);
                var newCommodityId = r.Next(1, 500);
                vol += newVol;
                MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Bid, Price = r.Next(1, 49), Volume = newVol, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 2,CommodityId = newCommodityId });
            }

            var trades = MatchingEngines.AllTrades();
            var orders = MatchingEngines.AllExistingOrders();

            //Assert.AreEqual(0, _matchingEngine.BidCount() + _matchingEngine.OfferCount());
            //Assert.AreEqual(_matchingEngine.trades.Sum(x => x.Volume), vol);
        }

        [Test]
        public void OrderCascade()
        {
            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer,Price = 40, Volume = 10, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 , CommodityId = 1});
            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer,Price = 39, Volume = 10, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 , CommodityId = 1});
            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer,Price = 36, Volume = 10, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 , CommodityId = 1});
            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer,Price = 38, Volume = 10, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 , CommodityId = 1});
            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer,Price = 32, Volume = 10, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 , CommodityId = 1});
            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer,Price = 35, Volume = 10, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 , CommodityId = 1});
            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Offer,Price = 37, Volume = 10, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 1 , CommodityId = 1});
            
            
            MatchingEngines.AddOrder(new Order() { BidOffer = BidOffer.Bid,Price = 41, Volume = 80, LastUpdateTime = DateTime.UtcNow, CounterpartyId = 2 , CommodityId = 1});

            var trades = MatchingEngines.AllTrades();
            var orders = MatchingEngines.AllExistingOrders();

        }
    }
}