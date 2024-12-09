namespace AMGService
{    
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

    public class OrderStore 
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
                            equityOrder = eventPlayer.Submit(submitEvent);
                            break;
                        case 1:
                            var tradeEvent = JsonSerializer.Deserialize<TradeEvent>(eventImage);
                            equityOrder = eventPlayer.Trade(equityOrder, tradeEvent);
                            break;
                        default:
                            break;
                    }
                }
                orders.Add(ID, equityOrder);
            }
        }
        private Dictionary<int, EquityOrder> orders = new Dictionary<int, EquityOrder>();
        private string StorePath;
       

        public void Submit(int ID, SubmitEvent submitEvent)
        {            
            EquityOrder newOrder = eventPlayer.Submit(submitEvent);

            string filePath = StorePath + ID + ".txt";            
            var eventImage = JsonSerializer.Serialize(submitEvent);
            using (StreamWriter writetext = new StreamWriter(filePath))
            {
                writetext.WriteLine((int)EventType.Submit + eventImage);                
            }
            orders.Add(ID, newOrder);
        }

        public EquityOrder GetOrderSync(int ID) {  return GetOrder(ID); }

        public EquityOrder GetOrder(int ID){
            return orders[ID];
        }



        public void Trade(int ID, TradeEvent tradeEvent)
        {
            var order = orders[ID];
            EquityOrder newOrder = eventPlayer.Trade(order, tradeEvent);

            string filePath = StorePath + ID + ".txt";
            var eventImage = JsonSerializer.Serialize(tradeEvent);
            using (StreamWriter writetext = File.AppendText(filePath))
            {                
                writetext.WriteLine((int)EventType.Trade + eventImage);
            }
            orders[ID] = newOrder;
        }
    }


    public class EventPlayer
    {
        public EquityOrder Submit(SubmitEvent submitEvent)
        {            
            return new EquityOrder(submitEvent.Size, submitEvent.Side, submitEvent.Asset);            
        }

        public EquityOrder Trade(EquityOrder equityOrder, TradeEvent tradeEvent)
        {
            var newPrice = (equityOrder.TradedSize * equityOrder.TradedPrice + tradeEvent.Size * tradeEvent.Price) / (equityOrder.TradedSize + tradeEvent.Size);
            return new EquityOrder(
                //equityOrder.ID,
                equityOrder.Size,
                equityOrder.Side,
                equityOrder.Asset,
                new List<TradeEvent>(equityOrder.Trades.Append(tradeEvent)),
                equityOrder.TradedSize + tradeEvent.Size,
                equityOrder.TradedPrice + tradeEvent.Price);
        }
    }

    public class EquityOrder
    {       

        public EquityOrder(//int id, 
            decimal size, Side side, string asset, List<TradeEvent> trades = null, decimal tradedSize = 0, decimal tradedPrice = 0)
        {
            //ID = id;
            Size = size;
            Side = side;
            Asset = asset;
            TradedSize = tradedSize;
            TradedPrice = tradedPrice;
            Trades = trades?? new List<TradeEvent>();
        }

        //public int ID { get; }
        public decimal Size { get; }
        public Side Side { get; }
        public string Asset { get; }
        public decimal TradedSize { get; }
        public decimal TradedPrice { get; }
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
        OrderStore orders;

        private int NextID { get { return nextID++; } }


        public EquityOrder GetOrder(int orderID) { return GetOrderSync(orderID); }

        public EquityOrder GetOrderSync(int orderID)
        {
            return orders.GetOrderSync(orderID);
        }


        public OrderService(string path) {
            orders = new OrderStore(path);
        }

        public int Submit(SubmitEvent submitEvent)
        {
            var id = NextID;
            orders.Submit(id, submitEvent);
            return id;
        }

        public void Trade(int orderID, TradeEvent tradeEvent)
        {            
            orders.Trade(orderID, tradeEvent);
        }
    }
}
