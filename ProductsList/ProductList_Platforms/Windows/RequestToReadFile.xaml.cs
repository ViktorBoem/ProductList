using Mopups.Services;

namespace ProductsList.ProductList_Platforms.Windows;

// Клас для вікна запиту
public partial class RequestPopup
{
    ProductList_Window productListWindow;// Зберігає посилання на головне вікно списку продуктів

    // Конструктор класу RequestPopup, ініціалізує компоненти
    public RequestPopup(ProductList_Window mainWindow)
	{
		InitializeComponent();
        productListWindow = mainWindow;
	}

    // Обробник події наведення миші на кнопку
    private void OnButtonMouseEnter(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            if (button == AddProductButton)
            {
                button.BackgroundColor = Color.FromArgb("#101630");
                button.TextColor = Color.FromArgb("#F8EFE1");
            }

            button.ScaleTo(1.1, 100);
        }
    }

    // Обробник події відведення миші від кнопки
    private void OnButtonMouseLeave(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            if (button == AddProductButton)
            {
                button.BackgroundColor = Color.FromArgb("#F8EFE1");
                button.TextColor = Color.FromArgb("#101630");
            }

            button.ScaleTo(1.0, 100);
        }
    }

    // Асинхронний метод для читання файлу
    private async void ReadFile(object sender, EventArgs e)
    {
        await productListWindow.productList.ReadProductsFromFile(productListWindow);
        await MopupService.Instance.PopAsync();
    }

    // Метод закриття вікна запиту
    private void RequestPopup_close(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}