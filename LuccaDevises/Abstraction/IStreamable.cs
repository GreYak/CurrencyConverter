namespace LuccaDevises.Abstraction
{
    /// <summary>
    /// Represents an instance which can be a source of strem stream.
    /// </summary>
    internal interface IStreamable
    {
        /// <summary>
        /// To read the stream.
        /// </summary>
        /// <returns><see cref="StreamReader"/></returns>
        StreamReader StreamReading();
    }
}
