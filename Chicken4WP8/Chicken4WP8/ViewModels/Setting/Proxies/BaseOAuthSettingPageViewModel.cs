﻿using System;
using Caliburn.Micro;
using Chicken4WP8.Controllers.Interface;
using Chicken4WP8.Models.Setting;
using Chicken4WP8.Services.Interface;
using Chicken4WP8.ViewModels.Home;
using Chicken4WP8.Views.Setting.Proxies;
using Microsoft.Phone.Controls;

namespace Chicken4WP8.ViewModels.Setting.Proxies
{
    public class BaseOAuthSettingPageViewModel : Screen
    {
        #region properties
        private const string KEY = "pPnxpn00RbGx3YJJtvYUsA";
        private const string SECRET = "PoX3exts23HJ1rlMaPr6RtlX2G5VQdrqbpUWpkMcCo";
        private OAuthSessionModel session;
        private readonly WaitCursor waitCursorService;

        public IStorageService StorageService { get; set; }
        public IBaseOAuthController BaseOAuthController { get; set; }
        public INavigationService NavigationService { get; set; }
        public ILanguageHelper LanguageHelper { get; set; }

        public BaseOAuthSettingPageViewModel()
        {
            waitCursorService = WaitCursorService.WaitCursor;
        }
        #endregion

        private string pin;
        public string PinCode
        {
            get { return pin; }
            set
            {
                pin = value;
                NotifyOfPropertyChange(() => PinCode);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            waitCursorService.Text = LanguageHelper["WaitCursor_Loading"];
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            waitCursorService.Text = LanguageHelper["WaitCursor_GetAuthorizationPage"];
            waitCursorService.IsVisible = true;

            session = await BaseOAuthController.AuthorizeAsync(KEY, SECRET);

            var page = view as BaseOAuthSettingPageView;
            var browser = page.Browser;
            browser.Navigated += (o, e) =>
            {
                waitCursorService.IsVisible = false;
            };
            browser.NavigationFailed += (o, e) => BrowserNavigationFailed(e.Exception);

            try
            {
                browser.Navigate(session.AuthorizeUri);
            }
            catch (Exception e)
            {
                BrowserNavigationFailed(e);
            }
        }

        public async void AppBar_Finish()
        {
            if (string.IsNullOrEmpty(PinCode))
                return;
            waitCursorService.IsVisible = true;
            waitCursorService.Text = LanguageHelper["WaitCursor_GetCredentials"];

            var setting = StorageService.GetCurrentUserSetting();
            if (setting == null)
                setting = new UserSetting();

            var oauth = await BaseOAuthController.GetTokensAsync(PinCode);
            setting.OAuthSetting = oauth;

            waitCursorService.Text = LanguageHelper["WaitCursor_GetCurrentUser"];

            var user = await BaseOAuthController.VerifyCredentialsAsync(oauth);
            setting.Id = user.Id;
            setting.Name = user.Name;
            setting.ScreenName = user.ScreenName;

            StorageService.UpdateCurrentUserSetting(setting);
            App.UpdateSetting(setting);

            waitCursorService.IsVisible = false;
            NavigationService.UriFor<HomePageViewModel>().Navigate();
        }

        private async void BrowserNavigationFailed(Exception exception)
        {
            waitCursorService.IsVisible = false;

            var messageBox = new CustomMessageBox
            {
                Caption = LanguageHelper["WaitCursor_AnErrorHappened"],
                Message = exception.Message,
                LeftButtonContent = LanguageHelper["Button_OK"],
                RightButtonContent = LanguageHelper["Button_Cancel"],
                IsFullScreen = false
            };
            await messageBox.ShowAsync();
        }
    }
}
