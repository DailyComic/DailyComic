using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Contracts
{
    public interface IComicSender
    {
        Task<ComicDeliveryResult> SendComicTo(SubscriptionSettings settings);
    }
}