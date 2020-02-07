using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KoobookSentimentAnalysis
{
    class TCPServer
    {
        //This method works by listening on port 9875 for any incoming connection. After it accepts the connection from the client being the Android application, it will use the network stream to read the data being sent
        //from the android app which is essentially all the reviews of the books liked by a certain user. The server keeps reading from the stream until it receives the "#" symbol which is the Android app's way of saying that it has
        //sent all the data. The server then formats the received data and uses the Reviews Controller to extract only the positive reviews from this data. After the Reviews Controller returns the string containing the top 5 positive reviews
        //it will send it to the android application and after sending it, it will also send a sub string "]d2C>^+" to tell the android app that it has sent all the data. If then waits until the Android app initiates the partial handshake
        //for closing connection. After closing connection, a console window opens and the entire workflow repeats.  
        public void Listen(string fileName)
        {
            TcpListener server = null;
            StringBuilder stringBuilder;
            string dataFromClient;
            try
            {
                Int32 port = 9875;
                server = new TcpListener(IPAddress.Any, port);
                server.Start();

                Byte[] bytes = new Byte[256];
                Byte[] reviewsBytes = new byte[256];

                String data = null;
                stringBuilder = new StringBuilder();
                bool bookDataSentToAndroid = false;
                while (bookDataSentToAndroid == false)
                {

                    Console.WriteLine("Waiting for connection....");
                    TcpClient client = server.AcceptTcpClient();

                    Console.WriteLine("Connected!");
                    data = null;


                    NetworkStream stream = client.GetStream();

                    int i;
                    bool dataSent = false;
                    bool dataFromClientReceived = false;
                    bool ackReceived = false;

                    //Read data from client to receive all the bytes and append it together to prdouce the isbn number
                    while (dataFromClientReceived == false)
                    {
                        i = stream.Read(reviewsBytes, 0, reviewsBytes.Length);
                        var mData = System.Text.Encoding.ASCII.GetString(reviewsBytes, 0, i);
                        stringBuilder.Append(mData);
                        if (stringBuilder.ToString().Contains("#"))
                        {
                            dataFromClientReceived = true;
                        }

                    }

                    dataFromClient = stringBuilder.ToString().Replace("#", "");
                    Console.WriteLine("Received: {0}", dataFromClient);

                    //Check to see if the this solution will need collect data of one book or more than one book
                    //this is done by the client including a flag to determine the number of books as part of sending the message via the network stream



                    //Analyse the reviews and only the ones that are positive and send it to the client

                    while (dataSent == false)
                    {
                        Helper helper = new Helper();
                        ReviewsController reviewsController = new ReviewsController();
                        var reviews = helper.CreateListFromDataString(dataFromClient);
                        var positiveReviews = reviewsController.GetPositiveReviews(reviews);

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(positiveReviews + "]d2C>^+");
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0} ", positiveReviews + "]d2C>^+");
                        dataSent = true;
                    }


                    //Reads data from client to check if the client wants to close connection(FIN)
                    //or has acknowledged(ACK) that the server is ready to close connection
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0 && ackReceived == false)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received data{0}: ", data);

                        data = data.ToUpper();
                        if (data.Equals("FIN") || data.Equals("F"))
                        {
                            byte[] ack_msg = System.Text.Encoding.ASCII.GetBytes("ACK");
                            stream.Write(ack_msg, 0, ack_msg.Length);


                            byte[] fin_msg = System.Text.Encoding.ASCII.GetBytes("FIN");
                            stream.Write(fin_msg, 0, ack_msg.Length);

                        }
                        else if (data.Equals("ACK") || data.Equals("A"))
                        {
                            byte[] closed_msg = System.Text.Encoding.ASCII.GetBytes("CLOSED");
                            stream.Write(closed_msg, 0, closed_msg.Length);
                            ackReceived = true;
                            Thread.Sleep(1000);
                        }
                    }
                    bookDataSentToAndroid = true;
                    client.Close();

                    System.Diagnostics.Process.Start(fileName);
                    Environment.Exit(0);

                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se);
            }
            finally
            {
                server.Stop();

            }

            Console.WriteLine("/n enter to continue");
            Console.Read();

        }

    }
}
