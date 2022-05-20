using System.Runtime.CompilerServices;

namespace LuccaDevises.Tools
{
    /// <summary>
    /// Parser of file, to extract its content. 
    /// </summary>
    internal class FileParser
    {
        private readonly string _filePath;

        /// <summary>
        /// Initialize a new instance of <see cref="FileParser"/>
        /// </summary>
        /// <param name="filePath">The path of the file to parse.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> doesn't exist.</exception>
        public FileParser(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));
            if (!File.Exists(filePath))
                throw new ArgumentException($"File '{filePath}' doesn't exist.");
            _filePath = filePath;
        }

        /// <summary>
        /// Extract the file content, line after line.
        /// </summary>
        /// <returns>File's lines.</returns>
        public async IAsyncEnumerable<string?> ReadContent([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using (var streamReader = new StreamReader(File.OpenRead(_filePath)))
            {
                while (!streamReader.EndOfStream)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        yield break; 
                    }
                    yield return await streamReader.ReadLineAsync();
                }

                streamReader.Close();
            }
        }
    }
}
