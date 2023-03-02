using Infrastructure;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpObjectReceiver
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var tcpListener = new TcpListener(IPAddress.Parse(Constants.HOSTNAME), 8888);
            tcpListener.Start();

            using var tcpClient = await tcpListener.AcceptTcpClientAsync();

            await ReceiveObjectsTaskRun(tcpClient);
        }

        private static async Task ReceiveObjectsTaskRun(TcpClient tcpClient)
        {
            var messagesHelper = new MessagesReceiveHelper();
            ReceiveMessagesAsync(tcpClient, messagesHelper);
            await Task.Run(() =>
            {
                while (true)
                {
                    var obj = ReceiveObject(tcpClient, messagesHelper);
                    if (obj != null)
                        Console.WriteLine(obj.ToString());
                }
            });
        }

        private static object ReceiveObject(TcpClient tcpClient, MessagesReceiveHelper messagesHelper)
        {
            var objectMessages = new List<string>();
            while (objectMessages.Count < 2)
            {
                var message = messagesHelper.GetFullMessage();
                if(!string.IsNullOrEmpty(message))
                    objectMessages.Add(message);
            }
            string typeString = objectMessages[0];
            var type = JsonConvert.DeserializeObject<Type>(typeString);
            string objData = objectMessages[1];

            var obj = JsonConvert.DeserializeObject(objData, type);
            return obj;
        }

        private static async Task ReceiveMessagesAsync(TcpClient tcpClient, MessagesReceiveHelper messagesHelper)
        {
            while (true)
            {
                var stream = tcpClient.GetStream();
                var builder = new StringBuilder();
                byte[] buffer = new byte[256];

                do
                {
                    int bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string addingText = Encoding.UTF8.GetString(buffer, 0, bytes);
                    builder.Append(addingText);
                } while (stream.DataAvailable);

                messagesHelper.AddMessage(builder.ToString());
            }
        }
    }
}