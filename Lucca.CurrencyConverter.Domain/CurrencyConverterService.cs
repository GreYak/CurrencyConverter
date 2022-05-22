using Lucca.CurrencyConverter.Domain.Contrats;
using Lucca.CurrencyConverter.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Lucca.CurrencyConverter.Domain
{
    public class CurrencyConverterService : ICurencyConverterService    // TODO => internal
    {
        private readonly ILogger _logger;
        private readonly ExchangeRateIndex _index;


        public CurrencyConverterService(ILogger<CurrencyConverterService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _index = new ExchangeRateIndex();
        }

        public int Convert(Currency fromCurrency, Currency toCurrency, int amount)
        {
            ExchangeRate exchangeRate = _index.GetExchangeRate(fromCurrency, toCurrency);
            return decimal.ToInt32(Math.Round(amount * exchangeRate.Rate));
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
