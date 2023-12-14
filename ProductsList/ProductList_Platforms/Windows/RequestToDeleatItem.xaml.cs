using Mopups.Services;

namespace ProductsList.ProductList_Platforms.Windows;

// Клас вікна запиту на видалення елементу
public partial class RequestToDeleatItem
{
    ProductList_Window productListWindow;// Змінна для зберігання посилання на основне вікно продуктового списку

    // Конструктор класу, ініціалізує компоненти і зберігає посилання на основне вікно
    public RequestToDeleatItem(ProductList_Window mainWindow)
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

    // Асинхронний метод для видалення елементу
    private async void DeleatElement(object sender, EventArgs e)
    {
        productListWindow.productList.RemoveProduct(productListWindow.selectedItemProductListTable);
        productListWindow.RemoveGridFromTable(productListWindow.selectedItemTable);
        await MopupService.Instance.PopAsync();
    }

    // Метод для закриття вікна запиту на видалення
    private void RequestDeletItem_close(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}