using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KoobookSentimentAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer server = new TCPServer();
            var fileName = Assembly.GetExecutingAssembly().Location;
            server.Listen(fileName);

        }
    }
}
