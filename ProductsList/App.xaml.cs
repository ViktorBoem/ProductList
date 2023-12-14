using Mopups.Services;

namespace ProductsList
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                MainPage = new ProductsList.ProductList_Platforms.Android.ProductList_Android();
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                MainPage = new ProductsList.ProductList_Platforms.Windows.ProductList_Window();
            }
        }
    }
}
