using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlightSimulator.ViewModels;
using FlightSimulator.Model.EventArgs;
using FlightSimulator.Model;

namespace FlightSimulator.Views
{
    /// <summary>
    /// Interaction logic for ManualPilot.xaml
    /// </summary>
    public partial class ManualPilot : UserControl
    {
        public ManualPilot()
        {
            InitializeComponent();
            ManualPilotViewModel MPVM = new ManualPilotViewModel(ApplicationModel.Instance);
            this.DataContext = MPVM;
            this.Joystick.Moved += MPVM.UpdateData;
        }
    }
}
