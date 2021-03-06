﻿using Caliburn.Micro;
using Chicken4WP8.Services.Interface;

namespace Chicken4WP8.ViewModels.Setting.Proxies
{
    public class TwipOAuthSettingPageViewModel : Screen
    {
        public ILanguageHelper LanguageHelper { get; set; }

        public TwipOAuthSettingPageViewModel()
        {
            baseUrl = "https://wxt2005.org/tapi/o/77Z655/";
        }

        private string baseUrl;
        public string BaseUrl
        {
            get { return baseUrl; }
            set
            {
                baseUrl = value;
                NotifyOfPropertyChange(() => BaseUrl);
            }
        }

        private string name;
        public string UserName
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public async void Finish()
        {
            if (!string.IsNullOrEmpty(BaseUrl))
            {
                //TwitterResources.BaseUrl = BaseUrl;
                //var user = await User.GetLoggedUserAsync();
                //UserName =  user.Name;

                //MessageBox.Show(UserName);
            }
        }
    }
}
