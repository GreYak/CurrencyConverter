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

        public Task<int> Convert(Currency fromCurrency, Currency toCurrency, int amount)
        {
            throw new NotImplementedException();
        }

        public async Task LoadExchangeRates(IAsyncEnumerable<ExchangeRate> exhangeRates, CancellationToken cancellationToken)
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
