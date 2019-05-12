using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FlightSimulator.Model
{
    public delegate void handler();
    public class IO
    {
        public event handler IoEvent;
        public Socket socket { get; set; }
        public TcpClient client { get; set; }
        public string command;
        public Thread newThread;
        private Point lonAndLat;
        public Point LonAndLat
        {
            get
            {
                return this.lonAndLat;
            }
            set
            {
                this.lonAndLat = value;
                // notify the view model that this property is changed
                IoEvent?.Invoke();
            }
        }

        /*
         * This function is not stop running until the connection to the Flight 
         * Simulator is ending.
         * The function reads all the data from the simulator as define in
         * the Generic Small file and, call to another function to parse the data
         * and in the end create a point from the Lon and Lat data.
         */
        public void ReadDataFromSimulator(TcpListener client)
        {
            byte[] Buffer = new byte[1024];
            bool isEndOfLine;
            int recv = 0;
            int EndOfLine = 0;
            String StringData = "";
            String Result = "";
            String Remainder = "";
            while (true)
            {
                StringData = "";
                Array.Clear(Buffer, 0, Buffer.Length);
                // reads data from simulator into buffer in bytes
                recv = this.socket.Receive(Buffer);
                // convert bytes recieved into a string
                StringData = Encoding.ASCII.GetString(Buffer, 0, recv);
                Result = Remainder;
                isEndOfLine = true;
                // finding the closest end of line
                while (isEndOfLine)
                {
                    EndOfLine = StringData.IndexOf('\n');
                    if (EndOfLine != -1)
                    {
                        // An end of line is found, the function adds the remaining
                        // data into the Result string and take it of from StringData.
                        Result += StringData.Substring(0, EndOfLine);
                        StringData = StringData.Substring(EndOfLine + 1);
                        ParseAndUpdate(Result);
                        // clear Result and Buffer
                        Result = "";
                        Remainder = "";
                    }
                    else
                    {
                        // An end of line is not found, move the data to the remainder
                        // and start loop again
                        Remainder += StringData;
                        isEndOfLine = false;
                    }
                }
            }
        }

        /*
         * This function receives a string which contains all data recieved from
         * simulator in a single time and extracting the Lon and Lat properties. 
         */
        public void ParseAndUpdate(String StringData)
        {
            int StartOfLon = 0;
            int EndOfLon = StringData.IndexOf(',', StartOfLon);
            // Extract the Lon property from the data string by finding the closest
            // ',' to it from start.
            double Lon = Double.Parse(StringData.Substring(StartOfLon, EndOfLon - StartOfLon));
            int StartOfLat = EndOfLon + 1;
            int EndOfLat = StringData.IndexOf(',', StartOfLat);
            // Extract the Lat property from the data string by finding the closest
            // ',' to it after the Lon.
            double Lat = Double.Parse(StringData.Substring(StartOfLat, EndOfLat - StartOfLat));
            this.LonAndLat = new Point(Lat, Lon);
        }

        /*
         * This function send a command to the simulator in a new thread with
         * the function "FunctionInThread"
         */
        public void SendCommandToSimulator(String command)
        {
            ApplicationModel AM = ApplicationModel.Instance;
            if (AM.isConnected)
            {
                this.command = command;
                this.newThread = new Thread(new ThreadStart(FunctionInThread));
                this.newThread.Start();
            }
        }

        /*
         * This function recieved a string which represents at least one command
         * that should sent to simulator.
         * The function parse that string using an end of line symbol, send each
         * command separately in 2 sec difference (as required). 
         */
        public void FunctionInThread()
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            Stream stream = this.client.GetStream();
            string[] CommandsArray = command.Split(new[] { "\r\n", "\r", "\n", ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string Command in CommandsArray)
            {
                byte[] ByteArray = asen.GetBytes(Command + "\r\n");
                stream.Write(ByteArray, 0, ByteArray.Length);
                stream.Flush();
                System.Threading.Thread.Sleep(2000);
            }
            // after each command has sent, the thread will be closed
            this.newThread.Abort();
        }

        /*
         * This function is called when a GUI component has triggred (one of the 
         * sliders or the joystick).
         * The function recived the property that need to be changed and it's new
         * value and create a set command in the Flight Simulator's language.
         */
        public void UpdateDataInSimulator(String DataName, double value)
        {
            ApplicationModel AM = ApplicationModel.Instance;
            if (AM.isConnected)
            {
                ASCIIEncoding asen = new ASCIIEncoding();
                Stream stream = this.client.GetStream();
                byte[] ByteArray;
                String command;
                // Switch case for the propery name
                switch (DataName)
                {
                    case "Ailron":
                        command = "set /controls/flight/aileron " + value + "\r\n";
                        ByteArray = asen.GetBytes(command);
                        break;
                    case "Elevator":
                        command = "set /controls/flight/elevator " + value + "\r\n";
                        ByteArray = asen.GetBytes(command);
                        break;
                    case "Throttle":
                        command = "set /controls/engines/current-engine/throttle " + value + "\r\n";
                        ByteArray = asen.GetBytes(command);
                        break;
                    case "Rudder":
                        command = "set /controls/flight/rudder " + value + "\r\n";
                        ByteArray = asen.GetBytes(command);
                        break;
                    default:
                        ByteArray = asen.GetBytes("");
                        break;
                }
                stream.Write(ByteArray, 0, ByteArray.Length);
                stream.Flush();
            }
        }
    }
}
