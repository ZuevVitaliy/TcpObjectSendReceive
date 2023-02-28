using Infrastructure;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpObjectReceiver
{
    internal class Program
    {
        private static Queue<string> _messagesQueue = new Queue<string>();

        static async Task Main(string[] args)
        {
            var tcpListener = new TcpListener(IPAddress.Parse(Constants.HOSTNAME), 8888);
            tcpListener.Start();

            using var tcpClient = await tcpListener.AcceptTcpClientAsync();

            Task.Run(() => ReceiveMessage(tcpClient));
            await ReceiveObjectsTask(tcpClient);
        }

        private static async Task ReceiveObjectsTask(TcpClient tcpClient)
        {
            while (true)
            {
                var obj = await ReceiveObject(tcpClient);
                Console.WriteLine(obj?.ToString());
            }
        }

        private static async Task<object> ReceiveObject(TcpClient tcpClient)
        {
            var objectMessages = new List<string>();
            while (objectMessages.Count < 2)
            {
                if (_messagesQueue.Count > 0)
                    objectMessages.Add(_messagesQueue.Dequeue());
                Thread.Sleep(1000);
            }
            string typeString = objectMessages[0];
            var type = JsonConvert.DeserializeObject<Type>(typeString);
            string objData = objectMessages[1];

            var obj = JsonConvert.DeserializeObject(objData, type);
            return obj;
        }

        private static async Task ReceiveMessage(TcpClient tcpClient)
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

            _messagesQueue.Enqueue(builder.ToString());
        }
    }
}