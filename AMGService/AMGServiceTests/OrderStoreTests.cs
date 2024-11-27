namespace AMGServiceTests
{
    using AMGService;
    public class OrderStoreTests
    {
        string pathRoot = @"c:\AMG\";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void OrderStorePersistsNewOrder()
        {
            var path = pathRoot + Guid.NewGuid() + @"\"; 

            using (OrderStore orderStore = new OrderStore(path))
            {
                SubmitEvent submitEvent = new SubmitEvent(7, Side.Buy, "MyAsset");
                var orderV1 = new EquityOrder(5, 7, Side.Buy, "MyAsset");
                orderStore.Add(submitEvent, orderV1);                
            }

            using (OrderStore orderStore = new OrderStore(path))
            {
                var order = orderStore.Get(5);
                Assert.IsNotNull(order);
                Assert.That(order.ID, Is.EqualTo(5));
                Assert.That(order.Size, Is.EqualTo(7));
                Assert.That(order.Asset, Is.EqualTo("MyAsset"));                
            }
            
        }

        [Test]
        public void OrderStorePersistsUpdatedOrder()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";

            using (OrderStore orderStore = new OrderStore(path))
            {
                SubmitEvent submitEvent = new SubmitEvent(7, Side.Buy, "MyAsset");
                var orderV1 = new EquityOrder(5, 7, Side.Buy, "MyAsset");
                orderStore.Add(submitEvent, orderV1);
                var tradeEvent = new TradeEvent(13, 17);
                var tradeEvents = new List<TradeEvent>([tradeEvent]);
                var orderV2 = new EquityOrder(5, 7, Side.Buy, "MyAsset", tradeEvents, 13, 17);
                orderStore.Update(orderV2, tradeEvent);
            }

            using (OrderStore orderStore = new OrderStore(path))
            {
                var order = orderStore.Get(5);
                Assert.IsNotNull(order);
                Assert.That(order.ID, Is.EqualTo(5));
                Assert.That(order.Size, Is.EqualTo(7));
                Assert.That(order.Asset, Is.EqualTo("MyAsset"));
                Assert.That(order.Traded, Is.EqualTo(13));
                Assert.That(order.Price, Is.EqualTo(17));
            }

        }
    }
}
