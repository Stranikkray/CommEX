namespace CommEX
{
    public enum OrderType
    {
        LIMIT,
        MARKET,
        STOP_LOSS,
        STOP_LOSS_LIMIT,
        TAKE_PROFIT,
        TAKE_PROFIT_LIMIT
    }



    public enum OrderSide
    {
        BUY,
        SELL
    }

    public enum TimeInForce
    {        
        GTC,
        IOC,
        FOK,
        GTX
    }

    public enum Interval
    {
        _1m,
        _3m,
        _5m,
        _15m,
        _30m,
        _1h,
        _2h,
        _4h,
        _6h,
        _8h,
        _12h,
        _1d,
        _3d,
        _1w,
        _1M
    }
    
    public enum AccountType
    {
        MAIN_FUTURE,
        FUTURE_MAIN,
        MAIN_FUNDING,
        FUNDING_MAIN,
        FUNDING_FUTURE,
        FUTURE_FUNDING
    }
}
