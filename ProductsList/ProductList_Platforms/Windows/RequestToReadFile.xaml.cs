using Mopups.Services;

namespace ProductsList.ProductList_Platforms.Windows;

// ���� ��� ���� ������
public partial class RequestPopup
{
    ProductList_Window productListWindow;// ������ ��������� �� ������� ���� ������ ��������

    // ����������� ����� RequestPopup, �������� ����������
    public RequestPopup(ProductList_Window mainWindow)
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

    // ����������� ����� ��� ������� �����
    private async void ReadFile(object sender, EventArgs e)
    {
        await productListWindow.productList.ReadProductsFromFile(productListWindow);
        await MopupService.Instance.PopAsync();
    }

    // ����� �������� ���� ������
    private void RequestPopup_close(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}