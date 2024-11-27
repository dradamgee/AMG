namespace AMGService
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Text.Json;

    public enum Side
    {
        None = 0,
        Buy = 1,
        Sey = 2,
    }

    public enum EventType
    {
        Submit = 0,
        Trade = 1,
    }

    public class OrderStore : IDisposable
    {
        private EventPlayer eventPlayer = new EventPlayer();

        public OrderStore(string _) 
        {
            StorePath = _;

            if (!Directory.Exists(StorePath))
            {
                Directory.CreateDirectory(StorePath);
            }

            foreach (string FileName in Directory.GetFiles(StorePath))
            {                
                var ID = int.Parse(Path.GetFileNameWithoutExtension(FileName));
                var fileToRead = File.ReadLines(FileName);
                EquityOrder equityOrder = null;
                foreach (var line in fileToRead)
                {
                    var typeID = int.Parse(line.First().ToString());
                    var eventImage = line.Substring(1);
                    
                    switch (typeID)
                    {
                        case 0:
                            var submitEvent = JsonSerializer.Deserialize<SubmitEvent>(eventImage);
                            equityOrder = eventPlayer.Submit(ID, submitEvent);
                            break;
                        case 1:
                            var tradeEvent = JsonSerializer.Deserialize<TradeEvent>(eventImage);
                            equityOrder = eventPlayer.Trade(equityOrder, tradeEvent);
                            break;
                        default:
                            break;
                    }
                }
                orders.Add(equityOrder.ID, equityOrder);
            }
        }
        private Dictionary<int, EquityOrder> orders = new Dictionary<int, EquityOrder>();
        private string StorePath;
       

        public void Add(SubmitEvent submitEvent, EquityOrder equityOrder)
        {
            string filePath = StorePath + equityOrder.ID + ".txt";            
            var eventImage = JsonSerializer.Serialize(submitEvent);
            using (StreamWriter writetext = new StreamWriter(filePath))
            {
                writetext.WriteLine((int)EventType.Submit + eventImage);                
            }
            orders.Add(equityOrder.ID, equityOrder);
        }

        public EquityOrder Get(int ID){
            return orders[ID];
        }

        public void Dispose()
        {
            
        }

        public void Update(EquityOrder equityOrder, TradeEvent tradeEvent)
        {
            string filePath = StorePath + equityOrder.ID + ".txt";
            var eventImage = JsonSerializer.Serialize(tradeEvent);
            using (StreamWriter writetext = File.AppendText(filePath))
            {                
                writetext.WriteLine((int)EventType.Trade + eventImage);
            }
            orders[equityOrder.ID] =  equityOrder;
        }
    }


    public class EventPlayer
    {
        public EquityOrder Submit(int id, SubmitEvent submitEvent)
        {            
            return new EquityOrder(id, submitEvent.Size, submitEvent.Side, submitEvent.Asset);            
        }

        public EquityOrder Trade(EquityOrder equityOrder, TradeEvent tradeEvent)
        {
            var newPrice = (equityOrder.Traded * equityOrder.Price + tradeEvent.Size * tradeEvent.Price) / (equityOrder.Traded + tradeEvent.Size);
            return new EquityOrder(
                equityOrder.ID,
                equityOrder.Size,
                equityOrder.Side,
                equityOrder.Asset,
                new List<TradeEvent>(equityOrder.Trades.Append(tradeEvent)),
                equityOrder.Traded + tradeEvent.Size,
                equityOrder.Price + tradeEvent.Price);
        }
    }

    public class EquityOrder
    {       

        public EquityOrder(int id, decimal size, Side buy, string asset, List<TradeEvent> trades = null, decimal traded = 0, decimal price = 0)
        {
            ID = id;
            Size = size;
            Side = buy;
            Asset = asset;
            Traded = traded;
            Price = price;
            Trades = trades?? new List<TradeEvent>();
        }

        public int ID { get; }
        public decimal Size { get; }
        public Side Side { get; }
        public string Asset { get; }
        public decimal Traded { get; }
        public decimal Price { get; }
        public List<TradeEvent> Trades { get; }
    }

    public class SubmitEvent 
    {
        public SubmitEvent(decimal size, Side side, string asset)
        {
            Size = size;
            Side = side;
            Asset = asset;
        }

        public decimal Size { get; }
        public Side Side { get; }
        public string Asset { get; }
    }

    public class TradeEvent
    {
        public decimal Size { get; }
        public decimal Price { get; }

        public TradeEvent(decimal size, decimal price)
        {
            this.Size = size;
            this.Price = price;
        }
    }


    public class OrderService
    {
        int nextID = 1;
        EventPlayer player = new EventPlayer();
        OrderStore orders;

        private int NextID { get { return nextID++; } }
        

        public EquityOrder GetOrder(int orderID)
        {
            return orders.Get(orderID);
        }

        public void Start(string path) {
            orders = new OrderStore(path);
        }

        public int Submit(SubmitEvent submitEvent)
        {
            var id = NextID;
            orders.Add(submitEvent, player.Submit(id, submitEvent));
            return id;
        }

        public void Trade(int orderID, TradeEvent tradeEvent)
        {
            var order = orders.Get(orderID);
            EquityOrder newOrder = player.Trade(order, tradeEvent);
            orders.Update(newOrder, tradeEvent);
        }
    }
}
