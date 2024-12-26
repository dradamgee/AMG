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
            Assert.That(orderV2.Orders.Head.Size, Is.EqualTo(7));
            Assert.That(orderV2.Orders.Head.Asset, Is.EqualTo("MyAsset"));                        
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
            orderStore1.Place(id, new PlaceEvent(0, 1000m, 531));
            var fillEvent = new FillEvent(0, 13, 17);            
            orderStore1.Fill(id, fillEvent);
            orderStore1.GetOrderSync(id);

            var orderStore2 = new OrderStore<BinaryWriter>(path, dal);
            orderStore2.GetOrderSync(id);
            var order = orderStore2.GetOrder(id);
            Assert.IsNotNull(order);            
            Assert.That(order.Orders.Head.Size, Is.EqualTo(7));
            Assert.That(order.Orders.Head.Asset, Is.EqualTo("MyAsset"));
            Assert.That(order.Placements.Head.FilledSize, Is.EqualTo(13));
            Assert.That(order.Placements.Head.FilledPrice, Is.EqualTo(17));
        }
    }
}
