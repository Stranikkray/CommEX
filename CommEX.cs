using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;
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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;

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
                    return responseString;
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
                    return responseString;
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
                    return responseString;
                }
                catch
                {
                    return "Exception openInterest";
                }
            }
        }
        #endregion

    }

    public abstract class APIClientCommEX : ClientCommEX
    {
        protected string apiKey = null;
        protected string apiSecret = null;

        protected APIClientCommEX(string market, string _apiKey, string _apiSecret) : base(market)
        {
            apiKey = _apiKey;
            apiSecret = _apiSecret;
        }

        protected string CalculateSignature(string data)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        protected string CalculateSignature(List<KeyValuePair<string, string>> parametrsPair)
        {
            string parametrs = parametrsPair[0].Key + "=" + parametrsPair[0].Value;

            for (int i = 1; i < parametrsPair.Count; i++)
            {
                parametrs += "&" + parametrsPair[i].Key + "=" + parametrsPair[i].Value;
            }
             return CalculateSignature(parametrs);        
        }
        protected string GenerateStringWithSignature(List<KeyValuePair<string, string>> parametrsPair)
        {
            string parametrs = parametrsPair[0].Key + "=" + parametrsPair[0].Value;

            for (int i = 1; i < parametrsPair.Count; i++)
            {
                parametrs += "&" + parametrsPair[i].Key + "=" + parametrsPair[i].Value;
            }
            parametrs += "&signature=" + CalculateSignature(parametrs);
            return parametrs;
        }      
    }

    public class SpotAPIClientCommEX : APIClientCommEX
    {
        public SpotAPIClientCommEX(string apiKey, string apiSecret) : base("/api", apiKey, apiSecret)
        {
        }

        #region AccountEndpoints
        /// <summary>
        /// Get current account information.
        /// </summary>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> AccountInformationAsync(long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/account?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception account";
                }
            }
        }

        /// <summary>
        /// Get trades for a specific account and symbol.
        /// </summary>
        /// <param name="orderId">This can only be used in combination with symbol</param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades.</param>
        /// <param name="limit">Default 500; max 1000.</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> AccountTradeListAsync(string symbol, long? orderId = null, long? startTime = null, long? endTime = null, long? fromId = null, int? limit = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/userTrades?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                if (orderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("orderId", orderId.ToString()));
                if (startTime != null)
                    parametrs.Add(new KeyValuePair<string, string>("startTime", startTime.ToString()));
                if (endTime != null)
                    parametrs.Add(new KeyValuePair<string, string>("endTime", endTime.ToString()));
                if (fromId != null)
                    parametrs.Add(new KeyValuePair<string, string>("fromId", fromId.ToString()));
                if (limit != null)
                    parametrs.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception userTrades";
                }
            }
        }

        /// <summary>
        /// Fetch trade fee.
        /// </summary>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> TradeFeeAsync(string? symbol = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/asset/tradeFee?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                if (symbol != null)
                    parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception tradeFee";
                }
            }
        }
        #endregion

        #region OrderEndpoints
        /// <summary>
        /// Check an order's status.
        /// </summary>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> QueryOrderAsync(string symbol, long? orderId = null, string? origClientOrderId = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/order?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                if (orderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("orderId", orderId.ToString()));
                if (origClientOrderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("origClientOrderId", origClientOrderId));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception queryOrder";
                }
            }
        }

        /// <summary>
        /// Send in a new order.
        /// </summary>     
        /// <param name="newClientOrderId">A unique id among open orders. Automatically generated if not sent.</param>
        /// <param name="stopPrice">Used with STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, and TAKE_PROFIT_LIMIT orders.</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> NewOrderAsync(string symbol, OrderSide side, OrderType type, TimeInForce? timeInForce = null, float? quantity = null, float? quoteOrderQty = null, float? price = null, string? newClientOrderId = null, float? stopPrice = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/order?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                parametrs.Add(new KeyValuePair<string, string>("side", side.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("type", type.ToString()));

                if (timeInForce != null)
                    parametrs.Add(new KeyValuePair<string, string>("timeInForce", timeInForce.ToString()));
                if (quantity != null)
                    parametrs.Add(new KeyValuePair<string, string>("quantity", quantity.ToString()));
                if (quoteOrderQty != null)
                    parametrs.Add(new KeyValuePair<string, string>("quoteOrderQty", quoteOrderQty.ToString()));
                if (price != null)
                    parametrs.Add(new KeyValuePair<string, string>("price", price.ToString()));
                if (newClientOrderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("type", newClientOrderId));
                if (stopPrice != null)
                    parametrs.Add(new KeyValuePair<string, string>("stopPrice", stopPrice.ToString()));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                parametrs.Add(new KeyValuePair<string, string>("signature", CalculateSignature(parametrs)));                

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(parametrs));

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception newOrder";
                }
            }
        }

        /// <summary>
        /// Cancel an active order.
        /// </summary>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> DeleteOrderAsync(string symbol, long? orderId = null, string? origClientOrderId = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/order?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                if (orderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("orderId", orderId.ToString()));
                if (origClientOrderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("origClientOrderId", origClientOrderId));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception deleteOrder";
                }
            }
        }

        /// <summary>
        /// Get all account orders; active, canceled, or filled.
        /// </summary>
        /// <param name="limit">Default 500; max 1000.</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> AllOrdersAsync(string symbol, long? orderId = null, long? startTime = null, long? endTime = null, int? limit = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/allOrders?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                if (orderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("orderId", orderId.ToString()));
                if (startTime != null)
                    parametrs.Add(new KeyValuePair<string, string>("startTime", startTime.ToString()));
                if (endTime != null)
                    parametrs.Add(new KeyValuePair<string, string>("endTime", endTime.ToString()));
                if (limit != null)
                    parametrs.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception allOrders";
                }
            }
        }

        /// <summary>
        /// Get all open orders on a symbol. Careful when accessing this with no symbol. Weight(IP): 3 for a single symbol; 40 when the symbol parameter is omitted;
        /// </summary>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> CurrentOpenOrdersAsync(string? symbol = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/openOrders?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                if (symbol != null)
                    parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception openOrders";
                }
            }
        }

        /// <summary>
        /// Cancels all active orders on a symbol or symbols. 
        /// </summary>
        /// <param name="symbol">symbols, eg: BTCUSDT,BTCBUSD</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> CancelAllOpenOrdersOnSymbolsAsync(string symbol, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/openOrders?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();
                                
                parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception cancelOpenOrders";
                }
            }
        }

        /// <summary>
        /// Send in a new oco order.
        /// </summary>
        /// <param name="listClientOrderId">A unique id for order list.</param>
        /// <param name="stopClientOrderId">A unique id for stop limit order.</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> NewOcoOrderAsync(string symbol, OrderSide side, float quantity, float price, float stopPrice, string? listClientOrderId = null, string? stopClientOrderId = null, long? recvWindow = null, float? stopLimitPrice = null, TimeInForce? timeInForce = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/order/oco?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                parametrs.Add(new KeyValuePair<string, string>("side", side.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("quantity", quantity.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("price", price.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("symbol", stopPrice.ToString()));
               
                if (listClientOrderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("listClientOrderId", listClientOrderId));
                if (stopClientOrderId != null)
                    parametrs.Add(new KeyValuePair<string, string>("stopClientOrderId", stopClientOrderId));
                if (stopLimitPrice != null)
                    parametrs.Add(new KeyValuePair<string, string>("stopLimitPrice", stopLimitPrice.ToString()));
                if (timeInForce != null)
                    parametrs.Add(new KeyValuePair<string, string>("timeInForce", timeInForce.ToString()));
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));

                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));
                parametrs.Add(new KeyValuePair<string, string>("signature", CalculateSignature(parametrs)));

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(parametrs));

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception newOcoOrder";
                }
            }
        }

        /// <summary>
        /// Cancel an active order.
        /// </summary>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> DeleteOcoOrderAsync(string symbol, long? orderListId = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/order?";

                List<KeyValuePair<string, string>> parametrs = new List<KeyValuePair<string, string>>();

                parametrs.Add(new KeyValuePair<string, string>("symbol", symbol));
                if (orderListId != null)
                    parametrs.Add(new KeyValuePair<string, string>("orderListId", orderListId.ToString()));                
                if (recvWindow != null)
                    parametrs.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));
                parametrs.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parametrs);

                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception deleteOrder";
                }
            }
        }

        #endregion

        #region WalletEndpoints

        /// <summary>
        /// Submit a withdraw request.
        /// </summary>
        /// <param name="withdrawOrderId">Client's custom ID for withdraw order, Server does not check it's uniqueness. Automatically generated if not sent</param>
        /// <param name="addressTag">Secondary address identifier for coins like XRP,XMR etc</param>
        /// <param name="transactionFeeFlag">When making internal transfer, true for returning the fee to the destination account; false for returning the fee back to the departure account. Default false</param>
        /// <param name="name">Description of the address. Space in name should be encoded into %20</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> WithdrawAsync(string coin, string address, decimal amount, string? withdrawOrderId = null, string? network = null, string? addressTag = null, bool transactionFeeFlag = false, string? name = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/capital/withdraw?";

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("coin", coin));
                parameters.Add(new KeyValuePair<string, string>("address", address));
                parameters.Add(new KeyValuePair<string, string>("amount", amount.ToString()));

                if (!string.IsNullOrEmpty(withdrawOrderId))
                    parameters.Add(new KeyValuePair<string, string>("withdrawOrderId", withdrawOrderId));
                if (!string.IsNullOrEmpty(network))
                    parameters.Add(new KeyValuePair<string, string>("network", network));
                if (!string.IsNullOrEmpty(addressTag))
                    parameters.Add(new KeyValuePair<string, string>("addressTag", addressTag));
                if (transactionFeeFlag)
                    parameters.Add(new KeyValuePair<string, string>("transactionFeeFlag", transactionFeeFlag.ToString()));
                if (!string.IsNullOrEmpty(name))
                    parameters.Add(new KeyValuePair<string, string>("name", name));
                if (recvWindow != null)
                    parameters.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));

                // Добавление временной метки
                parameters.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                // Расчет подписи
                parameters.Add(new KeyValuePair<string, string>("signature", CalculateSignature(parameters)));

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(parameters));

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception withdraw";
                }
            }
        }

        /// <summary>
        /// Fetch deposit address with network.
        /// </summary>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> GetDepositAddressAsync(string coin, string? network = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/capital/deposit/address?";

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("coin", coin));

                if (!string.IsNullOrEmpty(network))
                    parameters.Add(new KeyValuePair<string, string>("network", network));
                if (recvWindow != null)
                    parameters.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));

                // Добавление временной метки
                parameters.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parameters);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception capital/deposit/address ";
                }
            }
        }

        /// <summary>
        /// You need to enable Permits Universal Transfer option for the API Key which requests this endpoint.
        /// </summary>
        /// <param name="type">MAIN_FUTURE Spot account transfer to USDⓈ-M Futures account</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> UserUniversalTransferAsync(AccountType type, string asset, decimal amount, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/asset/transfer?";

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("type", type.ToString()));
                parameters.Add(new KeyValuePair<string, string>("asset", asset));
                parameters.Add(new KeyValuePair<string, string>("amount", amount.ToString()));

                if (recvWindow != null)
                    parameters.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));

                // Добавление временной метки
                parameters.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                // Расчет подписи
                parameters.Add(new KeyValuePair<string, string>("signature", CalculateSignature(parameters)));

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(parameters));

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception asset/transfer";
                }
            }
        }

        /// <summary>
        /// Fetch deposit history.
        /// </summary>
        /// <param name="status">0(0:pending,6: credited but cannot withdraw, 1:success)</param>
        /// <param name="startTime">Default: 90 days from current timestamp</param>
        /// <param name="endTime">Default: present timestamp</param>
        /// <param name="offset">Default:0</param>
        /// <param name="limit">Default 100; max 1000.</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> GetDepositHistoryAsync(string? coin = null, string? txId = null, int? status = null, long? startTime = null, long? endTime = null, int? offset = null, int? limit = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/capital/deposit/history?";

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

                if (!string.IsNullOrEmpty(coin))
                    parameters.Add(new KeyValuePair<string, string>("coin", coin));
                if (!string.IsNullOrEmpty(txId))
                    parameters.Add(new KeyValuePair<string, string>("txId", txId));
                if (status != null)
                    parameters.Add(new KeyValuePair<string, string>("status", status.ToString()));
                if (startTime != null)
                    parameters.Add(new KeyValuePair<string, string>("startTime", startTime.ToString()));
                if (endTime != null)
                    parameters.Add(new KeyValuePair<string, string>("endTime", endTime.ToString()));
                if (offset != null)
                    parameters.Add(new KeyValuePair<string, string>("offset", offset.ToString()));
                if (limit != null)
                    parameters.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                if (recvWindow != null)
                    parameters.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));

                // Добавление временной метки
                parameters.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parameters);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception capital/deposit/history";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">MAIN_FUTURE Spot account transfer to USDⓈ-M Futures account</param>
        /// <param name="startTime">Support query within the last 6 months only</param>
        /// <param name="endTime">If startTime and endTime not sent, return records of the last 7 days by default</param>
        /// <param name="current">Default 1</param>
        /// <param name="size">Default 10, Max 100</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> GetUserUniversalTransferHistoryAsync(AccountType type, long? startTime = null, long? endTime = null, long? current = null, long? size = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/asset/transfer-history?";

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("type", type.ToString()));

                if (startTime != null)
                    parameters.Add(new KeyValuePair<string, string>("startTime", startTime.ToString()));
                if (endTime != null)
                    parameters.Add(new KeyValuePair<string, string>("endTime", endTime.ToString()));
                if (current != null)
                    parameters.Add(new KeyValuePair<string, string>("current", current.ToString()));
                if (size != null)
                    parameters.Add(new KeyValuePair<string, string>("size", size.ToString()));
                if (recvWindow != null)
                    parameters.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));

                // Добавление временной метки
                parameters.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parameters);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception asset/transfer-history";
                }
            }
        }

        /// <summary>
        /// Fetch withdraw history.
        /// </summary>
        /// <param name="withdrawOrderId">client id for withdraw</param>
        /// <param name="status">0(0:Email Sent,1:Cancelled 2:Awaiting Approval 3:Rejected 4:Processing 5:Failure 6:Completed)</param>
        /// <param name="limit">Default 100; max 1000</param>
        /// <param name="startTime">Default: 90 days from current timestamp</param>
        /// <param name="endTime">Default: present timestamp</param>
        /// <param name="recvWindow">The value cannot be greater than 60000</param>
        /// <returns></returns>
        public async Task<string> GetWithdrawHistoryAsync(string? coin = null, string? withdrawOrderId = null, int? status = null, int? offset = null, int? limit = null, long? startTime = null, long? endTime = null, long? recvWindow = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                string url = baseURL + market + "/v1/capital/withdraw/history?";

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

                if (!string.IsNullOrEmpty(coin))
                    parameters.Add(new KeyValuePair<string, string>("coin", coin));
                if (!string.IsNullOrEmpty(withdrawOrderId))
                    parameters.Add(new KeyValuePair<string, string>("withdrawOrderId", withdrawOrderId));
                if (status != null)
                    parameters.Add(new KeyValuePair<string, string>("status", status.ToString()));
                if (offset != null)
                    parameters.Add(new KeyValuePair<string, string>("offset", offset.ToString()));
                if (limit != null)
                    parameters.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                if (startTime != null)
                    parameters.Add(new KeyValuePair<string, string>("startTime", startTime.ToString()));
                if (endTime != null)
                    parameters.Add(new KeyValuePair<string, string>("endTime", endTime.ToString()));
                if (recvWindow != null)
                    parameters.Add(new KeyValuePair<string, string>("recvWindow", recvWindow.ToString()));

                // Добавление временной метки
                parameters.Add(new KeyValuePair<string, string>("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));

                url += GenerateStringWithSignature(parameters);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;

                }
                catch
                {
                    return "Exception during getting withdraw history";
                }
            }
        }

        #endregion

    }

    //public class FuturesAPIClientCommEX : APIClientCommEX
    //{
    //    public FuturesAPIClientCommEX(string apiKey, string apiSecret) : base("/fapi/v1", apiKey, apiSecret)
    //    {
    //    }
    //}
}
