using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FlightSimulator.ViewModels.Windows;
using FlightSimulator.Model;
using System.Windows;
using FlightSimulator.Views;
using FlightSimulator.Views.Windows;
using System.ComponentModel;
using System.Threading;

namespace FlightSimulator.ViewModels
{
 
    public class MainViewModel : ViewModel
    {
        private SettingsViewModel settingsViewModel;
        private Settings settings;
        private MainWindowModel model;
        private bool isConnect;
        private FlightBoard flightBoard;
        //private Joystick joystick;


        public MainViewModel()
        {
            this.model = new MainWindowModel();
            this.isConnect = false;

            this.flightBoard = new FlightBoard();
            this.flightBoard.setVM(this);
        }


        public float Lon
        {
            get
            {
                return this.model.Lon;
            }
            set
            {
                this.model.Lon = value;
                NotifyPropertyChanged("Lon");
            }
        }

        public float Lat
        {
            get { return this.model.Lat; }
            set
            {
                this.model.Lat = value;
                NotifyPropertyChanged("Lat");
            }
        }
        

        public string Script
        {
            get { return this.model.Script; }
            set { 
                    this.model.Script = value;
                    NotifyPropertyChanged("Script");
                }
        }

        public string IP
        {
            get { return this.model.IP; }
            set { this.model.IP = value; NotifyPropertyChanged("IP");}
        }

        public int CommandPort
        {
            get { return this.model.CommandPort; }
            set { this.model.CommandPort = value; NotifyPropertyChanged("CommandPort");}
        }

        public int InfoPort
        {
            get { return this.model.InfoPort; }
            set { this.model.InfoPort = value; NotifyPropertyChanged("InfoPort");}
        }


        private ICommand showSettingsCommand;
        public ICommand ShowSettingsCommand
        {
            get
            {
                return showSettingsCommand ?? 
                    (showSettingsCommand = new CommandHandler(() => SettingsOnClick()));
            }
        }

        private void SettingsOnClick()
        {
            this.settings = new Settings();
            this.settingsViewModel = new SettingsViewModel(this.settings);

            this.settingsViewModel.ReloadSettings();

            settings.ShowDialog();

            this.IP = this.settingsViewModel.FlightServerIP;
            this.CommandPort = this.settingsViewModel.FlightCommandPort;
            this.InfoPort = this.settingsViewModel.FlightInfoPort;
        }


        private ICommand connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                return connectCommand ??
                    (connectCommand = new CommandHandler(() => ConnectOnClick()));
            }
        }


        // here need to connect as client and as server
        private void ConnectOnClick()
        {
            if(!this.isConnect)
            {
                this.model.openClientThread();
                this.model.openServerThread();
                this.isConnect = true;
            }
            // here shows path on graph
        }

        private ICommand clearCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ClearCommand
        {
            get
            {
                return clearCommand ??
                    (clearCommand = new CommandHandler(() => ClearOnClick()));
            }
        }

        private void ClearOnClick()
        {
            this.Script = "";
        }
        

        private ICommand okScriptCommand;
 
        public ICommand OkScriptCommand
        {
            get
            {
                return okScriptCommand ??
                    (okScriptCommand = new CommandHandler(() => ScriptOnClick()));
            }
        }

        private void ScriptOnClick()
        {
            model.sendAutoPilotCommands();
        }

    }
}
