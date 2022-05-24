using System.Runtime.Serialization;

namespace Lucca.CurrencyConverter.Domain.Exceptions
{
    /// <summary>
    /// Indicates a required entry is not stored in the index.
    /// </summary>
    [Serializable]
    public class IndexNotFoundException : ApplicationException
    {
        public IndexNotFoundException() : base() { }
        public IndexNotFoundException(string message) : base(message) { }
        public IndexNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected IndexNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
