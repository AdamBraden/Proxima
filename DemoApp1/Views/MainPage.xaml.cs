#pragma warning disable CS0618 // Type or member is obsolete (LoggingChannel with 1 param constructor)
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace DemoApp1.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        LoggingChannel log = new LoggingChannel("SampleProvider", null);
        public MainPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine(log.Id.ToString());
        }

        #region CodeGen

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
        #endregion

        private async void Picker_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                log.LogMessage("Folder Picked");

                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                //Show the chosen path
                this.folderTextBox.Text = folder.Path;
            }
            else
            {
                this.folderTextBox.Text = "Operation cancelled.";
            }
        }

        private async void UpdateImage_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var fileImage = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Elephant.jpg");
            if (fileImage != null)
            {
                log.LogMessage("Setting Background");
                BitmapImage bmp = new BitmapImage();
                var stream = await (fileImage as StorageFile).OpenReadAsync();
                bmp.SetSource(stream);
                Background.Source = bmp;
                log.LogMessage("Setting Background: COMPLETED");
            }
            else
            {
                log.LogMessage("No Background image in LocalAppData");
            }
        }


    }
}

