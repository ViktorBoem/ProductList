using System.Linq;

namespace ProductsList.ProductList_Platforms.Windows;

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Text;
using Mopups.Services;
using ProductsList.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

public partial class ProductList_Window : ContentPage
{
    //оголошені глобальні змінні
    private HorizontalStackLayout? saveMenuSliderButtonClicked = null;//зберігає кнопка якого слайдера зараз нажата
    private HorizontalStackLayout? saveMenuItemButtonClicked = null;//зберігає яка кнопка з елементів слайдера нажата
    private StackLayout? saveMenuOpen = null;//зберігає який зараз слайдер відкритий
    public int selectedItemTable = int.MinValue;//зберігає який зараз елемент таблиці обраний
    public int selectedItemProductListTable = int.MinValue; //зберігає обраний продукт у таблиці

    public ProductList productList = new ProductList();//списокпродуктів

    // Ініціалізація вікна прогами і очікування поки вікно не інціалізується
    public ProductList_Window()
    {
        InitializeComponent();

        this.Loaded += async (sender, e) =>
        {
            await Task.Delay(350);
            await MopupService.Instance.PushAsync(new RequestPopup(this));
        };
    }

    // Метод для застосування анімації прозорості до елементів Border
    private void AccessToBorderAndOpacityAnimation(Layout layout, double opacity, uint duration)
    {
        foreach (var child in layout.Children)
        {
            if (child is Border border)
            {
                border.FadeTo(opacity, duration);
                break;
            }
        }
    }

    // Асинхронний метод для показу або приховування меню
    private async Task ShowOrHideMenu(StackLayout menuToShow, EventArgs e)
    {
        if (saveMenuOpen != null && saveMenuOpen != menuToShow)
        {
            await HideMenu(saveMenuOpen);
        }

        if (!menuToShow.IsVisible)
        {
            await ShowMenu(menuToShow);
        }
        else
        {
            await HideMenu(menuToShow);

            if(saveMenuSliderButtonClicked != null)
            {
                AccessToBorderAndOpacityAnimation(saveMenuSliderButtonClicked, 0, 250);
                saveMenuSliderButtonClicked = null;
            }
        }
    }

    // Асинхронний метод для показу меню
    private async Task ShowMenu(StackLayout menu)
    {
        menu.IsVisible = true;

        foreach (var child in menu.Children.OfType<VisualElement>())
        {
            child.IsVisible = true;
            await child.FadeTo(1, 150);
        }

        saveMenuOpen = menu;
    }

    // Асинхронний метод для приховування меню
    private async Task HideMenu(StackLayout menu)
    {
        saveMenuOpen = null;

        foreach (var child in menu.Children.OfType<VisualElement>().Reverse())
        {
            await child.FadeTo(0, 150);
            child.IsVisible = false;
        }

        menu.IsVisible = false;
    }

    // Обробник події наведення миші на елемент Border
    private void OnBorderMouseEntered(object sender, EventArgs e)
    {
        if (sender is HorizontalStackLayout layout)
        {
            AccessToBorderAndOpacityAnimation(layout, 1, 250);

            if (layout == AddProduct && layout.Parent is Border border)
            {
                border.ScaleTo(1.1, 100);
            }
        }
    }

    // Обробник події відведення миші від елемента Border
    private void OnBorderMouseExit(object sender, EventArgs e)
    {
        if (sender is HorizontalStackLayout layout)
        {
            if(layout != saveMenuSliderButtonClicked && layout != saveMenuItemButtonClicked)
            {
                AccessToBorderAndOpacityAnimation(layout, 0, 250);
            }

            if (layout == AddProduct && layout.Parent is Border border)
            {
                border.ScaleTo(1, 100);
            }
        }
    }

    // Обробник кліку на слайдер або пункт меню
    private async void SliderOrMenuItem_Click(object sender, EventArgs e)
    {
        if (!(sender is HorizontalStackLayout layout))
        {
            return;
        }

        if (!(layout.BindingContext is MenuItemViewModel) && saveMenuSliderButtonClicked != null && saveMenuSliderButtonClicked != layout)
        {
            AccessToBorderAndOpacityAnimation(saveMenuSliderButtonClicked, 0, 250);
        }
        else if(!(layout.BindingContext is MenuSliderViewModel) && saveMenuItemButtonClicked != null && saveMenuItemButtonClicked != layout)
        {
            AccessToBorderAndOpacityAnimation(saveMenuItemButtonClicked, 0, 250);
        }

        AccessToBorderAndOpacityAnimation(layout, 1, 250);

        if (saveMenuItemButtonClicked == layout)
        {
            AccessToBorderAndOpacityAnimation(layout, 0, 250);
            saveMenuItemButtonClicked = null;

            return;
        }

        if (layout.BindingContext is MenuSliderViewModel)
        {
            saveMenuSliderButtonClicked = layout;

        }
        else if (layout.BindingContext is MenuItemViewModel)
        {
            saveMenuItemButtonClicked = layout;
        }

        if (sender == SortMenuSlider)
        {
            await ShowOrHideMenu(SortMenu, e);
        }
        else if(sender == StatisticMenuSlider)
        {
            await ShowOrHideMenu(StatisticMenu, e);
        }

    }

