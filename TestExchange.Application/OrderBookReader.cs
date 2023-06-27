using System.Text.Json;
using TestExchange.Domain;

namespace TestExchange.Application
{
    public class OrderBookReader
    {
        private string filePath;
        private Dictionary<string, OrderBook> orderBooks;

        public OrderBookReader(string filePath)
        {
            this.filePath = filePath;
            orderBooks = new Dictionary<string, OrderBook>();
        }

        public Dictionary<string, OrderBook> Read()
        {
            if (orderBooks.Count>0) return orderBooks;

            using (StreamReader reader = new StreamReader(filePath))
            {
                int linenumber = 0;
                string line;
                string[] parts;
                OrderBookDTO orderbookDTO = null;

                while ((line = reader.ReadLine()) != null)
                {
                    linenumber++;
                    parts = line.Split('\t');
                    if (parts.Length != 2) continue;

                    try
                    {
                        orderbookDTO = JsonSerializer.Deserialize<OrderBookDTO>(parts[1]);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine("JSON deserialization error: " + ex.Message);
                    }

                    if (orderbookDTO == null) continue;

                    orderBooks.Add(parts[0], orderbookDTO.ConvertToOrderBook());
                }

                Console.WriteLine("Reading complete. Total number of lines: "+ linenumber);

                return orderBooks;
            }

        }
    }
}