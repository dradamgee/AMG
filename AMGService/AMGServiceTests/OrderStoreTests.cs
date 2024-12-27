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

            SubmitEvent submitEvent = new SubmitEvent(OrderID.NewOrderID(id), 7, Side.Buy, "MyAsset");            
            await orderStore1.Submit(submitEvent);
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
            var orderIDint = 5;
            var orderID = OrderID.NewOrderID(orderIDint);
            var path = pathRoot + Guid.NewGuid() + @"\";

            var orderStore1 = new OrderStore<BinaryWriter>(path, dal);
            
            SubmitEvent submitEvent = new SubmitEvent(orderID, 7, Side.Buy, "MyAsset");
            var orderV1 = new EquityOrder(7, Side.Buy, "MyAsset");
            await orderStore1.Submit(submitEvent);
            var placeID = await orderStore1.Place(orderID, new PlaceEvent(PlaceID.NewPlaceID(0), 1000m, 531));
            var fillEvent = new FillEvent(placeID, 13, 17);            
            orderStore1.Fill(orderID, fillEvent);
            orderStore1.GetOrderSync(5);

            var orderStore2 = new OrderStore<BinaryWriter>(path, dal);
            orderStore2.GetOrderSync(orderIDint);
            var order = orderStore2.GetOrder(orderIDint);
            Assert.IsNotNull(order);            
            Assert.That(order.Orders.Head.Size, Is.EqualTo(7));
            Assert.That(order.Orders.Head.Asset, Is.EqualTo("MyAsset"));
            Assert.That(order.Placements.Head.FilledSize, Is.EqualTo(13));
            Assert.That(order.Placements.Head.FilledPrice, Is.EqualTo(17));
        }
    }
}
