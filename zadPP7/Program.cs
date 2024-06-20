using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class CurrencyConverter
{
    static async Task Main()
    {
        double amount;
        Console.WriteLine("Введите сумму в USD для конвертации:");
        while (!double.TryParse(Console.ReadLine(), out amount) || amount <= 0)
        {
            Console.WriteLine("Пожалуйста, введите положительное число для суммы:");
        }

        double exchangeRate = await GetExchangeRate("USD", "EUR");
        double convertedAmount = amount * exchangeRate;

        Console.WriteLine($"Курс USD к EUR: {exchangeRate}");
        Console.WriteLine($"Сумма {amount} USD равна {convertedAmount} EUR");
    }

    static async Task<double> GetExchangeRate(string baseCurrency, string targetCurrency)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"https://api.exchangerate-api.com/v4/latest/{baseCurrency}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var exchangeRates = JsonSerializer.Deserialize<ExchangeRates>(content);
                return exchangeRates.Rates[targetCurrency];
            }
            else
            {
                throw new HttpRequestException($"Ошибка при получении курса обмена: {response.ReasonPhrase}");
            }
        }
    }

    public class ExchangeRates
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }
}
