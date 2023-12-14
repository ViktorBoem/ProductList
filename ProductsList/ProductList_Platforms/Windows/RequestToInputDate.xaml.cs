using Mopups.Services;
using System.Globalization;

namespace ProductsList.ProductList_Platforms.Windows;

// Клас для вікна запиту на введення дати
public partial class RequestToInputDate
{
    ProductList_Window productListWindow;// Зберігає посилання на головне вікно списку продуктів

    // Конструктор класу RequestToInputDate, ініціалізує компоненти
    public RequestToInputDate(ProductList_Window mainWindow)
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

    // Асинхронний метод для аналізу статистики покупок
    private async void PurchaseStatistics(object sender, EventArgs e)
    {
        var userInput = EnteredDate.Text;
        DateTime parsedDate;

        if (DateTime.TryParseExact(userInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            string message = productListWindow.productList.AnalysisOfPurchaseStatistics(parsedDate);
            await MopupService.Instance.PopAsync();
            await MopupService.Instance.PushAsync(new MessagePopup(message));
        }
        else
        {
            await DisplayAlert("Помилка", "будь ласка, введіть дані у форматі дд.мм.рррр", "OK");
        }
    }

    // Метод закриття вікна запиту на введення дати
    private void RequestToInputDate_close(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}