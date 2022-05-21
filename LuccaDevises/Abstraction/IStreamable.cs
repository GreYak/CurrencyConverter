namespace LuccaDevises.Abstraction
{
    /// <summary>
    /// Represents an instance which can be read by stream.
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
