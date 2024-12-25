namespace AMGServiceTests    
{
    using AMFService;
    using Microsoft.FSharp.Data.UnitSystems.SI.UnitNames;

    [TestFixture(OrderServiceMode.BinaryMode)]
    [TestFixture(OrderServiceMode.JsonMode)]
    public class OrderServiceTests
    {
        public OrderServiceTests(OrderServiceMode _mode)
        {
            mode = _mode;
        }
        OrderServiceMode mode;
        string pathRoot = @"c:\AMG\";


        [Test]
        public async Task FirstEveryOrderHasAnIDof1()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";            
            var orderService = new OrderService(path, mode);            
            var submitEvent = new SubmitEvent(13, Side.Buy, "AMG Group");
            int orderID = await orderService.Submit(submitEvent);
            Assert.That(orderID, Is.EqualTo(1));            
        }

        [Test]
        public async Task ReturnedOrderHasTheCorrectSize()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path, mode);            
            var submitEvent = new SubmitEvent(13, Side.Buy, "AMG Group");
            int orderID = await orderService.Submit(submitEvent);
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor
            var order = orderService.GetOrder(orderID);
            Assert.That(order.Orders.Head.Size, Is.EqualTo(13));
        }

        [Test]
        public async Task TradedOrderHasAPrice()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path, mode);            
            int orderID = await orderService.Submit(new SubmitEvent(13, Side.Buy, "AMG Group"));
            orderService.Trade(orderID, new TradeEvent(0, 13, 17));
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor
            var order = orderService.GetOrder(orderID);
            Assert.That(order.Orders.Head.Size, Is.EqualTo(13));
        }

        [Test]
        public async Task perfTest()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path, mode);            
            int orderID = await orderService.Submit(new SubmitEvent(123456789012345621341m, Side.Buy, "AMG Group"));
            for (int i = 0; i < 10000; i++)
            {
                orderService.Trade(orderID, new TradeEvent(0, 1m + i, 2m+i));
            }
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor

            var orderService2 = new OrderService(path, mode);            
            var order = orderService2.GetOrderSync(orderID);
            Assert.That(order.Orders.Head.Size, Is.EqualTo(123456789012345621341m));
            Assert.That(decimal.Round(order.TradedPrice, 5), Is.EqualTo(6668m));
            Assert.That(order.TradedSize, Is.EqualTo(50005000m));            
        }

        [Test]        
        public async Task perfTest2()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path, mode);
            var tasks = new List<Task>();

            var orderVolume = 1;
            var executionVolume = 1000000;


            for (int j = 1; j <= orderVolume; j++)
            {
                tasks.Add(orderService.Submit(new SubmitEvent(123456789012345621341m, Side.Buy, "AMG Group")));
            }

            Task.WaitAll(tasks.ToArray());

            for (int j = 1; j <= orderVolume; j++)
            {
                for (int i = 0; i < executionVolume; i++)
                {
                    orderService.Trade(j, new TradeEvent(0, 1m + i, 2m + i));
                }
            }

            orderService.GetOrderSync(orderVolume); // wait for the events are processed by the actor

            var orderService2 = new OrderService(path, mode);
            var order = orderService2.GetOrderSync(orderVolume);

            Assert.That(order.Orders.Head.Size, Is.EqualTo(123456789012345621341m));
            Assert.That(decimal.Round(order.TradedPrice,5), Is.EqualTo(666668m));
            Assert.That(order.TradedSize, Is.EqualTo(500000500000m));
        }

    }
}
