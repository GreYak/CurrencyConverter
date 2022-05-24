using Lucca.CurrencyConverter.Domain.Contrats;
using Lucca.CurrencyConverter.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Lucca.CurrencyConverter.Domain
{
    public class CurrencyConverterService : ICurencyConverterService    
    {
        private readonly ILogger _logger;
        private readonly ExchangeRateIndex _index;


        public CurrencyConverterService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _index = new ExchangeRateIndex();
        }

        public async Task<int> Convert(Currency fromCurrency, Currency toCurrency, int amount)
        {
            IndexedExchangeRate indexedExchangeRate = _index.GetExchangeRate(fromCurrency, toCurrency);
            decimal convertedAmount = amount;
            await foreach(var rate in indexedExchangeRate.Rates())
            {
                convertedAmount = Math.Round(convertedAmount * rate, 4);
            }

            return decimal.ToInt32(Math.Round(convertedAmount, 0));
        }

        public async Task LoadExchangeRatesAsync(IAsyncEnumerable<ExchangeRate> exhangeRates, CancellationToken cancellationToken)
        {
            _index.Clear();

            await foreach (var rate in exhangeRates) 
            {
                _index.AddExchangeRate(rate);
                _index.AddExchangeRate(rate.GetReverseCurrencyRate());
            }
        }
    }
}
