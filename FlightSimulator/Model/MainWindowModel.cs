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


        public string FlightServerIP
        {
            get { return model.FlightServerIP; }
            set
            {
                model.FlightServerIP = value;
                NotifyPropertyChanged("FlightServerIP");
            }
        }

        public string Script
        {
            get { return this.script; }
            set { 
                    this.script = value;
                    NotifyPropertyChanged("Script");
                }
        }

        public string IP
        {
            get { return this.ip; }
            set { this.ip = value; NotifyPropertyChanged("IP");}
        }

        public int CommandPort
        {
            get { return this.cmdPort; }
            set { this.cmdPort = value; NotifyPropertyChanged("CommandPort");}
        }

        public int InfoPort
        {
            get { return this.infoPort; }
            set { this.infoPort = value; NotifyPropertyChanged("InfoPort");}
        }

        public void sendAutoPilotCommands()
        {
            /*try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect(this.ip, this.cmdPort);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");


                // Console.Write("Enter the string to be transmitted : ");
                // String str = Console.ReadLine();

                string[] stringSeparators = new string[] { "\r\n" };
                string[] cmds = script.Split(stringSeparators, StringSplitOptions.None);

                for(int j=0; j < cmds.Length; ++j)
                {
                    String str = cmds[j] + "\r\n";

                    Stream stm = tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(str);
                    Console.WriteLine("Transmitting.....");

                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);

                    for (int i = 0; i < k; i++)
                        Console.Write(Convert.ToChar(bb[i]));
                }

                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }*/
            this.isAutoPilot = true;
        }


        public void openClientThread()
        {
            new Thread(new ThreadStart(connectAsClient)).Start();
        }

        public void connectAsClient()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(this.ip), this.CommandPort);
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
                    int num = reader.ReadInt32();
                    Console.WriteLine("Number accepted");
                    num *= 2;
                    writer.Write(num);
                    writer.Flush();
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
