namespace Lucca.CurrencyConverter.Domain.Model
{
    /// <summary>
    /// Represents an indexed exchange rate.
    /// </summary>
    /// <param name="ExchangeRate">The exchange to be indexed.</param>
    internal record IndexedExchangeRate(ExchangeRate ExchangeRate)
    {
        private IndexedExchangeRate? _linked;

        /// <summary>
        /// Initialize a new instance of <see cref="IndexedExchangeRate"/>
        /// </summary>
        /// <param name="from">The source Currency</param>
        /// <param name="to">The destination Currency</param>
        /// <param name="changeRate">The change rate</param>
        /// <param name="linked"></param>
        public IndexedExchangeRate(Currency from, Currency to, decimal changeRate, IndexedExchangeRate linked) : this(new ExchangeRate(from, to, changeRate))
        {
            _linked = linked;
        }

        /// <summary>
        /// The weight of the index.
        /// </summary>
        /// <remarks>It corresponds to the number of intermediate conversions.</remarks>
        public int Weight => (_linked?.Weight ?? 0) + 1;
    }
}
