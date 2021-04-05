using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Contracts
{
    public interface IComicPusher
    {
        Task Push(ComicStrip comic);
    }
}