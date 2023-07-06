using System.Collections.Generic;
using TestExchange.Domain;

namespace TestExchange.Application
{
    public interface ICryptoExchangeStore
    {
        List<string> ExchangesId { get; }
        List<Order> FlattenedAsks { get; }
        List<Order> FlattenedBids { get; }
    }
}