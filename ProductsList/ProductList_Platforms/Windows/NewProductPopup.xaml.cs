namespace ProductsList.ProductList_Platforms.Windows;
using Mopups.Services;

// Клас для вікна додавання нового продукту
public partial class NewProductPopup
{
    ProductList_Window productListWindow;// Зберігає посилання на головне вікно списку продуктів

    // Конструктор класу NewProductPopup, ініціалізує компоненти
    public NewProductPopup(ProductList_Window mainWindow)
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

    // Метод обробки кліка по кнопці додавання продукту
    private void AddProductButtonCliced(object sender, EventArgs e)
    {
        if (ProductType.SelectedIndex < 0)
        {
            DisplayAlert("Помилка", "Будь ласка, виберіть тип продукту.", "OK");
            return;
        }

        if (ProductCurrency.SelectedIndex < 0)
        {
            DisplayAlert("Помилка", "Будь ласка, виберіть валюту.", "OK");
            return;
        }

        try
        {
            string productType = ProductType.Items[ProductType.SelectedIndex];
            string productCurrency = ProductCurrency.Items[ProductCurrency.SelectedIndex];
            int price = Convert.ToInt32(ProductPrice.Text);

            if (price < 0 || price == 0)
            {
                DisplayAlert("Помилка", "Ціна не може бути від'ємною.", "OK");
                return;
            }

            productListWindow.AddProductToList(productType, price, productCurrency);
            MopupService.Instance.PopAsync();
        }
        catch (FormatException)
        {
            DisplayAlert("Помилка", "Будь ласка, введіть дійсне значення ціни.", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Помилка", $"Виникла помилка: {ex.Message}", "OK");
        }
    }

    // Метод закриття вікна додавання нового продукту
    private void СloseNewProductPopup(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}