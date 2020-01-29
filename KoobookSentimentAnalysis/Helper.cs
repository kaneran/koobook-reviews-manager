using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoobookSentimentAnalysis
{
    public class Helper
    {
        public List<string> CreateListFromDataString(String data) {
            string[] reviews = data.Split('$');
            return reviews.ToList();
        }
    }
}
