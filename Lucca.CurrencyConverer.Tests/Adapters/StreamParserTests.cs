using LuccaDevises.Abstraction;
using LuccaDevises.Tools;
using System.Text;

namespace Lucca.CurrencyConverer.Tests.Adapters
{
    internal class StreamParserTests
    {
        private readonly Mock<IStreamable> _mockSourceOfStream;
        private StreamParser _streamParserToTest;
        private Func<string> StreamContent = () => string.Empty;

        private static readonly string Line1RightFormat = "EUR;550;JPY";
        private static readonly string[] Line1BadFormated = {
            "EUR",
            "EUR;550",
            "EUR;550;",
            "EURO;550;JPY",
            "EUR;550;JPYS",
            "EUR;550e;JPY",
            "EUR;JPY,550",
            "550;EUR,JPY",
            "EUR;550,0;JPY",
            "EUR;550.0;JPY",
            string.Empty
        };
        private static readonly string Line2RightFormat = "1";
        private static readonly string[] Line2BadFormated = {
            "EUR",
            "6a",
            "6.5",
            "6,6",
            "-6",
            string.Empty
        };
        private static readonly string Line3RightFormat = "AUD;CHF;0.9661";
        private static readonly string[] Line3BadFormated = {
            "AU;CHF;0.9661",
            "AUDI;CHF;0.9661",
            "AUD;CH;0.9661",
            "AUD;CHFI;0.9661",
            "AUD;CHF;0.966",
            "AUD;CHF;0.96615",
            "AUD;CHF;0,9661",
            "AUD;0,9661;CHF",
            "0,9661;AUD;CHF",
            string.Empty
        };

        public StreamParserTests()
        {
            _mockSourceOfStream = new Mock<IStreamable>();
            
        }

        [OneTimeSetUp]
        public void OnTimeSetup()
        {
            _mockSourceOfStream
                .Setup(s => s.StreamReading())
                .Returns(() =>
                {
                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(StreamContent()));
                    var reader = new StreamReader(stream);
                    return reader;
                });
        }

        [SetUp]
        public void Setup()
        {
            StreamContent = () => string.Empty;
            _streamParserToTest = new StreamParser(_mockSourceOfStream.Object);
        }

        [Test]
        public void GivenFileEmpty_WhenReadContent_ThenInvalidDataException()
        {
            Assert.ThrowsAsync<InvalidDataException>(async () => {
                await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
                {
                    Assert.IsTrue(false);
                }
            });
        }

        [Test]
        [TestCaseSource(nameof(Line1BadFormated))]
        public void GivenWrongFormatForLine1_WhenReadContent_ThenInvalidDataException(string line1)
        {
            StreamContent = () => line1;

            Assert.ThrowsAsync<InvalidDataException>(async () => {
                await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
                {
                    Assert.IsTrue(false);
                }
            });
        }

