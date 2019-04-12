using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FlightSimulator.ViewModels.Windows;
using FlightSimulator.Commands;
using FlightSimulator.Model;
using System.Windows;
using FlightSimulator.Views.Windows;
using System.ComponentModel;

namespace FlightSimulator.ViewModels
{
 
    public class MainViewModel : ViewModel
    {
        private SettingsViewModel settingsViewModel;
        private Settings settings;
        private MainWindowModel model;

        public MainViewModel()
        {
            this.model = new MainWindowModel();
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

            this.model.IP = this.settingsViewModel.FlightServerIP;
            this.model.CommandPort = this.settingsViewModel.FlightCommandPort;
            this.model.InfoPort = this.settingsViewModel.FlightInfoPort;
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

        private void ConnectOnClick()
        {
            
            this.settingsViewModel.ReloadSettings();
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

        public string Script
        {
            get { return this.model.Script; }
            set
            {
                model.Script = value;
                NotifyPropertyChanged("Script");
            }
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
