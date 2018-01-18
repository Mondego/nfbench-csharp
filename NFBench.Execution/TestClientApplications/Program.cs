using System;

namespace TestClientApplications
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length >= 2) {
                string hostname = args[0];
                int port = (int)Int64.Parse (args [1]);
                int id = -1;
                if (args.Length >= 3) {
                    id = (int)Int64.Parse (args [2]);
                }

                TestChatClient chatClient = new TestChatClient(hostname, port, id);
                chatClient.start();
            }
        }
    }
}
