using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static int port = 1034;
        static void Main(string[] args)
        {
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            List<string> testres = new List<string>();
            List<int> result = new List<int>();
            for (int i = 0; i > -1; i++)
            {
                try
                {
                    listenSocket.Bind(ipPoint);
                    
                    listenSocket.Listen(10);
                
                    while (true)
                    {
                        Socket handler = listenSocket.Accept();
                        
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        byte[] data = new byte[256];

                        do
                        {
                            bytes = handler.Receive(data);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            testres.Add(builder.ToString());
                        }
                        while (handler.Available > 0);
                        
                        string message = "ваше сообщение доставлено";
                        data = Encoding.Unicode.GetBytes(message);

                        if (builder.ToString() != "END")
                        {
                            handler.Send(data);
                        }

                        if (testres.Count % 3 == 0)
                        {
                            string message1 = "Помилка!";
                            if (testres[0] == "+")
                            {
                                message1 = (Convert.ToInt32(testres[1]) + Convert.ToInt32(testres[2])).ToString();
                            }
                            else if (testres[0] == "-")
                            {
                                message1 = (Convert.ToInt32(testres[1]) - Convert.ToInt32(testres[2])).ToString();
                            }
                            else if (testres[0] == "*")
                            {
                                message1 = (Convert.ToInt32(testres[1]) * Convert.ToInt32(testres[2])).ToString();
                            }
                            else if (testres[0] == "/")
                            {
                                message1 = (Convert.ToInt32(testres[1]) / Convert.ToInt32(testres[2])).ToString();
                            }
                            Console.WriteLine("Результат дiй клiента: " + message1);
                            for (int f = 0; f < 2; f++)
                            {
                                result.Add(Convert.ToInt32(testres[f + 1]));
                            }
                            testres.Clear();
                        }

                        if (builder.ToString() == "END")
                        {
                            Console.WriteLine("Завершення програми:");
                            data = Encoding.Unicode.GetBytes(Enumerable.Max(result).ToString());
                            handler.Send(data);
                            data = new byte[256];
                            data = Encoding.Unicode.GetBytes(Enumerable.Min(result).ToString());
                            handler.Send(data);
                            data = new byte[256];
                            double ser = 0;
                            for (int r = 0; r < result.Count; r++)
                                ser += result[r];
                            data = Encoding.Unicode.GetBytes((ser / Convert.ToDouble(result.Count)).ToString());
                            handler.Send(data);
                            data = new byte[256];
                        }
                        
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }
    }
}