using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoobookSentimentAnalysis
{

    //Credit to atripa5 for the solution ,along with stopwords and AFINN text files, which was implemented into java but I have implemented in C# as I could't did via Android studio   source: https://github.com/atripa5/Sentiment-Analysis-in-Java/tree/master/Data
    public class ReviewsController
    {
        public string GetPositiveReviews(List<string> reviews)
        {
            try
            {
                int count = 0;
                string review;

                List<string> stopwords = new List<string>();
                string line = "";
                string path = Path.Combine(Environment.CurrentDirectory, "..","..", "assets", "stopwords.txt");
                StreamReader stop = new System.IO.StreamReader(path);
                while ((line = stop.ReadLine()) != null)
                {
                    stopwords.Add(line);
                }

                stop.Close();

                Dictionary<string, string> dict = new Dictionary<string, string>();
                Dictionary<string, int> reviewsDict = new Dictionary<string, int>();

                path = Path.Combine(Environment.CurrentDirectory, "..", "..", "assets", "AFINN.txt");
                StreamReader sr = new System.IO.StreamReader(path);
                line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split('\t');
                    dict.Add(parts[0], parts[1]);
                    count++;
                }
                sr.Close();

                foreach (var rvw in reviews)
                {
                    string[] words = rvw.Split(' ');
                    int reviewScore = 0;
                    foreach (var word in words)
                    {
                        if (stopwords.Contains(word.ToLower()))
                        {
                            //Ignore the word 
                        }
                        else
                        {

                            try
                            {
                                string wordScore = dict[word.ToLower()];
                                reviewScore += Int32.Parse(wordScore);
                            }
                            catch (KeyNotFoundException e)
                            {
                                //Do nothing
                            }
                        }
                    }

                    reviewsDict.Add(rvw, reviewScore);
                }

                var reviewList = reviewsDict.ToList();

                //Credit to Leon for the solution on how to sort the values in a dictionary
                reviewList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

                List<string> positiveReviews = reviewsDict.Where(entry => entry.Value > 0).Select(entry => entry.Key).ToList();
                List<string> samplePositiveReviews = new List<string>();
                if (positiveReviews.Count >= 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        samplePositiveReviews.Add(positiveReviews[i]);
                    }
                }
                else {
                    foreach(var positiveReview in positiveReviews) {
                        samplePositiveReviews.Add(positiveReview);
                    }
                }

                StringBuilder sb = new StringBuilder();
                foreach (var rev in samplePositiveReviews) {
                    sb.Append("\"" +rev + "\""+"$");
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
