using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace FlightSimulator.Model
{
    public class MainWindowModel
    {
        private string script;
        private int cmdPort;
        private int infoPort;
        private string ip;
        private bool isAutoPilot;
        private bool isJoystick;


        // from the XML
        private float aileron;
        private float throttle;
        private float rudder;
        private float elevator;
        private float lon;
        private float lat;
        

        public MainWindowModel()
        {
            // some defaults
            this.script = "set controls/flight/rudder -1\r\nset controls/flight/rudder 1\r\n";
            this.ip = "127.0.0.1";
            this.cmdPort = 5400;
            this.infoPort = 5402;



            this.isAutoPilot = false;
            this.isJoystick = false;
        }

        
        public float Lon
        {
            get
            {
                return this.lon;
            }
            set
            {
                this.lon = value;
            }
        }

        public float Lat
        {
            get { return this.lat; }
            set
            {
                this.lat = value;
            }
        }
        

        public string Script
        {
            get { return this.script; }
            set { 
                    this.script = value;;
                }
        }

        public string IP
        {
            get { return this.ip; }
            set { this.ip = value; }
        }

        public int CommandPort
        {
            get { return this.cmdPort; }
            set { this.cmdPort = value;}
        }

        public int InfoPort
        {
            get { return this.infoPort; }
            set { this.infoPort = value;}
        }



        public void sendAutoPilotCommands()
        {
            this.isAutoPilot = true;
        }


        public void openClientThread()
        {
            new Thread(new ThreadStart(connectAsClient)).Start();
        }

        public void connectAsClient()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(this.ip), this.cmdPort);
            TcpClient client = new TcpClient();
            client.Connect(ep);
            Console.WriteLine("You are connected");
            using (NetworkStream stream = client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                while(true)
                {
                    if(isAutoPilot)
                    {
                        string[] stringSeparators = new string[] { "\r\n" };
                        string[] cmds = script.Split(stringSeparators, StringSplitOptions.None);

                        for(int j=0; j < cmds.Length; ++j)
                        {
                            String str = cmds[j] + "\r\n";

                            Stream stm = client.GetStream();

                            ASCIIEncoding asen = new ASCIIEncoding();
                            byte[] ba = asen.GetBytes(str);

                            stm.Write(ba, 0, ba.Length);
                        }

                        this.isAutoPilot = false;
                    }
                    if(isJoystick)
                    {
                        writer.Write("hi");
                    }
                }
            }
            client.Close();
        }

        public void openServerThread()
        {
            new Thread(new ThreadStart(connectAsServer)).Start();
        }

        public void connectAsServer()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), infoPort);
            TcpListener listener = new TcpListener(ep);
            listener.Start();
            Console.WriteLine("Waiting for client connections...");
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");
            using (NetworkStream stream = client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                while(true)
                {

                    Console.WriteLine("Waiting for a number");
                    string[] stringSeparators = new string[] { "," };
                    string[] info = reader.ReadString().Split(stringSeparators, StringSplitOptions.None);
                    this.lat = float.Parse(info[0]);
                    this.lon = float.Parse(info[1]);
                }
            }
            client.Close();
            listener.Stop();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