    // Обробник кліку для додавання нового продукту
    private void AddProductBorderButton_Click(object sender, EventArgs e)
    {
        MopupService.Instance.PushAsync(new NewProductPopup(this));
    }

    // Метод для додавання продукту до списку
    public void AddProductToList(string productType, double price, string currency)
    {
        if(StartedTablePhoto.IsVisible == true)
        {
            StartedTablePhoto.IsVisible = false;
        }

        productList.AddProduct(productType, price, currency);
        var currencyProduct = productList.Last();
        CreateProductsGrid(currencyProduct);
    }

    // Метод для додавання продукту до списку з файлу
    public void AddProductToListFromFile(int year, int month, int day, int hour, int minut, string productType, double price, string currency)
    {
        if (StartedTablePhoto.IsVisible == true)
        {
            StartedTablePhoto.IsVisible = false;
        }

        productList.AddProduct(year, month, day, hour, minut, productType, price, currency);
        var currencyProduct = productList.Last();
        CreateProductsGrid(currencyProduct);
    }

    // Метод для створення сітки продуктів
    private void CreateProductsGrid(Product currencyProduct)
    {
        Grid productsGrid = new Grid();
        AddingAnimationToGrid(productsGrid, currencyProduct.ID, currencyProduct.ProductId);

        productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(125) });
        productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(175) });
        productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(125) });
        productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(175) });
        productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(125) });
        productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(125) });
        productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(175) });

        productsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
        int rowIndex = productsGrid.RowDefinitions.Count - 1;

        productsGrid.Children.Add(CreateLabelForGrid(currencyProduct.ProductId.ToString(), 0, rowIndex));
        productsGrid.Children.Add(CreateLabelForGrid(currencyProduct.DateForOutput, 1, rowIndex));
        productsGrid.Children.Add(CreateLabelForGrid(currencyProduct.TimeForOutput, 2, rowIndex));
        productsGrid.Children.Add(CreateLabelForGrid(currencyProduct.ProductType, 3, rowIndex));
        productsGrid.Children.Add(CreateLabelForGrid(currencyProduct.Price.ToString(), 4, rowIndex));
        productsGrid.Children.Add(CreateLabelForGrid(currencyProduct.Currency, 5, rowIndex));
        productsGrid.Children.Add(CreateLabelForGrid(currencyProduct.RateCurrency.ToString(), 6, rowIndex));

        TableProductList.Children.Add(productsGrid);
    }

    // Метод для створення мітки у сітці
    private Label CreateLabelForGrid(string? text, int column, int row)
    {
        Label label = new Label
        {
            Text = text,
            Style = (Style?)Application.Current?.Resources["TibleItemStyle"]
        };

        Grid.SetColumn(label, column);
        Grid.SetRow(label, row);
        return label;
    }

    // Асинхронний метод для видалення елемента зі сітки
    public async void RemoveGridFromTable(int index)
    {
        var item = TableProductList.Children[index] as VisualElement;

        if (item != null)
        {
            await Task.Delay(100);
            await item.ScaleTo(1, 150);
            await item.FadeTo(0, 250);

            TableProductList.Children.RemoveAt(index);
        }
    }

    // Метод для додавання анімації до сітки
    private void AddingAnimationToGrid(Grid grid, int internalID, int externalID)
    {
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += (s, e) => TriggerGridAnimation(grid, internalID, externalID);
        grid.GestureRecognizers.Add(tapGestureRecognizer);
    }

    // Асинхронний метод для активації анімації сітки
    private async void TriggerGridAnimation(Grid grid, int internalID, int externalID)
    {
        selectedItemProductListTable = internalID;
        selectedItemTable = externalID;

        var animateIn = new Animation(v => grid.Margin = new Thickness(0, v, 0, v), grid.Margin.Top, grid.Margin.Top + 15);
        animateIn.Commit(grid, "GridClickAnimationIn", 16, 200, Easing.Linear);
        await grid.ScaleTo(1.05, 100);

        await Task.Delay(100);
        await MopupService.Instance.PushAsync(new RequestToDeleatItem(this));
    }

    // Асинхронний метод для сортування списку за кількістю
    private async void SortListByAmount(object sender, EventArgs e)
    {
        await Task.Delay(250);
        List<Product> sortedList = new List<Product>();

        if (sender is HorizontalStackLayout layout && layout.Children.OfType<Border>().FirstOrDefault()?.Opacity == 1)
        {
            var text = (sender as HorizontalStackLayout)?.Children.OfType<Label>().FirstOrDefault()?.Text;

            sortedList = productList.SortProductList(product =>
            {
                return text switch
                {
                    "за сумою" => product.PriceInUAH,
                    "за датою" => product.Date,
                    "за типом" => product.ProductType ?? string.Empty,
                    _ => product.Currency ?? string.Empty,
                };
            });
        }
        else
        {
            sortedList = productList.GetProducts();
        }

        if (!sortedList.Any() || sortedList.Count == 1)
        {
            await DisplayAlert("Помилка", "додайте більше елементів до списку", "OK");
            return;
        }

        TableProductList.Clear();
        productList.WriteProductsToFile(sortedList);

        int count = 0;
        foreach (var item in sortedList)
        {
            item.ProductId = ++count;
            CreateProductsGrid(item);
        }
    }

    // Асинхронний метод для виведення 5 дат з найвищою ціною.
    private async void Output5DatesWithTheHighestPrice(object sender, EventArgs e)
    {
        await Task.Delay(250);
        List<Product> List5DatesWithTheHighestPrice = new List<Product>();

        try
        {
            if (sender is HorizontalStackLayout layout && layout.Children.OfType<Border>().FirstOrDefault()?.Opacity == 1)
            {
                List5DatesWithTheHighestPrice = productList.Get5DatesWithTheHighestPrice();
            }
            else
            {
                List5DatesWithTheHighestPrice = productList.GetProducts();
            }

        }
        catch (ArgumentNullException ex)
        {
            await DisplayAlert("Помилка", ex.Message, "OK");
            List5DatesWithTheHighestPrice = productList.GetProducts();
        }

        TableProductList.Clear();
        productList.WriteProductsToFile(List5DatesWithTheHighestPrice);

        int count = 0;
        foreach (var item in List5DatesWithTheHighestPrice)
        {
            item.ProductId = ++count;
            CreateProductsGrid(item);
        }
    }

    // Асинхронний метод для статистики за конкретний день
    private async void StatisticsOnparticularDay(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new RequestToInputDate(this));
    }

    // Асинхронний метод для статистики щодо продовольчих товарів до 50 гривень
    private async void StatisticsOfFoodProductsUnder50UAH(object sender, EventArgs e)
    {
        await Task.Delay(250);
        await MopupService.Instance.PushAsync(new RequestToInputMonths(this));
    }

    // Асинхронний метод для виведення типів шести найменших покупок
    private async void OutputTypesOfSixSmallestPurchases(object sender, EventArgs e)
    {
        await Task.Delay(250);
        string message = productList.GetTypesOfSixSmallestPurchases();

        await MopupService.Instance.PushAsync(new MessagePopup(message));
    }

    // Асинхронний метод для виведення покупок у день зміни курсу
    private async void OutputPurchasesOnCurrencyChangeDays(object sender, EventArgs e)
    {
        await Task.Delay(250);
        List<Product> listWithPurchasesOnCurrencyChangeDays = new List<Product>();

        try
        {
            if (sender is HorizontalStackLayout layout && layout.Children.OfType<Border>().FirstOrDefault()?.Opacity == 1)
            {
                listWithPurchasesOnCurrencyChangeDays = productList.GetPurchasesOnCurrencyChangeDays();
            }
            else
            {
                listWithPurchasesOnCurrencyChangeDays = productList.GetProducts();
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "OK");
            listWithPurchasesOnCurrencyChangeDays = productList.GetProducts();
        }

        TableProductList.Clear();
        productList.WriteProductsToFile(listWithPurchasesOnCurrencyChangeDays);

        int count = 0;
        foreach (var item in listWithPurchasesOnCurrencyChangeDays)
        {
            item.ProductId = ++count;
            CreateProductsGrid(item);
        }
    }

}
