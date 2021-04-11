using System.Collections.Generic;
using DailyComic.Integrations.Slack.Model;
using DailyComic.Model;

namespace DailyComic.Integrations.Slack
{

    internal class DilbertCardCreator : CardCreator
    {
        protected override void RenderHeader(ComicStrip comic, MessageCard card)
        {
            string buttons = GetNavigationButtons(comic);
            if (!string.IsNullOrEmpty(buttons))
            {
                buttons += " | ";
            }
                          card.Blocks.Add(new Block()
                          {
                              Type = "header",
                              Text = new Text()
                              {
                                  TextText = $"{comic.Title}"
                              }
                          });
            card.Blocks.Add(new Block()
            {
                Type = "context",
                Elements = new List<Text>()
                {
                    new Text()
                    {
                        Type = "mrkdown",
                        TextText= $"<{comic.PageUrl}|See on {GetDomain(comic)} :arrow_upper_right:> | " +
                                 buttons + 
                                  $"[BUY](https://dilbert.com/buy?date={comic.ComicId})"
                    }
                }
            });

        }

        
    }
}