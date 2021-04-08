using System;
using System.Collections;
using System.Threading.Tasks;
using DailyComic.Model;

namespace DailyComic.Contracts
{
    public interface IComicRetriever
    {
        Task<ComicStrip> GetComic();
    }
}
