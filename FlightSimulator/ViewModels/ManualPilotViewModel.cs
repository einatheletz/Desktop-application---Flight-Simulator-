using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FlightSimulator.Model.EventArgs;
using FlightSimulator.Model;
using System.Windows.Input;

namespace FlightSimulator.ViewModels
{
    class ManualPilotViewModel : BaseViewModel
    {
        private ApplicationModel VJEA;

        /*
         * This class has 4 properties which represents each data variable from
         * the Flight Simulator which can changed by user using the GUI
         */
        #region Propeties
        private double rudder;
        public double Rudder
        {
            get
            {
                return this.rudder;
            }
            set
            {
                if (rudder != value)
                {
                    this.rudder = value;
                    this.VJEA.Io.UpdateDataInSimulator("Rudder", value);
                    OnPropertyChanged();
                }
                
            }
        }

        private double throttle;
        public double Throttle
        {
            get
            {
                return this.throttle;
            }
            set
            {
                if (throttle != value)
                {
                    this.throttle = value;
                    this.VJEA.Io.UpdateDataInSimulator("Throttle", value);
                    OnPropertyChanged();
                }
            }
        }

        private double elevator;
        public double Elevator
        {
            get
            {
                return this.elevator;
            }
            set
            {
                if (elevator != value)
                {
                    this.elevator = value;
                    this.VJEA.Io.UpdateDataInSimulator("Elevator", value);
                    OnPropertyChanged();
                }
            }
        }

        private double ailron;
        public double Ailron
        {
            get
            {
                return this.ailron;
            }
            set
            {
                if (ailron != value)
                {
                    this.ailron = value;
                    this.VJEA.Io.UpdateDataInSimulator("Ailron", value);
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        /*
         * This function is added to the Jostick "Moved" event which is called
         * when the Joystick is moving and updates the View Model properties with
         * it's new values after the Joystick move.
         */
        public void UpdateData(object Sender, VirtualJoystickEventArgs VJEA)
        {
            this.Ailron = VJEA.Aileron;
            this.Elevator = VJEA.Elevator;
        }

        public ManualPilotViewModel(ApplicationModel VJEA)
        {
            this.VJEA = VJEA;
        }
    }
}