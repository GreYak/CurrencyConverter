using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
            int maxExchangeRatesCount;

            using (var streamReader = new StreamReader(File.OpenRead(_filePath)))
            {
                yield return ExtractAndAnalyseFirstLine(await streamReader.ReadLineAsync());

                if (!cancellationToken.IsCancellationRequested)
                {
                    yield return ExtractAndAnalyseSecondLine(await streamReader.ReadLineAsync(), out maxExchangeRatesCount);

                    // Content
                    int currentLine = 1;
                    while (!streamReader.EndOfStream)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            yield break;
                        }
                        yield return ExtractAndAnalyseOtherLine(await streamReader.ReadLineAsync(), maxExchangeRatesCount, currentLine++);
                    }
                }

                streamReader.Close();
            }
        }

        private string? ExtractAndAnalyseFirstLine(string? firstLine)
        {
            if (firstLine is null || !Regex.IsMatch(firstLine, @"^(\w{3});(\d*);(\w{3})$"))        
                throw new InvalidDataException("Ligne 1 doit respecter le format 'D1;M;D2'");
            return firstLine;
        }

        private string? ExtractAndAnalyseSecondLine(string? secondLine, out int converted)
        {
            if (!int.TryParse(secondLine, out converted) || converted <= 0)
                throw new InvalidDataException("Ligne 2 doit être entier positif");
            return secondLine;
        }

        private string? ExtractAndAnalyseOtherLine(string? defaultLine, int maxExchangeRatesCount, int lineNumber)
        {
            if (maxExchangeRatesCount < lineNumber)
                throw new InvalidDataException($"Le nombre de ligne de taux de change ne peut excéder celui spécifier à la ligne 2 ({maxExchangeRatesCount});");

            if (defaultLine is null || !Regex.IsMatch(defaultLine, @"^(\w{3});(\w{3});(\d*\.?\d{4}?)$"))        
                throw new InvalidDataException($"Ligne {lineNumber + 2} doit respecter le format 'DD;DA;T'");
            return defaultLine;
        }
    }
}
