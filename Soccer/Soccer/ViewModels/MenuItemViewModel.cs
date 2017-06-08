﻿using GalaSoft.MvvmLight.Command;
using Soccer.Models;
using Soccer.Services;
using System.Windows.Input;

namespace Soccer.ViewModels
{
    public class MenuItemViewModel
    {
        #region Attributes
        private NavigationService navigationService;
        private DataService dataService;
        #endregion

        #region Properties
        public string Icon { get; set; }

        public string Title { get; set; }

        public string PageName { get; set; }
        #endregion

        #region Constructor
        public MenuItemViewModel()
        {
            navigationService = new NavigationService();
            dataService = new DataService();
        }
        #endregion

        #region Commands
        public ICommand NavigateCommand { get { return new RelayCommand(Navigate); } }

        private async void Navigate()
        {
            var mainViewModel = MainViewModel.GetInstance();

            if (PageName == "LoginPage")
            {
                mainViewModel.CurrentUser.IsRemembered = false;
                dataService.Update(mainViewModel.CurrentUser);
                navigationService.SetMainPage("LoginPage");
            }
            else
            {
                switch (PageName)
                {
                    case "SelectTournamentPage":
                        var parameters = dataService.First<Parameter>(false);
                        parameters.Option = Title;
                        dataService.Update(parameters);
                        mainViewModel.SelectTournament = new SelectTournamentViewModel();                        
                        await navigationService.Navigate(PageName);
                        break;
                    case "ConfigPage":
                        mainViewModel.Config = new ConfigViewModel(mainViewModel.CurrentUser);
                        await navigationService.Navigate(PageName);
                        break;
                    case "SelectUserGroupPage":
                        mainViewModel.SelectUserGroup = new SelectUserGroupViewModel();
                        await navigationService.Navigate(PageName);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
