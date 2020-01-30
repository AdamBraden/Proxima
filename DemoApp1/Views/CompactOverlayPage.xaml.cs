using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.WindowManagement;
using Windows.UI.WindowManagement.Preview;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;

namespace DemoApp1.Views
{
    public sealed partial class CompactOverlayPage : Page, INotifyPropertyChanged
    {
        private AppWindow appWindow;
        private Frame appWindowFrame = new Frame();
        // Lower bounds on window size to keep things from being unresonably tiny.
        const double MinWindowWidth = 192;
        const double MinWindowHeight = 48;

        public CompactOverlayPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (appWindow != null)
            {
                await appWindow.CloseAsync();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void ShowWindowBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            double windowWidth = 500;
            double windowHeight = 500;

            //if (windowWidth < MinWindowWidth || windowHeight < MinWindowHeight)
            //{
            //    MainPage.Current.NotifyUser($"Please specify a width of at least {MinWindowWidth} and a height of at least {MinWindowHeight}.", NotifyType.ErrorMessage);
            //    return;
            //}

            showWindowBtn.IsEnabled = false;

            // Only ever create and show one window. If the AppWindow exists use it
            if (appWindow == null)
            {
                // Create a new window
                appWindow = await AppWindow.TryCreateAsync();
                // Make sure we release the reference to this window, and release XAML resources, when it's closed
                appWindow.Closed += delegate { appWindow = null; appWindowFrame.Content = null; };
                // Navigate the frame to the page we want to show in the new window
                appWindowFrame.Navigate(typeof(SecondaryAppWindowPage));
            }

            // If specified size is smaller than the default min size for a window we need to set a new preferred min size first.
            // Let's set it to the smallest allowed and leave it at that.
            if (windowWidth < 500 || windowHeight < 320)
            {
                WindowManagementPreview.SetPreferredMinSize(appWindow, new Size(MinWindowWidth, MinWindowHeight));
            }
            // Request the size of our window
            appWindow.RequestSize(new Size(windowWidth, windowHeight));
            // Attach the XAML content to our window
            ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowFrame);

            // If the window is not visible, show it and/or bring it to foreground
            if (!appWindow.IsVisible)
            {
                await appWindow.TryShowAsync();
            }

            showWindowBtn.IsEnabled = true;
        }
    }
}
