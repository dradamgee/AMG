namespace AMGServiceTests    
{
    using AMGService;
    public class OrderServiceTests
    {
        string pathRoot = @"c:\AMG\";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void FirstEveryOrderHasAnIDof1()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            OrderService orderService = new OrderService();
            orderService.Start(path);
            var OrderService = new OrderService();            
            var submitEvent = new SubmitEvent(13, Side.Buy, "AMG Group");
            int orderID = orderService.Submit(submitEvent);
            Assert.That(orderID, Is.EqualTo(1));            
        }

        [Test]
        public void ReturnedOrderHasTheCorrectSize()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService();
            orderService.Start(path);
            var submitEvent = new SubmitEvent(13, Side.Buy, "AMG Group");
            int orderID = orderService.Submit(submitEvent);
            var order = orderService.GetOrder(orderID);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Size, Is.EqualTo(13));
        }

        [Test]
        public void TradedOrderHasAPrice()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService();
            orderService.Start(path);
            int orderID = orderService.Submit(new SubmitEvent(13, Side.Buy, "AMG Group"));
            orderService.Trade(orderID, new TradeEvent(13, 17));
            var order = orderService.GetOrder(orderID);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Size, Is.EqualTo(13));
        }

        [Test]
        public void perfTest()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService();
            orderService.Start(path);
            int orderID = orderService.Submit(new SubmitEvent(123456789012345621341m, Side.Buy, "AMG Group"));
            for (int i = 0; i < 10000; i++)
            {
                orderService.Trade(orderID, new TradeEvent(1m+i, 2m+i));
            }            
            
            var orderService2 = new OrderService();
            orderService2.Start(path);
            var order = orderService.GetOrder(orderID);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Size, Is.EqualTo(123456789012345621341m));


        }


    }
}