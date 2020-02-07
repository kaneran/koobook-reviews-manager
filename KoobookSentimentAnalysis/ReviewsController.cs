using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoobookSentimentAnalysis
{

    //Credit to atripa5 for the solution ,along with stopwords and AFINN text files, which was implemented into java but I have implemented in C# as I could't did via Android studio 
    //source: https://github.com/atripa5/Sentiment-Analysis-in-Java/tree/master/Data

    //This method works by using the stream reader to add all the words from the stopwords.txt to a string list. It then uses the stream reader to to read the AFINN.txt. Unlike the stopwords.txt, this text file contain a word followed by a 
    //score and this was for each row in the text file. Therefore, for each line in the text field, the word and the score were split and each part was added to the initialised dictionary. After closing the stream reader, it then iterates through each review
    //from the list of reviews(passed into the method's arguments) and for each review, it splits into words and iterates through each word in the review. It first checks whether that word is in the list of stopwords and if is then then it will ignore it and move to the next word.
    //If it is not then it attempt to lookup the dictionary(populated with the values from the AFINN.txt) using the word as the key and trying to get the score affiliated with that word. The score retrieved from teh lookup will be added
    //to the score for that review. However, if an exception was caught while looking up the dinctionary then that meant that there the dictionary does not have a score for that word. Hence it will add nothing to the score. It'll then repeat the process
    //for the other words for that review and once it has finished, the review itself along with its review score will be added to another dicitonary which stores the reviews and its corresponding score. After iterating through all the reviews,
    //it will convert the dictionary to a list and will sort it based on the review scores such that the reviews which had the highest score will be index first in the list. Using that sorted list, it will execute a LINQ query to get the reviews which had review score greater than 0 
    //and convert it to a list. I believed that any reviews that had a score less than zero are negative reviews. If this filtered list contained more than 5 reviews then it will iterate through the first five reviws and all it to the list of sample reviews.
    //If it contained less than 5 reviews then it will iterate through all the reviews and add it to list of sample reviews. It then iterates through each review in the list of sample reviews and uses the stirng builder to append each review while
    //using "$" to separate each review. The output of this stirng builder will be returned by this method.


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

                Dictionary<string, string> afinnDict = new Dictionary<string, string>();
                Dictionary<string, int> reviewsDict = new Dictionary<string, int>();

                path = Path.Combine(Environment.CurrentDirectory, "..", "..", "assets", "AFINN.txt");
                StreamReader sr = new System.IO.StreamReader(path);
                line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split('\t');
                    afinnDict.Add(parts[0], parts[1]);
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
                                string wordScore = afinnDict[word.ToLower()];
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

                List<string> positiveReviews = reviewList.Where(entry => entry.Value > 0).Select(entry => entry.Key).ToList();
                
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
