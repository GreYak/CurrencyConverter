namespace Lucca.CurrencyConverter.Domain.Model
{
    /// <summary>
    /// Represents a currency.
    /// </summary>
    public class Currency : IEquatable<Currency>
    {
        /// <summary>
        /// Initialize a new instance of <see cref="Currency"/>
        /// </summary>
        /// <param name="code">The code of the currency.</param>
        public Currency(string code)
        {
            if (code?.Length != 3) throw new ArgumentException($"{nameof(Currency)} => Unsupported Code Format : '{code ?? "null"}'");
            Code = code.ToUpper();
        }

        private string Key => Code;

        /// <summary>
        /// The trigram to represents the currency
        /// </summary>
        public string Code;

        /// <summary>
        /// Indicates if the current instance is equal to an other instance
        /// </summary>
        /// <param name="other">Other instance of <see cref="Currency"/></param>
        /// <returns>True if instances are equals, else false.</returns>
        public bool Equals(Currency? other) => Key == other?.Key;

        /// <summary>
        /// Return a string to represent current instance.
        /// </summary>
        public override string ToString() => Key;
    }
}
