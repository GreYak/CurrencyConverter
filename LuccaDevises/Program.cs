using Lucca.CurrencyConverter.Domain;
using Lucca.CurrencyConverter.Domain.Contrats;
using Lucca.CurrencyConverter.Domain.Model;
using LuccaDevises.Tools;
using System.Globalization;
using System.Runtime.CompilerServices;

static class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Un chemin de fichier doit être spécifier.");
        }
        else
        {
            ICurencyConverterService _currencyConverterService = new CurrencyConverterService();

            // Define the cancellation token.
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            try
            {
                var fileParser = new StreamParser(new FileManager(args[0]));
                await _currencyConverterService.LoadExchangeRatesAsync(fileParser
                                                                    .ReadContentAsync(token)
                                                                    .ToExchangeRateAsync(token)
                                                                  ,token);

               Console.WriteLine();
               var result = await _currencyConverterService.Convert( new Currency(fileParser.FromCurrency),
                                                                new Currency(fileParser.ToCurrency),
                                                                fileParser.Amount);

                Console.WriteLine($"Resultat de la conversion => {fileParser.Amount}{fileParser.FromCurrency} = {result}{fileParser.ToCurrency}");
            }
            catch (InvalidDataException exc)
            {
                source.Cancel();
                Console.Error.WriteLine($"Le fichier fourni n'a pas un format valide => {exc.Message}");
            }
            catch (Exception exc)
            {
                source.Cancel();
                Console.Error.WriteLine($"Une erreur inattendue s'est produite => {exc.Message}");
            }
            finally
            {
                source.Dispose();
            }
        }
    }

    private static async IAsyncEnumerable<ExchangeRate> ToExchangeRateAsync(this IAsyncEnumerable<string> fileContentEnumerator, [EnumeratorCancellation] CancellationToken token)
    {
        int countTuple = 0;
        await foreach (var currentLine in fileContentEnumerator)
        {
            Console.WriteLine(currentLine);
            if (++countTuple >= StreamParser.FirstExchangeRateLineNumber)
            {
                string[] splittedLine = currentLine.Split(";");
                yield return new ExchangeRate(new Currency(splittedLine[0]),
                                              new Currency(splittedLine[1]), 
                                              decimal.Parse(splittedLine[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)
                                            );
            }
        }
    }
}