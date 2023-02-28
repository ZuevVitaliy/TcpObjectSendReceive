using Infrastructure;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpObjectReceiver
{
    internal class Program
    {
        private static MessagesReceiveHelper _receiveHelper = new MessagesReceiveHelper();

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
                if(obj != null)
                    Console.WriteLine(obj.ToString());
            }
        }

        private static async Task<object> ReceiveObject(TcpClient tcpClient)
        {
            var objectMessages = new List<string>();
            while (objectMessages.Count < 2)
            {
                var message = _receiveHelper.GetFullMessage();
                if(!string.IsNullOrEmpty(message))
                    objectMessages.Add(message);
            }
            string typeString = objectMessages[0];
            var type = JsonConvert.DeserializeObject<Type>(typeString);
            string objData = objectMessages[1];

            var obj = JsonConvert.DeserializeObject(objData, type);
            return obj;
        }

        private static async Task ReceiveMessage(TcpClient tcpClient)
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

                _receiveHelper.AddMessage(builder.ToString());
            }
        }
    }
}