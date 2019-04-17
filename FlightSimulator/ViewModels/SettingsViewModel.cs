using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using FlightSimulator.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FlightSimulator.ViewModels.Windows
{
    public class SettingsViewModel : BaseNotify
    {
        private ISettingsModel model;
        private Settings settings;

        public SettingsViewModel(Settings settings)
        {
            this.model = new ApplicationSettingsModel();
            this.settings = settings;
            this.settings.setDataContext(this);
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

        public int FlightCommandPort
        {
            get { return model.FlightCommandPort; }
            set
            {
                model.FlightCommandPort = value;
                NotifyPropertyChanged("FlightCommandPort");
            }
        }

        public int FlightInfoPort
        {
            get { return model.FlightInfoPort; }
            set
            {
                model.FlightInfoPort = value;
                NotifyPropertyChanged("FlightInfoPort");
            }
        }

        public void SaveSettings()
        {
            model.SaveSettings();
        }

        public void ReloadSettings()
        {
            model.ReloadSettings();
        }

        private ICommand okCommand;
        public ICommand OkCommand
        {
            get
            {
                return okCommand ??
                    (okCommand = new CommandHandler(() => OkOnClick()));
            }
        }

        private void OkOnClick()
        {
            this.SaveSettings();
            this.settings.Close();
        }


        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand ??
                    (cancelCommand = new CommandHandler(() => CancelOnClick()));
            }
        }

        private void CancelOnClick()
        {
            this.settings.Close();
        }
        
    }
}

