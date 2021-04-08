using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Retrievers.Dilbert
{
    public class RandomDilbertRetriever : DilbertRetrieverBase
    {
        public override Task<ComicStrip> GetComic()
        {
            return RetryPolicy.ExecuteAsync(async () => await GetComic(true));
        }

    }
}