using TestExchange.Domain;

namespace TestExchange.Application
{
    public interface IResolver
    {
        PurchaseList Buy(decimal targetAmount);
        PurchaseList Sell(decimal targetAmount);
    }
}