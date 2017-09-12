using GalaSoft.MvvmLight.Messaging;
using Migrate.UWP.Messages;
using System;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Migrate.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _resultText;
        private CoreDispatcher _dispatcher;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var x = SystemInformation.OperatingSystem == "";

            if (AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Desktop")
            {
                OpenForm.Visibility = Visibility.Collapsed;
            }

            Messenger.Default.Register<ConnectionReadyMessage>(this, message =>
            {
                if (App.Connection != null)
                {
                    App.Connection.RequestReceived += Connection_RequestReceived;
                }
            });

            var currentView = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView();
            _dispatcher = currentView.CoreWindow.Dispatcher;
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var deferral = args.GetDeferral();
            string name = args.Request.Message["name"].ToString();
            _resultText = name;

            //Result.Text = $"Hello {name}";
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, ShowMessageResult);

            ValueSet valueSet = new ValueSet();
            valueSet.Add("response", "success");
            await args.Request.SendResponseAsync(valueSet);
            deferral.Complete();
        }

        private async void OnOpenForm(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
        }

        private void OnSayHello(object sender, RoutedEventArgs e)
        {
            Result.Text = "Hello world!";
        }

        private void ShowMessageResult()
        {
            Result.Text = $"Hello {_resultText}";
        }
    }
}
