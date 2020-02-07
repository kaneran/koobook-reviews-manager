using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoobookSentimentAnalysis
{
    public class Helper
    {
        //This method creates a string array, from the data passed into the method's arguments, by splitting each string that were separated by the "$". The method then converts this to a list and returns it.
        //This method was needed as the reviews being sent from the android application via TCP was in the form "review1$review2$review3" and this method will help to convert this a list of reviews without the "$" symbol
        public List<string> CreateListFromDataString(String data) {
            string[] reviews = data.Split('$');
            return reviews.ToList();
        }
    }
}
