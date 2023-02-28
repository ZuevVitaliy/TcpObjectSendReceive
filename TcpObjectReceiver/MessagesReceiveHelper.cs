using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpObjectReceiver
{
    internal class MessagesReceiveHelper
    {
        private StringBuilder _builder = new StringBuilder();
        private Mutex _mutex = new Mutex();

        public void AddMessage(string inputString)
        {
            _mutex.WaitOne();

            _builder.Append(inputString);

            _mutex.ReleaseMutex();
        }

        public string GetFullMessage()
        {
            _mutex.WaitOne();

            string fullMessage = string.Empty;

            var allMessages = _builder.ToString();
            if (allMessages.Contains(Constants.END_SYMBOL))
            {
                fullMessage = allMessages.Substring(0, allMessages.IndexOf(Constants.END_SYMBOL));
                _builder.Remove(0, fullMessage.Length + Constants.END_SYMBOL.Length);
            }

            _mutex.ReleaseMutex();

            return fullMessage;
        }
    }
}
