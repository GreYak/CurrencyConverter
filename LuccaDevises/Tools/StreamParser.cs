using LuccaDevises.Abstraction;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LuccaDevises.Tools
{
    /// <summary>
    /// Parser of file, to extract its content. 
    /// </summary>
    internal class StreamParser
    {
        private readonly IStreamable _sourceOfStream;

        /// <summary>
        /// Initialize a new instance of <see cref="StreamParser"/>
        /// </summary>
        /// <param name="sourceOfStream"><see cref="IStreamable"/></param>
        public StreamParser(IStreamable sourceOfStream)
        {
            ArgumentNullException.ThrowIfNull(sourceOfStream, nameof(sourceOfStream));
            _sourceOfStream = sourceOfStream;
        }

        /// <summary>
        /// Source currency for echange.
        /// </summary>
        public string? FromCurrency { get; private set; }

        /// <summary>
        /// Amount of source currency. 
        /// </summary>
        public int Amount { get; private set; }

        /// <summary>
        /// Destination currency for echange.
        /// </summary>
        public string? ToCurrency { get; private set; }

        /// <summary>
        /// Count of given exchanges rates.
        /// </summary>
        public int ExchangeRatesCount { get; private set; }

        /// <summary>
        /// Line of the first exchange rate in the format.
        /// </summary>
        public const int FirstExchangeRateLineNumber = 3;


        /// <summary>
        /// Extract the file content, line after line.
        /// </summary>
        /// <returns>File's lines.</returns>
        public async IAsyncEnumerable<string> ReadContent([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            int currentLine = 1;

            using (var streamReader = _sourceOfStream.StreamReading())
            {
                yield return ExtractAndAnalyseFirstLine(await streamReader.ReadLineAsync(), currentLine++);

                if (!cancellationToken.IsCancellationRequested)
                {
                    yield return ExtractAndAnalyseSecondLine(await streamReader.ReadLineAsync(), currentLine++);

                    // Content
                    int exchangeRatesCount = 0;
                    while (!streamReader.EndOfStream)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            yield break;
                        }
                        yield return ExtractAndAnalyseExchangeRateLine(await streamReader.ReadLineAsync(), currentLine++, ++exchangeRatesCount);
                    }
                    if (exchangeRatesCount < ExchangeRatesCount)
                        throw new InvalidDataException($"Line {currentLine} no exchange rates fond when {ExchangeRatesCount} were expected.");
                }

                streamReader.Close();
            }
        }

        private string ExtractAndAnalyseFirstLine(string? firstLine, int lineNumber)
        {
            var line = firstLine ?? string.Empty;
            var match = Regex.Match(line, @"^(\w{3});(\d*);(\w{3})$");
            if (!match.Success)
                throw new InvalidDataException($"Ligne {lineNumber} doit respecter le format 'D1;M;D2'");

            FromCurrency = match.Groups[1].Value;
            Amount = int.Parse(match.Groups[2].Value);
            ToCurrency = match.Groups[3].Value;

            return line;
        }

        private string ExtractAndAnalyseSecondLine(string? secondLine, int lineNumber)
        {
            var line = secondLine ?? string.Empty;
            if (!int.TryParse(line, out int converted) || converted <= 0)
                throw new InvalidDataException($"Ligne {lineNumber} doit être entier positif");

            ExchangeRatesCount = converted;

            return line;
        }

        private string ExtractAndAnalyseExchangeRateLine(string? defaultLine, int lineNumber, int exchangeRatesCount)
        {
            if (ExchangeRatesCount < exchangeRatesCount)
                throw new InvalidDataException($"Le nombre de ligne de taux de change ne peut excéder celui spécifier à la ligne 2 ({ExchangeRatesCount});");

            if (defaultLine is null || !Regex.IsMatch(defaultLine, @"^(\w{3});(\w{3});(\d*\.?\d{4}?)$"))        
                throw new InvalidDataException($"Ligne {lineNumber} doit respecter le format 'DD;DA;T'");

            return defaultLine;
        }
    }
}
