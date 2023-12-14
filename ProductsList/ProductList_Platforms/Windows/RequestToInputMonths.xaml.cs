using Mopups.Services;
using System.Globalization;

namespace ProductsList.ProductList_Platforms.Windows;

// Клас для вікна запиту на введення місяця і року
public partial class RequestToInputMonths
{
    ProductList_Window productListWindow;// Зберігає посилання на головне вікно списку продуктів

    // Конструктор класу RequestToInputMonths, ініціалізує компоненти
    public RequestToInputMonths(ProductList_Window mainWindow)
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

    // Асинхронний метод для аналізу статистики покупок продовольчих продуктів за місяць і рік
    private async void PurchaseStatistics(object sender, EventArgs e)
    {
        var userInput = EnteredMonth.Text;
        DateTime parsedDate;

        if (DateTime.TryParseExact(userInput, "MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            int year = parsedDate.Year;
            int month = parsedDate.Month;

            string message = productListWindow.productList.GetStatisticsOfFoodProductsUnder50UAH(year, month);
            await MopupService.Instance.PopAsync();
            await MopupService.Instance.PushAsync(new MessagePopup(message));
        }
        else
        {
            await DisplayAlert("Помилка", "будь ласка, введіть дані у форматі мм.рррр", "OK");
        }
    }

    // Метод закриття вікна запиту на введення місяця і року
    private void RequestToInputDate_close(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}