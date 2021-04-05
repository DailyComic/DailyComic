using System.Collections.Generic;
using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Contracts
{
    public interface ISubscriberProvider
    {
        Task<IEnumerable<SubscriptionSettings>> GetSubscribers(SubscriptionName subscriptionName);
    }
}