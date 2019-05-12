using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FlightSimulator.ViewModels.Windows;
using FlightSimulator.Views.Windows;
using System.Windows;
using System.ComponentModel;

namespace FlightSimulator.ViewModels
{
    public delegate void handler(object sender, PropertyChangedEventArgs e);
    public class FlightBoardViewModel : BaseNotify
    {
        public event handler FVBMEvent;
        private ApplicationModel AM;
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
            }
        }

        #region Commands
        #region ClickCommand
        /*
         * This command is binded to the "Settings" button
         */
        private ICommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                return _settingsCommand ?? (_settingsCommand = new CommandHandler(() => SettingsClick()));
            }
        }
        /*
         * This function creates a new SettingsWindow which is a GUI to the 
         * settings as required and creates a SettingWindowViewModel to be it's 
         * data context
         */
        private void SettingsClick()
        {
            var swvm = new SettingsWindowViewModel(ApplicationModel.Instance);
            var sw = new SettingsWindow() { DataContext = swvm };
            swvm.OnRequestClose += (s, e) => sw.Close();
            sw.Show();
        }
        #endregion
        #endregion

        #region Commands
        #region ConnectCommand
        /*
         * This command is binded to the "Connect" button
         */
        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                return _connectCommand ?? (_connectCommand = new CommandHandler(() => ConnectClick()));
            }
        }
        /*
         * This function is calling the ApllicationModel function "Connect", which
         * connects to the Flight Simulator as client and creates a server which 
         * the Flight Simulator is it's client.
         * Moreover, this function creates a no name function to add to the "Io"
         * class event. When triggered, the function will set the View Model 
         * LonAndLat point property to be the Io's LonAndLat point and notifies the
         * View that there was a change.
         */
        private void ConnectClick()
        {
            this.AM = ApplicationModel.Instance;
            this.AM.Io.IoEvent += () =>
            {
                this.LonAndLat = this.AM.Io.LonAndLat;
                FVBMEvent?.Invoke(this, null);
            };
            AM.Connect();
        }
        #endregion

        /*
         * This command is binded to the Disconnect button and when triggred, the 
         * function closes the communication threads and the sockets. In the end,
         * the function closes the program
         */
        private ICommand _disconnectCommand;
        public ICommand DisconnectCommand
        {
            get
            {
                return _disconnectCommand ?? (_disconnectCommand = new CommandHandler(() => CloseWindow()));
            }
        }

        private void CloseWindow()
        {
            ApplicationModel AM = ApplicationModel.Instance;
            AM.n_server.Abort();
            AM.client.Close();
            AM.server.Stop();
            System.Environment.Exit(0);
        }
        #endregion
    }
}