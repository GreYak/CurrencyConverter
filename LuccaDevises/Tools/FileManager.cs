using LuccaDevises.Abstraction;

namespace LuccaDevises.Tools
{
    internal class FileManager : IStreamable
    {
        private readonly string _filePath;

        /// <summary>
        /// Initialize a new instance of <see cref="StreamParser"/>
        /// </summary>
        /// <param name="filePath">The path of the file to parse.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> doesn't exist.</exception>
        public FileManager(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));
            if (!File.Exists(filePath))
                throw new ArgumentException($"File '{filePath}' doesn't exist.");
            _filePath = filePath;
        }

        public StreamReader StreamReading() => new StreamReader(File.OpenRead(_filePath));
    }
}
