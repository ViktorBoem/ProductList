using Mopups.Services;

namespace ProductsList.ProductList_Platforms.Windows;

// Клас для вікна повідомлення
public partial class MessagePopup
{
    // Конструктор класу MessagePopup, ініціалізує компоненти
    // у нього передається повідомлення, яке треба вивести
    public MessagePopup(string message)
	{
		InitializeComponent();
        
        MyMessage.Text = message;
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

    // Метод для закриття вікна повідомлення
    private void Message_close(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}