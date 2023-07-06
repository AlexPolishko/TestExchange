using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TestExchange.Domain;

namespace TestExchange.Application
{
    public class OrderBookReader : IOrderBookReader
    {
        private string filePath = "order_books_data";

        public OrderBookReader(IOptions<AppSettings> appSettings)
        {
            this.filePath = appSettings.Value.FilePath;
        }

        public Dictionary<string, OrderBook> Read()
        {
            var orderBooks = new Dictionary<string, OrderBook>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                int linenumber = 0;
                string line;
                string[] parts;
                OrderBookDto orderbookDTO = null;

                while ((line = reader.ReadLine()) != null)
                {
                    linenumber++;
                    parts = line.Split('\t');
                    if (parts.Length != 2) continue;

                    try
                    {
                        orderbookDTO = JsonSerializer.Deserialize<OrderBookDto>(parts[1]);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine("JSON deserialization error: " + ex.Message);
                    }

                    if (orderbookDTO == null) continue;

                    orderBooks.Add(parts[0], orderbookDTO.ConvertToOrderBook(parts[0]));
                }

                Console.WriteLine("Reading complete. Total number of lines: " + linenumber);

                return orderBooks;
            }

        }
    }
}