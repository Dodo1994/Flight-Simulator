using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulator.Model
{
    public class MainWindowModel
    {
        private string script;
        private int cmdPort;
        private int infoPort;
        private string ip;

        public MainWindowModel()
        {
            // some defaults
            this.script = "set controls/flight/rudder -1\r\nset controls/flight/rudder 1\r\n";

            this.ip = "127.0.0.1";
            this.cmdPort = 5400;
            this.infoPort = 5402;
        }


        public string Script
        {
            get { return this.script; }
            set { this.script = value; }
        }

        public string IP
        {
            get { return this.ip; }
            set { this.ip = value; }
        }

        public int CommandPort
        {
            get { return this.cmdPort; }
            set { this.cmdPort = value; }
        }

        public int InfoPort
        {
            get { return this.infoPort; }
            set { this.infoPort = value; }
        }

        public void sendAutoPilotCommands()
        {
            try
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
            }

        }
    }
}
