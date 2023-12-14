using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using ProductsList.ProductList_Platforms.Windows;

namespace ProductsList.Models
{
    public class ProductList : Product, IEnumerable<Product>
    {
        List<Product> products;//список продуктів

        // Конструктор класу ProductList, ініціалізує список продуктів
        public ProductList()
        { 
            products = new List<Product>();
        }

        // Метод додавання продукту до списку за заданими параметрами
        public void AddProduct(string productType, double price, string currency)
        {
            products.Add(new Product(productType, price, currency));
            WriteProductsToFile(products);
        }

        // Метод додавання продукту до списку з детальними датою і часом
        public void AddProduct(int year, int month, int day, int hour, int minut, string productType, double price, string currency)
        {
            products.Add(new Product(year, month, day, hour, minut, productType, price, currency));
            WriteProductsToFile(products);
        }

        // Реалізація інтерфейсу IEnumerable для перебору продуктів
        public IEnumerator<Product> GetEnumerator()
        {
            return products.GetEnumerator();
        }

        // Додаткова реалізація інтерфейсу IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Отримання списку всіх продуктів
        public List<Product> GetProducts()
        {
            return products;
        }

        // Видалення продукту зі списку за індексом
        public void RemoveProduct(int index)
        {
            if (index >= 0 && index < products.Count)
            {
                products.RemoveAt(index);
                WriteProductsToFile(products);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
        }

        // Асинхронне читання продуктів з файлу
        public async Task ReadProductsFromFile(ProductList_Window mainWindow)
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "text/plain" } },
                    { DevicePlatform.WinUI, new[] { ".txt" } },
                });

                var options = new PickOptions
                {
                    PickerTitle = "будь ласка, виберіть файл",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    var fileContents = await File.ReadAllTextAsync(result.FullPath);

                    string[] lines = fileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        try
                        {
                            string[] parts = line.Split(' ');
                            if (parts.Length == 8)
                            {
                                int year = int.Parse(parts[0]);
                                int month = int.Parse(parts[1]);
                                int day = int.Parse(parts[2]);
                                int hour = int.Parse(parts[3]);
                                int minut = int.Parse(parts[4]);
                                string productType = parts[5];
                                double price = double.Parse(parts[6], CultureInfo.InvariantCulture);
                                string currency = parts[7];

                                mainWindow.AddProductToListFromFile(year, month, day, hour, minut, productType, price, currency);
                            }
                        }
                        catch (FormatException)
                        {
                            await mainWindow.DisplayAlert("Помилка", "Помилка формату при обробці рядка файлу.", "OK");
                        }
                        catch (Exception ex)
                        {
                            await mainWindow.DisplayAlert("Помилка", $"Помилка при обробці рядка: {ex.Message}", "OK");
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                await mainWindow.DisplayAlert("Помилка", "Файл не знайдено.", "OK");
            }
            catch (IOException ex)
            {
                await mainWindow.DisplayAlert("Помилка", $"Помилка вводу-виводу: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await mainWindow.DisplayAlert("Помилка", $"Невідома помилка: {ex.Message}", "OK");
            }

            WriteProductsToFile(products);
        }


        // Аналіз статистики покупок за конкретною датою
        public string AnalysisOfPurchaseStatistics(DateTime date)
        {
            try
            {
                double significantPurchaseThreshold = 10000;

                bool productsExist = products.Any(p => p.Date.Date == date.Date);
                if (!productsExist)
                {
                    return "Продукти для вказаної дати не знайдені";
                }

                var intervals = new List<(TimeSpan start, TimeSpan end)>
                {
                    (TimeSpan.FromHours(0), TimeSpan.FromHours(8)),
                    (TimeSpan.FromHours(8), TimeSpan.FromHours(16)),
                    (TimeSpan.FromHours(16), TimeSpan.FromHours(24))
                };

                var intervalResults = intervals.Select(interval => new
                {
                    Interval = interval,
                    SignificantPurchases = products.Where(p => p.Date.Date == date.Date &&
                                                               p.Time >= interval.start &&
                                                               p.Time < interval.end &&
                                                               p.PriceInUAH >= significantPurchaseThreshold).Count(),

                    MinorPurchases = products.Where(p => p.Date.Date == date.Date &&
                                                         p.Time >= interval.start &&
                                                         p.Time < interval.end &&
                                                         p.PriceInUAH < significantPurchaseThreshold).Count(),

                    TotalPurchases = products.Where(p => p.Date.Date == date.Date &&
                                                         p.Time >= interval.start &&
                                                         p.Time < interval.end).Count()
                }).ToList();

                if (!intervalResults.Any())
                {
                    return "Продукти з вказаною датою не знайдені.";
                }

                string result = "";

                foreach (var intervalResult in intervalResults)
                {
                    string formattedStart = intervalResult.Interval.start.ToString(@"hh\:mm\:ss");
                    string formattedEnd = intervalResult.Interval.end == TimeSpan.FromHours(24) ? "00:00:00" : intervalResult.Interval.end.ToString(@"hh\:mm\:ss");

                    result += $"Від {formattedStart} до {formattedEnd}: " +
                              $"\nЗначні покупки: {intervalResult.SignificantPurchases} " +
                              $"\nДрібні покупки: {intervalResult.MinorPurchases} " +
                              $"\nВсього покупок: {intervalResult.TotalPurchases}\n";
                }

                return result;
            }
            catch (Exception ex)
            {
                return $"Виникла помилка під час аналізу статистики: {ex.Message}";
            }
        }


        // Отримання 5 продуктів із найвищою ціною
        public List<Product> Get5DatesWithTheHighestPrice()
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products), "Список продуктів пустий");
            }

            if (products.Count < 5)
            {
                throw new ArgumentNullException(nameof(products), "Недостатньо елементів");
            }

            List<Product> productsCopy = new List<Product>(products);
            List<Product> productsWithTheHighestPrice = new List<Product>();

            for (int i = 0; (i < 5) && (productsCopy.Count > 0); i++)
            {
                Product highestPriceProduct = productsCopy[0];

                foreach (var product in productsCopy)
                {
                    if (product.PriceInUAH > highestPriceProduct.PriceInUAH)
                    {
                        highestPriceProduct = product;
                    }
                }

                productsWithTheHighestPrice.Add(highestPriceProduct);
                productsCopy.Remove(highestPriceProduct);
            }

            return productsWithTheHighestPrice;
        }

        // Статистика продовольчих продуктів з ціною нижче 50 грн за певний місяць і рік
        public string GetStatisticsOfFoodProductsUnder50UAH(int year, int month)
        {
            double priceThreshold = 50;
            string productTypeCriteria = "харчування";

            if (month < 1 || month > 12)
            {
                return "Помилка: Невірний місяць.";
            }

            var foodPurchasesInSpecificMonth = products
                .Where(p => p.ProductType == productTypeCriteria &&
                            p.PriceInUAH < priceThreshold &&
                            p.Date.Year == year &&
                            p.Date.Month == month)
                .ToList();

            int totalFoodPurchasesInMonth = foodPurchasesInSpecificMonth.Count;

            if (!products.Any(p => p.PriceInUAH < priceThreshold))
            {
                return "Помилка: Немає продуктів з ціною менше 50 грн.";
            }

            int totalProducts = products.Count;

            double ratio = totalProducts > 0
                ? (double)totalFoodPurchasesInMonth / totalProducts
                : 0;

            return $"Кількість покупок типу \"{productTypeCriteria}\" з ціною менше {priceThreshold} грн у {month}.{year}: {totalFoodPurchasesInMonth}\n" +
                   $"Відношення до загальної кількості продуктів: {ratio:P2}";
        }

        // Отримання типів шести найменших покупок
        public string GetTypesOfSixSmallestPurchases()
        {
            if (products.Count() < 6)
            {
                return "Помилка: Недостатньо продуктів у списку";
            }

            var smallestPurchases = products.OrderBy(p => p.Price).Take(6);

            return string.Join(", ", smallestPurchases.Select(p => p.ProductType));
        }

        // Отримання списку покупок у дні зміни валютного курсу
        public List<Product> GetPurchasesOnCurrencyChangeDays()
        {
            var purchasesOnCurrencyChangeDays = products
                .Where(product => product.DayOfRateCurrencyChange == true)
                .ToList();

            if (!purchasesOnCurrencyChangeDays.Any())
            {
                throw new InvalidOperationException("Помилка: Немає покупок в дні зміни валютного курсу.");
            }

            return purchasesOnCurrencyChangeDays;
        }

        // Сортування списку продуктів з використанням функції для визначення ключа
        public List<Product> SortProductList(Func<Product, IComparable> keySelector)
        {
            List<Product> sortedProductList = new List<Product>(products);

            QuickSort(sortedProductList, 0, sortedProductList.Count - 1, keySelector);

            return sortedProductList;
        }

        // Внутрішній метод швидкого сортування (QuickSort) для списку продуктів
        private void QuickSort(List<Product> list, int left, int right, Func<Product, IComparable> keySelector)
        {
            if (left < right)
            {
                int pivotIndex = Partition(list, left, right, keySelector);
                QuickSort(list, left, pivotIndex - 1, keySelector);
                QuickSort(list, pivotIndex + 1, right, keySelector);
            }
        }

        // Внутрішній метод для розділення списку продуктів під час швидкого сортування
        private int Partition(List<Product> list, int left, int right, Func<Product, IComparable> keySelector)
        {
            var pivot = list[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (keySelector(list[j]).CompareTo(keySelector(pivot)) < 0)
                {
                    i++;
                    Swap(list, i, j);
                }
            }

            Swap(list, i + 1, right);
            return i + 1;
        }

        // Внутрішній метод для обміну елементів у списку
        private void Swap(List<Product> list, int index1, int index2)
        {
            Product temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        // Метод запису списку продуктів у файл
        public void WriteProductsToFile(List<Product> products)
        {
            int count = 0;

            using (StreamWriter writer = new StreamWriter("D:\\sharaga\\2CirclesOfHell\\Fuck\\Kursach\\ProductsList\\Products.txt"))
            {
                writer.WriteLine("№,   Дата,\tЧас,   Тип,   Ціна,   Валюта,   Курс");
                foreach (Product product in products)
                {
                    string line = $"{++count},   {product.Date:yyyy-MM-dd},      {product.Time:hh\\:mm},   {product.ProductType},   {product.Price},   {product.Currency},   {product.RateCurrency}";  
                    writer.WriteLine(line);
                }
            }
        }
    }
}
