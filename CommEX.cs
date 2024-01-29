using System.Security.Cryptography;
using System.Text;

namespace CommEX
{
    public abstract class ClientCommEX
    {
        public const string BaseURL = "https://api.commex.com";
        protected string market;

        protected string[] intervalStrings =
        ["1m","3m","5m","15m","30m","1h","2h","4h","6h","8h","12h","1d","3d","1w","1M"];

        protected ClientCommEX(string _market)
        {
            market = _market;
        }

        #region GeneralEndpoints
        public async Task<string> CheckServerTimeAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/time?";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception time";
                }
            }
        }

        public async Task<string> ExchangeInformationAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/exchangeInfo";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception exchangeInfo";
                }
            }
        }

        public async Task<string> CheckSymbolTypeAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/symbolType";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception symbolType";
                }
            }
        }
        #endregion

        #region MarketDataEndpoints
        public async Task<string> SymbolOrderBookTickerAsync(string? symbol = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/ticker/bookTicker";
                if (symbol != null)
                    url += "?symbol=" + symbol;
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception bookTicker?symbol";
                }
            }
        }

        /// <param name="limit">Default 500; max 1000</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>       
        public async Task<string> Kline(string symbol, Interval interval, int? limit = null, long? startTime = null, long? endTime = null)
        {
            if (limit <= 0 || limit > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Argument limit out of range");
            }

            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/klines?symbol=" + symbol + "&interval=" + intervalStrings[(int)interval];

                if (limit != null)
                    url += "&limit=" + limit;
                if (startTime != null)
                    url += "&startTime=" + startTime;
                if (endTime != null)
                    url += "&endTime=" + endTime;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception klines";
                }
            }
        }

        public async Task<string> TickerPriceChangeStatistics24hrAsync(string symbol)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/ticker/24hr?symbol=" + symbol;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    return "Exception ticker/24hr";
                }
            }
        }

        /// <param name="limit">Default 500; max 1000.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>       
        public async Task<string> OrderBookAsync(string symbol, int? limit = null)
        {
            if (limit <= 0 || limit > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Argument limit out of range");
            }

            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/depth?symbol=" + symbol;

                if (limit != null)
                    url += "&limit=" + limit;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception depth";
                }
            }
        }

        /// <param name="limit">Default 500; max 1000.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>       
        public async Task<string> RecentTradesListAsync(string symbol, int? limit = null)
        {
            if (limit <= 0 || limit > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Argument limit out of range");
            }

            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/trades?symbol=" + symbol;

                if (limit != null)
                    url += "&limit=" + limit;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception trades";
                }
            }
        }

        /// <param name="fromId">id to get aggregate trades from INCLUSIVE</param>
        /// <param name="startTime">Timestamp in ms to get aggregate trades from INCLUSIVE</param>
        /// <param name="endTime">Timestamp in ms to get aggregate trades until INCLUSIVE</param>
        /// <param name="limit">Default 500; max 1000</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<string> CompressedTradesListAsync(string symbol, int? limit = null, long? fromId = null, long? startTime = null, long? endTime = null)
        {
            if (limit <= 0 || limit > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Argument \"limit\" out of range");
            }

            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/aggTrades?symbol=" + symbol;

                if (limit != null)
                    url += "&limit=" + limit;
                if (startTime != null)
                    url += "&startTime=" + startTime;
                if (endTime != null)
                    url += "&endTime=" + endTime;
                if (fromId != null)
                    url += "&fromId=" + fromId;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception aggTrades";
                }
            }
        }

        public async Task<string> SymbolPriceTickerAsync(string? symbol = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseURL + market + "/ticker/price";
                if (symbol != null)
                    url += "?symbol=" + symbol;
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception bookTicker?symbol";
                }
            }
        }
        #endregion

    }

    public class FuturesClientCommEX : ClientCommEX
    {
        public FuturesClientCommEX() : base("/fapi/v1")
        {

        }

    }
    public class SpotClientCommEX : ClientCommEX
    {
        public SpotClientCommEX() : base("/api/v1")
        {

        }
    }

}
