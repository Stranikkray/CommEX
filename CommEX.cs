using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CommEX
{
    public abstract class DataCommEX
    {
        public const string baseURL = "https://api.commex.com";
        protected string market;

        protected string[] intervalStrings =
        ["1m", "3m", "5m", "15m", "30m", "1h", "2h", "4h", "6h", "8h", "12h", "1d", "3d", "1w", "1M"];

        protected DataCommEX(string _market)
        {
            market = _market;
        }
    }

    public abstract class ClientCommEX : DataCommEX
    {
        protected ClientCommEX(string _market) : base(_market)
        {

        }

        #region GeneralEndpoints
        /// <summary>
        /// Test connectivity to the Rest API and get the current server time
        /// </summary>        
        public async Task<string> CheckServerTimeAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/time?";

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

        /// <summary>
        /// Current exchange trading rules and symbol information.
        /// </summary>
        public async Task<string> ExchangeInformationAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/exchangeInfo";

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

        #endregion

        #region MarketDataEndpoints

        /// <summary>
        /// Best price/qty on the order book for a symbol or symbols.
        /// </summary>
        public async Task<string> SymbolOrderBookTickerAsync(string? symbol = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/ticker/bookTicker";
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

        /// <summary>
        /// Kline/candlestick bars for a symbol. Klines are uniquely identified by their open time.
        /// </summary>
        /// <param name="limit">Default 500; max 1000</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>       
        public async Task<string> KlineData(string symbol, Interval interval, int? limit = null, long? startTime = null, long? endTime = null)
        {
            if (limit <= 0 || limit > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Argument limit out of range");
            }

            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/klines?symbol=" + symbol + "&interval=" + intervalStrings[(int)interval];

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

        /// <summary>
        /// 24 hour rolling window price change statistics.
        /// </summary>
        public async Task<string> TickerPriceChangeStatistics24hrAsync(string symbol)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/ticker/24hr?symbol=" + symbol;

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

        /// <summary>
        /// Get Order Book.
        /// </summary>
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
                string url = baseURL + market + "/v1/depth?symbol=" + symbol;

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

        /// <summary>
        /// Get recent trades.
        /// </summary>
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
                string url = baseURL + market + "/v1/trades?symbol=" + symbol;

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

        /// <summary>
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// </summary>
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
                string url = baseURL + market + "/v1/aggTrades?symbol=" + symbol;

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

        /// <summary>
        /// Latest price for a symbol or symbols.
        /// </summary>
        public async Task<string> SymbolPriceTickerAsync(string? symbol = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/ticker/price";
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
       
    public class SpotClientCommEX : ClientCommEX
    {
        public SpotClientCommEX() : base("/api")
        {

        }

        #region GeneralEndpoints
        /// <summary>
        /// Check Symbol Type, type will be GLOBAL or SITE.
        /// </summary>
        /// <returns></returns>
        public async Task<string> CheckSymbolTypeAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/symbolType";

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
    }

    public class FuturesClientCommEX : ClientCommEX
    {
        public FuturesClientCommEX() : base("/fapi")
        {

        }

        #region Market Data Endpoints

        /// <summary>
        /// Test connectivity to the Rest API.
        /// </summary>
        public async Task<string> TestConnectivityAsync()
        {       
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/ping";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();

                }
                catch
                {
                    return "Exception ping";
                }
            }
        }

        /// <summary>
        /// Get older market historical trades.
        /// </summary>
        /// <param name="limit">Default 500; max 1000</param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades.</param>
        public async Task<string> OldTradesLookupAsync(string symbol,int? limit = null,long? fromId = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/historicalTrades?symbol=" + symbol;

                if (limit != null)
                    url += "&limit=" + limit;
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
                    return "Exception historicalTrades";
                }
            }
        }

        /// <summary>
        /// Kline/candlestick bars for the index price of a pair.
        /// Klines are uniquely identified by their open time.
        /// </summary>
        /// <param name="limit">Default 500; max 1500.</param>
        /// <returns></returns>
        public async Task<string> IndexPriceKlineDataAsync(string symbol, Interval interval, long? startTime = null, long? endTime = null, int? limit = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/historicalTrades?symbol=" + symbol+ "&interval=" + intervalStrings[(int)interval];

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
                    return "Exception historicalTrades";
                }
            }
        }

        /// <summary>
        /// Kline/candlestick bars for the mark price of a symbol.
        /// Klines are uniquely identified by their open time.
        /// </summary>
        /// <param name="limit">Default 500; max 1500.</param>
        /// <returns></returns>
        public async Task<string> MarkPriceKlineDataAsync(string symbol, Interval interval, long? startTime = null, long? endTime = null, int? limit = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/markPriceKlines?symbol=" + symbol + "&interval=" + intervalStrings[(int)interval];

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
                    return "Exception markPriceKlines";
                }
            }
        }

        /// <summary>
        /// Mark Price and Funding Rate
        /// </summary>
        public async Task<string> MarkPriceAsync(string? symbol=null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/premiumIndex?";

                if (symbol != null)
                    url += "symbol=" + symbol;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    return "Exception premiumIndex";
                }
            }
        }

        /// <param name="startTime">Timestamp in ms to get funding rate from INCLUSIVE.</param>
        /// <param name="endTime">Timestamp in ms to get funding rate until INCLUSIVE.</param>
        /// <param name="limit"></param>
        /// <returns>Default 100; max 1000</returns>
        public async Task<string> GetFundingRateHistoryAsync(string? symbol = null, long? startTime = null, long? endTime = null, int? limit = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v1/fundingRate?" ;

                if (symbol != null)
                    url += "symbol=" + symbol;
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
                    return "Exception fundingRate";
                }
            }
        }

        /// <summary>
        /// Get present open interest of a specific symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public async Task<string> OpenInterestAsync(string symbol)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = baseURL + market + "/v2/openInterest?symbol=" + symbol;                

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    return "Exception openInterest";
                }
            }
        }
        #endregion

    }

}
