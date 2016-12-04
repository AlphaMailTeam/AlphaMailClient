using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using AlphaMailClient.Events;

namespace AlphaMailClient
{
    public class Client
    {
        public event EventHandler<AuthMessageReceivedEventArgs> AuthMessageReceived;
        public event EventHandler<ConnectedToServerEventArgs> ConnectedToServer;
        public event EventHandler<DisconnectedFromServerEventArgs> DisconnectedFromServer;
        public event EventHandler<ErrorMessageReceivedEventArgs> ErrorMessageReceived;
        public event EventHandler<MessageMessageReceivedEventArgs> MessageMessageReceived;
        public event EventHandler<PKeyMessageReceivedEventArgs> PKeyMessageReceived;

        private TcpClient client;
        private BinaryReader reader;
        private BinaryWriter writer;

        public Client(string host, int port)
        {
            client = new TcpClient(host, port);
            while (!client.Connected)
                ;
            reader = new BinaryReader(client.GetStream());
            writer = new BinaryWriter(client.GetStream());
        }

        public void Close()
        {
            reader.Close();
            writer.Close();
            client.Close();
        }

        private bool sending = false;
        public void Send(string msg, params object[] args)
        {
            while (sending)
                ;
            sending = true;
            writer.Write(string.Format(msg, args));
            writer.Flush();
            sending = false;
        }
        public void SendCheck()
        {
            Send("CHECK");
        }
        public void SendGetKey(string user)
        {
            Send("GETKEY {0}", user);
        }
        public void SendLogin(string user, string pass)
        {
            Send("LOGIN {0} {1}", user, pass);
        }
        public void SendMessage(string toUser, string content)
        {
            Send("SEND {0} {1}", toUser, content);
        }
        public void SendRegister(string user, string pass, string pkey, string e)
        {
            Send("REGISTER {0} {1} {2} {3}", user, pass, pkey, e);
        }

        public void Start()
        {
            new Thread(() => listenThread()).Start();
        }

        private void listenThread()
        {
            try
            {
                while (true)
                {
                    string msg = reader.ReadString();
                    if (msg == "PING")
                        Send("PONG");
                    else
                        parseMsg(msg);
                }
            }
            catch (IOException)
            {
                OnDisconnectedFromServer(new DisconnectedFromServerEventArgs());
            }
        }

        private void parseMsg(string msg)
        {
            string[] parts = msg.Split(' ');

            switch (parts[0].ToUpper())
            {
                case "AUTH":
                    OnAuthMessageReceived(new AuthMessageReceivedEventArgs(msg, splitArray(parts, 1)));
                    break;
                case "ERROR":
                    OnErrorMessageReceived(new ErrorMessageReceivedEventArgs(msg, splitArray(parts, 1)));
                    break;
                case "MESSAGE":
                    OnMessageMessageReceived(new MessageMessageReceivedEventArgs(msg, parts[1], splitArray(parts, 2)));
                    break;
                case "PKEY":
                    OnPKeyMessageReceived(new PKeyMessageReceivedEventArgs(msg, parts[1], parts[2], parts[3]));
                    break;
            }
        }

        private string splitArray(string[] arr, int startIndex, char sep = ' ')
        {
            StringBuilder sb = new StringBuilder();
            for (int i = startIndex; i < arr.Length; i++)
                sb.AppendFormat("{0}{1}", arr[i], sep);
            return sb.ToString();
        }

        protected virtual void OnAuthMessageReceived(AuthMessageReceivedEventArgs e)
        {
            var handler = AuthMessageReceived;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnConnectedToServer(ConnectedToServerEventArgs e)
        {
            var handler = ConnectedToServer;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnDisconnectedFromServer(DisconnectedFromServerEventArgs e)
        {
            var handler = DisconnectedFromServer;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnErrorMessageReceived(ErrorMessageReceivedEventArgs e)
        {
            var handler = ErrorMessageReceived;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnMessageMessageReceived(MessageMessageReceivedEventArgs e)
        {
            var handler = MessageMessageReceived;
            if (handler != null)
                handler(this, e);
        }
        protected virtual void OnPKeyMessageReceived(PKeyMessageReceivedEventArgs e)
        {
            var handler = PKeyMessageReceived;
            if (handler != null)
                handler(this, e);
        }
    }
}

