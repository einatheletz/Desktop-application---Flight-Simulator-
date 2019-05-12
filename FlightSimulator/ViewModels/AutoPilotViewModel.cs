using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FlightSimulator.Model;
using FlightSimulator.ViewModels;

namespace FlightSimulator.ViewModels
{
    class AutoPilotViewModel : BaseNotify
    {
        public ApplicationModel AM { get; set; }
        // a boolean member that indicates if the user is started to right when the
        // Text Box has triggered
        public bool isFirstLetter { get; set; }

        public AutoPilotViewModel(ApplicationModel AM)
        {
            this.AM = AM;
            isFirstLetter = true;
            this.IsPink = false;
        }

        /*
         * This property is binded to the text in the Auto Pilot tab Text Box 
         */
        private string textFromTextBox;
        public string TextFromTextBox
        {
            get
            {
                return this.textFromTextBox;
            }
            set
            {
                this.textFromTextBox = value;
                NotifyPropertyChanged("TextFromTexrBox");
                // If the text box has changed and the change is not updated in
                // simulator, change the boolean member IsPink to be true and then
                // the text box background color will change to pink
                if (isFirstLetter)
                {
                    IsPink = true;
                    isFirstLetter = false;
                }
            }
        }

        /*
         * When this property is true, the Auto Pilot Text Box background color
         * will change to pink. Otherwise it will be white.
         */
        private bool isPink;
        public bool IsPink
        {
            get
            {
                return isPink;
            }
            set
            {
                isPink = value;
                NotifyPropertyChanged("IsPink");
            }

        }

        #region Commands
        #region OkCommand
        /*
         * This command is binded to the "Ok" button
         */
        private ICommand _okCommand;
        public ICommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new CommandHandler(() => OkClick()));
            }
        }
        /*
         * When the "Ok" button is clicked, this function will be called.
         * The text box color will change to white and the data from it will be
         * sent to a function in "Io" class which sends all the commands written
         * in the text box to the simulator.
         */
        private void OkClick()
        {
            IsPink = false;
            NotifyPropertyChanged("IsPink");
            String temp = TextFromTextBox;
            TextFromTextBox = "";
            NotifyPropertyChanged("TextFromTextBox");
            isFirstLetter = true;
            this.AM.Io.SendCommandToSimulator(temp);
        }
        #endregion

        #region ClearCommand
        /*
         * This command is binded to the "Clear" button
         */
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new CommandHandler(() => OnClear()));
            }
        }
        /*
         * The function will change the TextFromTextBox property which is binded
         * to the text from text box to be empty
         */
        private void OnClear()
        {
            TextFromTextBox = "";
            NotifyPropertyChanged("TextFromTextBox");
        }
        #endregion
        #endregion
    }
}
