using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FlightSimulator.Model.Interface;

namespace FlightSimulator.Model
{
    class ApplicationModel : ISettingsModel
    {
        private static ApplicationModel instance = null;
        private static readonly object padlock = new object();
        public Thread n_server { get; set; }
        public Thread ConnectionThread { get; set; }
        public bool isConnected { get; set; }
        public TcpClient client { get; set; }
        public TcpListener server { get; set; }
        private IO io;
        public IO Io
        {
            get
            {
                return this.io;
            }
            set
            {
                this.io = value;
            }
        }

        #region ConnectionData
        public string FlightServerIP
        {
            get { return Properties.Settings.Default.FlightServerIP; }
            set { Properties.Settings.Default.FlightServerIP = value; }
        }
        public int FlightCommandPort
        {
            get { return Properties.Settings.Default.FlightCommandPort; }
            set { Properties.Settings.Default.FlightCommandPort = value; }
        }

        public int FlightInfoPort
        {
            get { return Properties.Settings.Default.FlightInfoPort; }
            set { Properties.Settings.Default.FlightInfoPort = value; }
        }
        #endregion

        ApplicationModel()
        {
            this.io = new IO();
            this.client = new TcpClient(); // create client
            this.io.client = this.client;
            this.server = new TcpListener(IPAddress.Parse(FlightServerIP), FlightInfoPort); // create a server
        }

        /*
         * Implements Singelton design pattren to the ApplicationModel class.
         */
        public static ApplicationModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ApplicationModel();
                    }
                    return instance;
                }
            }
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }

        public void ReloadSettings()
        {
            Properties.Settings.Default.Reload();
        }

        /*
         * This function is called when the "Connect" button is clicked.
         * The function creates new thread for the server and start it.
         * Moreover, the function accepts a client (the Flight Simulator)
         * to it's server and connect as a client to the Flight Simulator.
         */
        public void Connect()
        {
            this.ConnectionThread = new Thread(new ThreadStart(ConnectInOtherThread));
            this.ConnectionThread.Start();
        }

        public void ConnectInOtherThread()
        {
            this.n_server = new Thread(new ThreadStart(Server));
            this.server.Start();
            this.io.socket = this.server.AcceptSocket();
            this.client.Connect(FlightServerIP, FlightCommandPort);
            this.isConnected = true;
            this.n_server.Start();
            this.ConnectionThread.Abort();
        }

        /*
         * This function works in a new thread.
         * The function call a function from the Io class which
         * recives data from the simulator
         */
        public void Server()
        {
            this.io.ReadDataFromSimulator(this.server);
        }
    }
}