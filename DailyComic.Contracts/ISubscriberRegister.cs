using System.Collections.Generic;
using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Contracts
{
    public interface ISubscriberRegister
    {
        Task AddSubscriber(SubscriptionSettings subscriptionSettings);
    }
}