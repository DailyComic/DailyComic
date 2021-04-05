using System.Collections.Generic;
using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Contracts
{
    public interface IComicPusher
    {
        Task Push(ComicStrip comic, IEnumerable<SubscriptionSettings> subscriptionSettingsEnumerable);
    }
}