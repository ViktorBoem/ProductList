using Mopups.Services;
using System.Globalization;

namespace ProductsList.ProductList_Platforms.Windows;

// ���� ��� ���� ������ �� �������� ����� � ����
public partial class RequestToInputMonths
{
    ProductList_Window productListWindow;// ������ ��������� �� ������� ���� ������ ��������

    // ����������� ����� RequestToInputMonths, ��������� ����������
    public RequestToInputMonths(ProductList_Window mainWindow)
	{
		InitializeComponent();
        productListWindow = mainWindow;
    }

    // �������� ��䳿 ��������� ���� �� ������
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

    // �������� ��䳿 ��������� ���� �� ������
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

    // ����������� ����� ��� ������ ���������� ������� ������������ �������� �� ����� � ��
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
            await DisplayAlert("�������", "���� �����, ������ ���� � ������ ��.����", "OK");
        }
    }

    // ����� �������� ���� ������ �� �������� ����� � ����
    private void RequestToInputDate_close(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}