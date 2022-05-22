using System.Collections;

namespace Lucca.CurrencyConverter.Domain.Model
{
    internal class IdexedExchangeRateCollection : IEnumerable
    {
        public readonly Dictionary<Currency, IndexedExchangeRate> _exchangeRatesByToCurrency;

        public IdexedExchangeRateCollection()
        {
            _exchangeRatesByToCurrency = new Dictionary<Currency, IndexedExchangeRate>();
        }

        public void Add(IndexedExchangeRate indexedExchangeRate)
        {
            Currency key = indexedExchangeRate.ExchangeRate.ToCurrency;
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

        public IEnumerator GetEnumerator()
        {
            return _exchangeRatesByToCurrency.Values.GetEnumerator();
        }

        public bool TryToGetValue(Currency key, out IndexedExchangeRate? indexedValue)
        {
            return _exchangeRatesByToCurrency.TryGetValue(key, out indexedValue);
        }
    }
}
