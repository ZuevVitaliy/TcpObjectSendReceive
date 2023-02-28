using Infrastructure;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpObjectSender
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Thread.Sleep(1000);

            using var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(Constants.HOSTNAME, 8888);

            var testPerson = new Person { Name = "Павел", BirthDate = new DateTime(1988, 10, 28) };
            await SendObjectAsync(tcpClient, testPerson);
            var testPet = new Pet { Name = "Мухтар", PetType = PetType.Dog };
            await SendObjectAsync(tcpClient, testPet);

            Console.ReadLine();
        }

        private static async Task SendObjectAsync(TcpClient tcpClient, object obj)
        {
            var objType = obj.GetType();
            var serializedObjType = JsonConvert.SerializeObject(objType);
            var serializedObject = JsonConvert.SerializeObject(obj);

            var stream = tcpClient.GetStream();

            var buffer = Encoding.UTF8.GetBytes(serializedObjType + Constants.END_SYMBOL);
            await stream.WriteAsync(buffer, 0, buffer.Length);

            buffer = Encoding.UTF8.GetBytes(serializedObject + Constants.END_SYMBOL);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}