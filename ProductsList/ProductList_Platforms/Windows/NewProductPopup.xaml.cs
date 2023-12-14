namespace ProductsList.ProductList_Platforms.Windows;
using Mopups.Services;

// ���� ��� ���� ��������� ������ ��������
public partial class NewProductPopup
{
    ProductList_Window productListWindow;// ������ ��������� �� ������� ���� ������ ��������

    // ����������� ����� NewProductPopup, �������� ����������
    public NewProductPopup(ProductList_Window mainWindow)
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

    // ����� ������� ���� �� ������ ��������� ��������
    private void AddProductButtonCliced(object sender, EventArgs e)
    {
        if (ProductType.SelectedIndex < 0)
        {
            DisplayAlert("�������", "���� �����, ������� ��� ��������.", "OK");
            return;
        }

        if (ProductCurrency.SelectedIndex < 0)
        {
            DisplayAlert("�������", "���� �����, ������� ������.", "OK");
            return;
        }

        try
        {
            string productType = ProductType.Items[ProductType.SelectedIndex];
            string productCurrency = ProductCurrency.Items[ProductCurrency.SelectedIndex];
            int price = Convert.ToInt32(ProductPrice.Text);

            if (price < 0 || price == 0)
            {
                DisplayAlert("�������", "ֳ�� �� ���� ���� ��'�����.", "OK");
                return;
            }

            productListWindow.AddProductToList(productType, price, productCurrency);
            MopupService.Instance.PopAsync();
        }
        catch (FormatException)
        {
            DisplayAlert("�������", "���� �����, ������ ����� �������� ����.", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("�������", $"������� �������: {ex.Message}", "OK");
        }
    }

    // ����� �������� ���� ��������� ������ ��������
    private void �loseNewProductPopup(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}