namespace AMGServiceTests
{
    using System.IO;
    using AMFService;
    using AMFServiceBinaryDAL;

    public class OrderStoreTests
    {
        DAL<BinaryWriter> dal = new BinaryDAL();
        string pathRoot = @"c:\AMG\";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task OrderStorePersistsNewOrder()
        {
            var id = 5;

            var path = pathRoot + Guid.NewGuid() + @"\";

            var orderStore1 = new OrderStore<BinaryWriter>(path, dal);
            
            SubmitEvent submitEvent = new SubmitEvent(7, Side.Buy, "MyAsset");            
            await orderStore1.Submit(id, submitEvent);
            var orderV1 = orderStore1.GetOrderSync(id);

            var orderStore2 = new OrderStore<BinaryWriter>(path, dal);            
            var orderV2 = orderStore2.GetOrderSync(id);

            Assert.IsNotNull(orderV2);            
            Assert.That(orderV2.Size, Is.EqualTo(7));
            Assert.That(orderV2.Asset, Is.EqualTo("MyAsset"));                        
        }

        [Test]
        public async Task OrderStorePersistsUpdatedOrder()
        {
            var id = 5;
            var path = pathRoot + Guid.NewGuid() + @"\";

            var orderStore1 = new OrderStore<BinaryWriter>(path, dal);
            
            SubmitEvent submitEvent = new SubmitEvent(7, Side.Buy, "MyAsset");
            var orderV1 = new EquityOrder(7, Side.Buy, "MyAsset");
            await orderStore1.Submit(id, submitEvent);
            var tradeEvent = new TradeEvent(13, 17);            
            orderStore1.Trade(id, tradeEvent);
            orderStore1.GetOrderSync(id);

            var orderStore2 = new OrderStore<BinaryWriter>(path, dal);
            orderStore2.GetOrderSync(id);
            var order = orderStore2.GetOrder(id);
            Assert.IsNotNull(order);            
            Assert.That(order.Size, Is.EqualTo(7));
            Assert.That(order.Asset, Is.EqualTo("MyAsset"));
            Assert.That(order.TradedSize, Is.EqualTo(13));
            Assert.That(order.TradedPrice, Is.EqualTo(17));
        }
    }
}
