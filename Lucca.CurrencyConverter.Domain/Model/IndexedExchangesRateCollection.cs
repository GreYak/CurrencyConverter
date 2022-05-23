using System.Collections;

namespace Lucca.CurrencyConverter.Domain.Model
{
    /// <summary>
    /// Represents a collection of indexed Exchange rates, by CurrrencyTo.
    /// </summary>
    internal class IndexedExchangesRateCollection : IEnumerable
    {
        public readonly Dictionary<Currency, IndexedExchangeRate> _exchangeRatesByToCurrency;

        /// <summary>
        /// Initialize a new instance of <see cref="IndexedExchangesRateCollection"/>
        /// </summary>
        public IndexedExchangesRateCollection()
        {
            _exchangeRatesByToCurrency = new Dictionary<Currency, IndexedExchangeRate>();
        }

        /// <summary>
        /// To Add a new indexed exchange rate into the collection.
        /// </summary>
        /// <param name="indexedExchangeRate"><paramref name="indexedExchangeRate"/></param>
        public void Add(IndexedExchangeRate indexedExchangeRate)
        {
            Currency key = indexedExchangeRate.ToCurrency;
            if (!_exchangeRatesByToCurrency.ContainsKey(key))
            {
                _exchangeRatesByToCurrency.Add(key, indexedExchangeRate);
            }
            else
            {
                if (_exchangeRatesByToCurrency[key].Weight > indexedExchangeRate.Weight)
                {
                    _exchangeRatesByToCurrency[key] = indexedExchangeRate;
                }
            }
        }

        /// <summary>
        /// <see cref="IEnumerator"/> to cross the <see cref="IndexedExchangeRate"/> of the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the <see cref="IndexedExchangesRateCollection"/></returns>
        public IEnumerator GetEnumerator()
        {
            return _exchangeRatesByToCurrency.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="IndexedExchangeRate"/> associated with the specified key.
        /// </summary>
        /// <param name="key">Destination <see cref="Currency"/> indexing the value.</param>
        /// <param name="indexedValue">The <see cref="IndexedExchangeRate"/> associated with <paramref name="key"/>, if found; else, null. This parameter is passed uninitialized.</param>
        /// <returns>True if an <see cref="IndexedExchangeRate"/> is associated to <paramref name="key"/>, else false. </returns>
        public bool TryToGetValue(Currency key, out IndexedExchangeRate? indexedValue)
        {
            return _exchangeRatesByToCurrency.TryGetValue(key, out indexedValue);
        }
    }
}
