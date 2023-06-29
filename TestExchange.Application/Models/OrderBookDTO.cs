﻿using TestExchange.Domain;

namespace TestExchange.Application
{
    internal class OrderBookDto
    {
        public OrderContainer[] Bids { get; set; }
        public OrderContainer[] Asks { get; set; }

        public OrderBook ConvertToOrderBook(string exchangeId)
        {
            var result = new OrderBook();
            result.Bids = new Order[Bids.Length];
            result.Asks = new Order[Asks.Length];

            for (int i = 0; i < Bids.Length; i++)
            {
                result.Bids[i] = new Order(Bids[i].Order.Price, Bids[i].Order.Amount, OrderType.Buy, exchangeId);
            }

            for (int i = 0; i < Asks.Length; i++)
            {
                result.Asks[i] = new Order(Asks[i].Order.Price, Asks[i].Order.Amount, OrderType.Sell, exchangeId);
            }

            return result;
        }

    }
}
