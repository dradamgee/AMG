namespace AMGServiceTests    
{
    using AMFService;
    using Microsoft.FSharp.Data.UnitSystems.SI.UnitNames;
    using System.Threading.Tasks;

    [TestFixture(OrderServiceMode.BinaryMode)]
    //[TestFixture(OrderServiceMode.JsonMode)]
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
            var submitCommand = new SubmitCommand(13, Side.Buy, "AMG Group");
            var orderID = await orderService.Submit(submitCommand);
            Assert.That(orderID.Item, Is.EqualTo(1));            
        }

        [Test]
        public async Task ReturnedOrderHasTheCorrectSize()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path, mode);            
            var submitCommand = new SubmitCommand(13, Side.Buy, "AMG Group");
            var orderID = await orderService.Submit(submitCommand);
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor
            var order = orderService.GetOrder(orderID);
            Assert.That(order.Orders.Head.Size, Is.EqualTo(13));
        }

        [Test]
        public async Task FilledOrderHasAPrice()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path, mode);            
            var orderID = await orderService.Submit(new SubmitCommand(13, Side.Buy, "AMG Group"));            
            var placeID = await orderService.Place(new PlaceCommand(orderID, 1000m, 531));
            orderService.GetOrderSync(orderID);
            orderService.Fill(orderID, new FillEvent(placeID, 13, 17));
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor
            var order = orderService.GetOrder(orderID);
            Assert.That(order.Orders.Head.Size, Is.EqualTo(13));
        }

        [Test]
        public async Task perfTest()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path, mode);            
            var orderID = await orderService.Submit(new SubmitCommand(123456789012345621341m, Side.Buy, "AMG Group"));
            var placeID = await orderService.Place(new PlaceCommand(orderID, 1000m, 531));
            for (int i = 0; i < 10000; i++)
            {
                orderService.Fill(orderID, new FillEvent(placeID, 1m + i, 2m+i));
            }
            orderService.GetOrderSync(orderID); // wait for the events are processed by the actor

            var orderService2 = new OrderService(path, mode);            
            var order = orderService2.GetOrderSync(orderID);
            Assert.That(order.Orders.Head.Size, Is.EqualTo(123456789012345621341m));
            Assert.That(decimal.Round(order.Placements.Head.FilledPrice, 5), Is.EqualTo(6668m));
            Assert.That(order.Placements.Head.FilledSize, Is.EqualTo(50005000m));            
        }

        [Test]        
        public async Task perfTest2()
        {
            var path = pathRoot + Guid.NewGuid() + @"\";
            var orderService = new OrderService(path, mode);
            var tasks = new List<Task>();

            var orderVolume = 1;
            var executionVolume = 10000;


            for (int j = 1; j <= orderVolume; j++)
            {
                tasks.Add(orderService.Submit(new SubmitCommand(123456789012345621341m, Side.Buy, "AMG Group")));

            }

            Task.WaitAll(tasks.ToArray());

            foreach (Task<OrderID> task in tasks) {
                var orderID = await task;
                var placeID = await orderService.Place(new PlaceCommand(orderID, 1000m, 531));

                for (int i = 0; i < executionVolume; i++)
                {
                    orderService.Fill(orderID, new FillEvent(placeID, 1m + i, 2m + i));
                }
            }


            orderService.GetOrderSync(OrderID.NewOrderID(orderVolume)); // wait for the events are processed by the actor

            var orderService2 = new OrderService(path, mode);
            var order = orderService2.GetOrderSync(OrderID.NewOrderID(orderVolume));

            Assert.That(order.Orders.Head.Size, Is.EqualTo(123456789012345621341m));
            Assert.That(decimal.Round(order.Placements.Head.FilledPrice,5), Is.EqualTo(6668m));
            Assert.That(order.Placements.Head.FilledSize, Is.EqualTo(50005000m));
        }

    }
}
