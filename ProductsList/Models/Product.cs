using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductsList.Models
{
    public class Product
    {
        // Статичні поля для всіх параметрів продукту
        protected static int externalProductId;
        protected int internalProductId;
        protected DateTime date;
        protected TimeSpan time;
        protected string? productType;
        protected double price;
        protected string? currency;
        protected double rateCurrency;

        // статитчі допоміжні поля  
        protected bool dayOfRateCurrencyChange = false;
        private static int productId = 0;
        private static DateTime lastRateUpdate = DateTime.MinValue;
        private static double savedRateCurrency = 36.76;

        // Конструктор за замовчуванням
        public Product()
        {
            date = DateTime.Today;
            time = DateTime.Now.TimeOfDay;
            this.productType = null;
            this.price = 0;
            this.currency = null;
            this.rateCurrency = 0;
        }

        // Конструктор з параметрами для створення продукту
        public Product(string productType, double price, string currency)
        {
            internalProductId = productId;
            productId++;
            externalProductId = productId;
            date = DateTime.Now;
            time = DateTime.Now.TimeOfDay;
            this.productType = productType;
            this.price = price;
            this.currency = currency;
            this.rateCurrency = SetRateCurrency();
        }

        // Конструктор з параметрами для деталізації дати та часу продукту
        public Product(int year, int month, int day, int hour, int minut, string productType, double price, string currency)
        {
            internalProductId = productId;
            productId++;
            externalProductId = productId;
            date = new DateTime(year, month, day);
            time = new TimeSpan(hour, minut, 0);
            this.productType = productType;
            this.price = price;
            this.currency = currency;
            this.rateCurrency = SetRateCurrency();
        }

        // Конструктор копіювання
        public Product(Product other)
        {
            internalProductId = other.internalProductId;
            date = other.date;
            time = other.time;
            productType = other.productType;
            price = other.price;
            currency = other.currency;
            rateCurrency = other.rateCurrency;
            dayOfRateCurrencyChange = other.dayOfRateCurrencyChange;
        }

        // Метод для встановлення валютного курсу
        private double SetRateCurrency()
        {
            if ((date - lastRateUpdate).Days >= 2)
            {
                savedRateCurrency = Math.Round(GenerateNewRate(), 2);
                lastRateUpdate = date;
                dayOfRateCurrencyChange = true;
            }

            return savedRateCurrency;
        }

        // Статичний метод для генерації нового валютного курсу
        private static double GenerateNewRate()
        {
            Random random = new Random();
            return 30 + (15 * random.NextDouble()); 
        }

        // Властивість для отримання та встановлення зовнішнього ID продукту
        public int ProductId
        {
            get { return externalProductId; }
            set { externalProductId = value; }
        }

        // Властивість для отримання внутрішнього ID продукту
        public int ID
        {
            get { return internalProductId; }
        }

        // Властивість для отримання форматованої дати
        public string DateForOutput
        {
            get { return date.ToString("dd.MM.yyyy"); }
        }

        // Властивість для отримання та встановлення дати продукту
        public DateTime Date
        {
            get { return date; }
        }

        // Властивість для отримання часу продукту
        public TimeSpan Time
        {
            get { return time; }
        }

        // Властивість для отримання форматованого часу
        public string TimeForOutput
        {
            get { return time.ToString(@"hh\:mm"); }
        }

        // Властивість для отримання та встановлення типу продукту
        public string? ProductType
        {
            get { return productType; }
            set { productType = value; }
        }

        // Властивість для отримання та встановлення ціни продукту
        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        // Властивість для отримання ціни продукту в гривнях
        public double PriceInUAH
        {
            get
            {
                return Currency == "UAH" ? Price : Price * RateCurrency;
            }
        }

        // Властивість для перевірки дня зміни валютного курсу
        public bool DayOfRateCurrencyChange
        {
            get { return dayOfRateCurrencyChange;  }
        }

        // Властивість для отримання та встановлення валюти
        public string? Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        // Властивість для отримання та встановлення валютного курсу
        public double RateCurrency
        {
            get { return rateCurrency; }
            set { rateCurrency = value; }
        }
    }
}
