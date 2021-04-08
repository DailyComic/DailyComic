using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Retrievers.Dilbert
{
    public class DilbertOfTheDayRetriever : DilbertRetrieverBase
    {
        public override Task<ComicStrip> GetComic()
        {
            return RetryPolicy.ExecuteAsync(async () => await GetComic(false));
        }

    }
}