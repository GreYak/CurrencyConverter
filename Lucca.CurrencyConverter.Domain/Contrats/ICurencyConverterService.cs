using Lucca.CurrencyConverter.Domain.Model;

namespace Lucca.CurrencyConverter.Domain.Contrats
{
    /// <summary>
    /// The service to ensure converting money.
    /// </summary>
    public interface ICurencyConverterService
    {
        /// <summary>
        /// Allow to import the exchange rates to consider.
        /// </summary>
        /// <param name="exhangeRates">List of <see cref="ExchangeRate"/></param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        Task LoadExchangeRates(IAsyncEnumerable<ExchangeRate> exhangeRates, CancellationToken cancellationToken);
        
        /// <summary>
        /// Convert an amount in a given currency, to another currency.
        /// </summary>
        /// <param name="fromCurrency"><see cref="Currency"/> </param>
        /// <param name="toCurrency">Destination currency</param>
        /// <param name="amount">Amount in the <paramref name="fromCurrency"/></param>
        /// <returns>The Amount in the <paramref name="toCurrency"/></returns>
        Task<int> Convert(Currency fromCurrency, Currency toCurrency, int amount);
    }
}
