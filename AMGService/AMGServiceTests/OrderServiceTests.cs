namespace AMGServiceTests    
{
    using AMFService;
    public class OrderServiceTests
    {
        string pathRoot = @"c:\AMG\";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task FirstEveryOrderHasAnIDof1()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";            
            var orderService = new OrderService(path);            
            var submitEvent = new SubmitEvent(13, Side.Buy, "AMG Group");
            int orderID = await orderService.Submit(submitEvent);
            Assert.That(orderID, Is.EqualTo(1));            
        }

        [Test]
        public async Task ReturnedOrderHasTheCorrectSize()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path);            
            var submitEvent = new SubmitEvent(13, Side.Buy, "AMG Group");
            int orderID = await orderService.Submit(submitEvent);
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor
            var order = orderService.GetOrder(orderID);
            Assert.That(order.Size, Is.EqualTo(13));
        }

        [Test]
        public async Task TradedOrderHasAPrice()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path);            
            int orderID = await orderService.Submit(new SubmitEvent(13, Side.Buy, "AMG Group"));
            orderService.Trade(orderID, new TradeEvent(13, 17));
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor
            var order = orderService.GetOrder(orderID);
            Assert.That(order.Size, Is.EqualTo(13));
        }

        [Test]
        public async Task perfTest()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path);            
            int orderID = await orderService.Submit(new SubmitEvent(123456789012345621341m, Side.Buy, "AMG Group"));
            for (int i = 0; i < 100000; i++)
            {
                orderService.Trade(orderID, new TradeEvent(1m+i, 2m+i));
            }
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor

            //await Task.Delay(new TimeSpan(0, 0, 4));

            var orderService2 = new OrderService(path);            
            var order = orderService2.GetOrderSync(orderID);
            Assert.That(order.Size, Is.EqualTo(123456789012345621341m));
            Assert.That(decimal.Round(order.TradedPrice, 5), Is.EqualTo(66668m));
            Assert.That(order.TradedSize, Is.EqualTo(5000050000m));            
        }
    }
}