        [Test]
        public void GivenOnlyRightLine1_WhenReadContent_ThenInvalidDataExceptionOnLine2()
        {
            StreamContent = () => Line1RightFormat;

            int linceCount = 0;

            Assert.ThrowsAsync<InvalidDataException>(async () => {
                await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
                {
                    linceCount++;
                    Assert.That(_streamParserToTest.FromCurrency, Is.EqualTo("EUR"));
                    Assert.That(_streamParserToTest.ToCurrency, Is.EqualTo("JPY"));
                    Assert.That(_streamParserToTest.Amount, Is.EqualTo(550));
                    Assert.That(_streamParserToTest.ExchangeRatesCount, Is.EqualTo(0));
                    Assert.That(linceCount, Is.EqualTo(1));
                }
            });
            Assert.That(linceCount, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(nameof(Line2BadFormated))]
        public void GivenLine2BadFormat_WhenReadContent_ThenInvalidDataExceptionOnLine2(string line2)
        {
            StreamContent = () => $"{Line1RightFormat}\n{line2}";

            int linceCount = 0;

            Assert.ThrowsAsync<InvalidDataException>(async () => {
                await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
                {
                    linceCount++;
                    Assert.That(_streamParserToTest.FromCurrency, Is.EqualTo("EUR"));
                    Assert.That(_streamParserToTest.ToCurrency, Is.EqualTo("JPY"));
                    Assert.That(_streamParserToTest.Amount, Is.EqualTo(550));
                    Assert.That(_streamParserToTest.ExchangeRatesCount, Is.EqualTo(0));
                }
            });
            Assert.That(linceCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenOnlyRightLine2_WhenReadContent_ThenInvalidDataExceptionOnLine3()
        {
            StreamContent = () => $"{Line1RightFormat}\n{Line2RightFormat}";

            int linceCount = 0;

            Assert.ThrowsAsync<InvalidDataException>(async () => {
                await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
                {
                    linceCount++;
                    Assert.That(_streamParserToTest.FromCurrency, Is.EqualTo("EUR"));
                    Assert.That(_streamParserToTest.ToCurrency, Is.EqualTo("JPY"));
                    Assert.That(_streamParserToTest.Amount, Is.EqualTo(550));
                }
            });
            Assert.That(_streamParserToTest.ExchangeRatesCount, Is.EqualTo(1));
            Assert.That(linceCount, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(nameof(Line3BadFormated))]
        public void GivenLine3BadFormat_WhenReadContent_ThenInvalidDataExceptionOnLine3(string line3)
        {
            StreamContent = () => $"{Line1RightFormat}\n{Line2RightFormat}\n{line3}";

            int linceCount = 0;

            Assert.ThrowsAsync<InvalidDataException>(async () => {
                await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
                {
                    linceCount++;
                    Assert.That(_streamParserToTest.FromCurrency, Is.EqualTo("EUR"));
                    Assert.That(_streamParserToTest.ToCurrency, Is.EqualTo("JPY"));
                    Assert.That(_streamParserToTest.Amount, Is.EqualTo(550));
                }
            });
            Assert.That(_streamParserToTest.ExchangeRatesCount, Is.EqualTo(1));
            Assert.That(linceCount, Is.EqualTo(2));
        }

        [Test]
        public void GivenMoreExchangeThanLine2_WhenReadContent_ThenInvalidDataExceptionOnLine3()
        {
            StreamContent = () => $"{Line1RightFormat}\n{Line2RightFormat}\n{Line3RightFormat}\n{Line3RightFormat}";

            int linceCount = 0;

            Assert.ThrowsAsync<InvalidDataException>(async () => {
                await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
                {
                    linceCount++;
                    Assert.That(_streamParserToTest.FromCurrency, Is.EqualTo("EUR"));
                    Assert.That(_streamParserToTest.ToCurrency, Is.EqualTo("JPY"));
                    Assert.That(_streamParserToTest.Amount, Is.EqualTo(550));
                }
            });
            Assert.That(_streamParserToTest.ExchangeRatesCount, Is.EqualTo(1));
            Assert.That(linceCount, Is.EqualTo(3));
        }

        [Test]
        public void GivenLessExchangeThanLine2_WhenReadContent_ThenInvalidDataExceptionOnLine3()
        {
            StreamContent = () => $"{Line1RightFormat}\n2\n{Line3RightFormat}";

            int linceCount = 0;

            Assert.ThrowsAsync<InvalidDataException>(async () => {
                await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
                {
                    linceCount++;
                    Assert.That(_streamParserToTest.FromCurrency, Is.EqualTo("EUR"));
                    Assert.That(_streamParserToTest.ToCurrency, Is.EqualTo("JPY"));
                    Assert.That(_streamParserToTest.Amount, Is.EqualTo(550));
                }
            });
            Assert.That(_streamParserToTest.ExchangeRatesCount, Is.EqualTo(2));
            Assert.That(linceCount, Is.EqualTo(3));
        }

        [Test]
        public async Task GivenCorrectFile_WhenReadContent_ThenOk()
        {
            StreamContent = () => $"{Line1RightFormat}\n{Line2RightFormat}\n{Line3RightFormat}";

            int linceCount = 0;

            await foreach (string? line in _streamParserToTest.ReadContentAsync(new CancellationToken()))
            {
                linceCount++;
            };

            Assert.That(_streamParserToTest.FromCurrency, Is.EqualTo("EUR"));
            Assert.That(_streamParserToTest.ToCurrency, Is.EqualTo("JPY"));
            Assert.That(_streamParserToTest.Amount, Is.EqualTo(550));
            Assert.That(_streamParserToTest.ExchangeRatesCount, Is.EqualTo(1));
            Assert.That(linceCount, Is.EqualTo(3));
        }
    }
}
