using System;

namespace NFBench.Services.ChatClient
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int applicationServerPort = (int)Int64.Parse (args [0]);

            ChatClient chat = new ChatClient (applicationServerPort);
            chat.start();
        }
    }
}
