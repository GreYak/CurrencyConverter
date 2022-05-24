using Lucca.CurrencyConverter.Domain;
using Lucca.CurrencyConverter.Domain.Contrats;
using Lucca.CurrencyConverter.Domain.Exceptions;
using Lucca.CurrencyConverter.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Lucca.CurrencyConverter.Tests.Core
{
    internal class CurrencyConverterTests
    {
        private readonly Mock<ILogger> _mockLoger;
        private readonly ICurencyConverterService _currencyConverterService;

        private readonly List<ExchangeRate> ExchangeRateExamples = new List<ExchangeRate>()
        {
            new ExchangeRate(new Currency("AUD"), new Currency("CHF"), 0.9661m),
            new ExchangeRate(new Currency("JPY"), new Currency("KWU"), 13.1151m),
            new ExchangeRate(new Currency("EUR"), new Currency("CHF"), 1.2053m),
            new ExchangeRate(new Currency("AUD"), new Currency("JPY"), 86.0305m),
            new ExchangeRate(new Currency("EUR"), new Currency("USD"), 1.2989m),
            new ExchangeRate(new Currency("JPY"), new Currency("INR"), 0.6571m)
        };

        // To help to load exchange rates.
        private async IAsyncEnumerable<ExchangeRate> Feed_LoadExchangeRatesAsync(IEnumerable<ExchangeRate> exchangeRates)
        {
            foreach (var exchangeRate in exchangeRates)
                yield return exchangeRate;
        }

        public CurrencyConverterTests()
        {
            _mockLoger = new Mock<ILogger>();
            _currencyConverterService = new CurrencyConverterService(_mockLoger.Object);
        }


        private static readonly (Currency from, Currency to, int amount, int expected)[] NominalCases = {
            // Simple cases
            (new Currency("AUD"), new Currency("CHF"), 10000, 9661),
            (new Currency("JPY"), new Currency("KWU"), 10000, 131151),
            (new Currency("EUR"), new Currency("CHF"), 10000, 12053),
            (new Currency("AUD"), new Currency("JPY"), 10000, 860305),
            (new Currency("EUR"), new Currency("USD"), 10000, 12989),
            (new Currency("JPY"), new Currency("INR"), 10000, 6571),
            // Arrondis
            (new Currency("AUD"), new Currency("CHF"), 100, 97),
            (new Currency("JPY"), new Currency("KWU"), 100, 1312),
            (new Currency("EUR"), new Currency("CHF"), 100, 121),
            (new Currency("AUD"), new Currency("JPY"), 100, 8603),
            (new Currency("EUR"), new Currency("USD"), 100, 130),
            (new Currency("JPY"), new Currency("INR"), 100, 66),
            // Reverse
            (new Currency("CHF"),new Currency("AUD"), 10000, 10351),
            (new Currency("KWU"),new Currency("JPY"), 10000, 762),
            (new Currency("CHF"),new Currency("EUR"), 10000, 8297),
            (new Currency("JPY"),new Currency("AUD"), 10000, 116),
            (new Currency("USD"),new Currency("EUR"), 10000, 7699),
            (new Currency("INR"),new Currency("JPY"), 10000, 15218),
            // Linking
            (new Currency("AUD"),new Currency("INR"), 10000, 565306),
            (new Currency("AUD"),new Currency("INR"), 10001, 565363),
            (new Currency("INR"),new Currency("AUD"), 10000, 177),
            // Linking avec reverse
            (new Currency("EUR"),new Currency("AUD"), 10001, 12477),
            (new Currency("AUD"),new Currency("EUR"), 10001, 8017),
            // 3 Links + reverse
            (new Currency("EUR"),new Currency("JPY"), 550, 59033),
        };

        [Test]
        [TestCaseSource(nameof(NominalCases))]
        public async Task GivenNominalCases_WhenConvert_ThenExpectedRate((Currency from, Currency to, int amount, int expected)testLines)
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                await _currencyConverterService.LoadExchangeRatesAsync(
                    Feed_LoadExchangeRatesAsync(ExchangeRateExamples),
                    new CancellationToken());
            });

            var result = await _currencyConverterService.Convert(testLines.from, testLines.to, testLines.amount);
            Assert.That(result, Is.EqualTo(testLines.expected));
        }

        private static readonly (Currency from, Currency to, int amount)[] NotFoundCases = {
            // Same currency not indexed.
            (new Currency("AUD"), new Currency("AUD"), 1),
            (new Currency("JPY"), new Currency("JPY"), 1),
            (new Currency("EUR"), new Currency("EUR"), 1),
            (new Currency("KWU"), new Currency("KWU"), 1),
            (new Currency("CHF"), new Currency("CHF"), 1),
            (new Currency("USD"), new Currency("USD"), 1),                           
            (new Currency("INR"), new Currency("INR"), 1),
            // Not indexed
            (new Currency("AAA"), new Currency("AUD"), 1),
            (new Currency("JPY"), new Currency("AAA"), 1),
        };

        [Test]
        [TestCaseSource(nameof(NotFoundCases))]
        public async Task GivenNominalCases_WhenConvert_ThenExpectedRate((Currency from, Currency to, int amount) testLines)
        {
            await _currencyConverterService.LoadExchangeRatesAsync(Feed_LoadExchangeRatesAsync(ExchangeRateExamples), new CancellationToken());

            Assert.ThrowsAsync<IndexNotFoundException>(async () =>
            {
                var result = await _currencyConverterService.Convert(testLines.from, testLines.to, testLines.amount);
            });
        }
    }
}
