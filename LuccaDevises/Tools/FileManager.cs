using LuccaDevises.Abstraction;

namespace LuccaDevises.Tools
{
    /// <summary>
    /// Represents a physical files in the system.
    /// </summary>
    internal class FileManager : IStreamable
    {
        private readonly string _filePath;

        /// <summary>
        /// Initialize new instansce of <see cref="FileManager"/>
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <exception cref="ArgumentException">When file doesn't exists.</exception>
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
