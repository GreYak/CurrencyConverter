using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucca.CurrencyConverter.Domain.Model
{
    /// <summary>
    /// Represents a exchange rates between 2 <see cref="Currency"/>.
    /// </summary>
    public class ExchangeRate
    {
        /// <summary>
        /// Unitialiaze a new instance of <see cref="ExchangeRate"/>.
        /// </summary>
        /// <param name="from">The source Currency</param>
        /// <param name="to">The destination Currency</param>
        /// <param name="changeRate">The change rate</param>
        public ExchangeRate(Currency from, Currency to, decimal changeRate)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);
            if (changeRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(changeRate), changeRate, "Should be > 0");
            if (from.Equals(to))
                throw new ArgumentException($"Source and destination currencies couldn't be similar : {from.ToString()}");

            Rate = Math.Round(changeRate, 4);
            FromCurrency = from;
            ToCurrency = to;
        }

        /// <summary>
        /// The source Currency
        /// </summary>
        public Currency FromCurrency;

        /// <summary>
        /// The destination Currency 
        /// </summary>
        public Currency ToCurrency;

        /// <summary>
        /// The change rate
        /// </summary>
        public decimal Rate;

        /// <summary>
        /// Get the reverse change rate.
        /// </summary>
        /// <returns><see cref="ExchangeRate"/></returns>
        public ExchangeRate GetReverseCurrencyRate() => new ExchangeRate(ToCurrency, FromCurrency, 1 / Rate);
    }
}
