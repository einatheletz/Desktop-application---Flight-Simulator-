using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FlightSimulator.ViewModels.Windows
{
    public class SettingsWindowViewModel : BaseNotify
    {
        private ISettingsModel model;
        public event EventHandler OnRequestClose;

        public SettingsWindowViewModel(ISettingsModel model)
        {
            this.model = model;
        }

        /*
         * The Flight Simulator IP property
         */
        public string FlightServerIP
        {
            get { return model.FlightServerIP; }
            set
            {
                model.FlightServerIP = value;
                NotifyPropertyChanged("FlightServerIP");
            }
        }

        /*
         * The Flight Simulator command port property
         */
        public int FlightCommandPort
        {
            get { return model.FlightCommandPort; }
            set
            {
                model.FlightCommandPort = value;
                NotifyPropertyChanged("FlightCommandPort");
            }
        }

        /*
         * The Flight Simulator command info property
         */
        public int FlightInfoPort
        {
            get { return model.FlightInfoPort; }
            set
            {
                model.FlightInfoPort = value;
                NotifyPropertyChanged("FlightInfoPort");
            }
        }

        /*
         * Calling the SaveSettings function in the model
         */
        public void SaveSettings()
        {
            model.SaveSettings();
        }

        /*
         * Calling the ReloadSetting function in the model
         */
        public void ReloadSettings()
        {
            model.ReloadSettings();
        }

        #region Commands
        #region ClickCommand
        /*
         * This command is binded to the "Ok" button and saves the settings data
         */
        private ICommand _okCommand;
        public ICommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new CommandHandler(() => OkClick()));
            }
        }
        private void OkClick()
        {
            model.SaveSettings();
            OnRequestClose(this, null);
        }
        #endregion

        #region CancelCommand

        /*
         * This command is binded to the "Clear" button and clears the settings data
         */
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new CommandHandler(() => OnCancel()));
            }
        }
        private void OnCancel()
        {
            model.ReloadSettings();
            OnRequestClose(this, null);
        }
        #endregion
        #endregion
    }
}

