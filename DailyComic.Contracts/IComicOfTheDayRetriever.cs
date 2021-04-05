using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Contracts
{
    public interface IComicOfTheDayRetriever
    {
        public Task<ComicStrip> GetComicOfTheDay();
    }
}