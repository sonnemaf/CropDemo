using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CropDemo {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();
        }

        private async void ButtonOpen_Click(object sender, RoutedEventArgs e) {

            // Load a File Picker that shows image file types
            FileOpenPicker open = new FileOpenPicker();
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpeg");
            open.ViewMode = PickerViewMode.Thumbnail;
            open.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            open.CommitButtonText = "Open";

            // Wait for user to select a file 
            StorageFile source = await open.PickSingleFileAsync();

            // Verify the source is not null
            if (source != null) {
                try {
                    // Create a destination file
                    StorageFile dest = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Cropped.jpg", CreationCollisionOption.ReplaceExisting);

                    // Call CropImageAsync and receive Result
                    LaunchUriResult result = await this.CropImageAsync(source, dest, 500, 500);

                    // Load Destination Image into Image Preview
                    var stream = await dest.OpenReadAsync();

                    var bm = new BitmapImage();
                    await bm.SetSourceAsync(stream);

                    this.croppedImage.Source = bm;

                } catch {
                    MessageDialog md = new MessageDialog("Error loading image file.");
                    await md.ShowAsync();
                }

            }
        }

        private async void ButtonSelect_Click(object sender, RoutedEventArgs e) {
            try {
                // Create a destination file
                StorageFile dest = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Cropped.jpg", CreationCollisionOption.ReplaceExisting);

                // Call CropImageAsync and receive Result
                LaunchUriResult result = await this.CropImageAsync(dest, 500, 500);

                // Load Destination Image into Image Preview
                var stream = await dest.OpenReadAsync();

                var bm = new BitmapImage();
                await bm.SetSourceAsync(stream);

                this.croppedImage.Source = bm;

            } catch {
                MessageDialog md = new MessageDialog("Error loading image file.");
                await md.ShowAsync();
            }
        }

        private async Task<LaunchUriResult> CropImageAsync(IStorageFile destination, int width, int height) {
            // Get access tokens to pass input and output files between apps
            var destinationToken = SharedStorageAccessManager.AddFile(destination);

            // Specify an app to launch by using LaunchUriForResultsAsync
            var options = new LauncherOptions();
            options.TargetApplicationPackageFamilyName = "Microsoft.Windows.Photos_8wekyb3d8bbwe";

            // Specify protocol launch options
            var parameters = new ValueSet();
            parameters.Add("DestinationToken", destinationToken);
            parameters.Add("CropWidthPixels", width);
            parameters.Add("CropHeightPixels", height);
            parameters.Add("EllipticalCrop", false);
            parameters.Add("ShowCamera", true);

            // Perform LaunchUriForResultsAsync
            return await Launcher.LaunchUriForResultsAsync(new Uri("microsoft.windows.photos.crop:"), options, parameters);

            // See also: 
            // https://frayxrulez.wordpress.com/tag/microsoft-windows-photos-crop/ 
            // https://gist.github.com/c2f1bbfa996ad5751b87 
            // https://myignite.microsoft.com/videos/2571
        }

        private async Task<LaunchUriResult> CropImageAsync(IStorageFile input, IStorageFile destination, int width, int height) {
            // Get access tokens to pass input and output files between apps
            var inputToken = SharedStorageAccessManager.AddFile(input);
            var destinationToken = SharedStorageAccessManager.AddFile(destination);

            // Specify an app to launch by using LaunchUriForResultsAsync
            var options = new LauncherOptions();
            options.TargetApplicationPackageFamilyName = "Microsoft.Windows.Photos_8wekyb3d8bbwe";

            // Specify protocol launch options
            var parameters = new ValueSet();
            parameters.Add("InputToken", inputToken);
            parameters.Add("DestinationToken", destinationToken);
            parameters.Add("CropWidthPixels", width);
            parameters.Add("CropHeightPixels", height);
            parameters.Add("EllipticalCrop", false);
            parameters.Add("ShowCamera", false);

            // Perform LaunchUriForResultsAsync
            return await Launcher.LaunchUriForResultsAsync(new Uri("microsoft.windows.photos.crop:"), options, parameters);

            // See also: 
            // https://frayxrulez.wordpress.com/tag/microsoft-windows-photos-crop/ 
            // https://gist.github.com/c2f1bbfa996ad5751b87 
            // https://myignite.microsoft.com/videos/2571
        }
    }
}
