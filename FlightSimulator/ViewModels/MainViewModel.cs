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

namespace FlightSimulator.ViewModels
{
 
    public class MainViewModel
    {
        private SettingsViewModel settingsViewModel;
        private Settings settings;

        public MainViewModel()
        {
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
       
    }
}
