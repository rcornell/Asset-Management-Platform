using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class StockDataResponseMessage
    {
        public List<Security> Securities;
        public Security Security;
        public List<Position> Positions;
        public Position Position;
        public bool IsStartupResponse;
        public bool IsScreenerResponse;
        public bool IsPreviewResponse;

        public StockDataResponseMessage(List<Security> securities, bool isStartup)
        {
            Securities = securities;
            IsStartupResponse = isStartup;
        }

        public StockDataResponseMessage(Security security, bool isPreview, bool isScreener)
        {
            Security = security;
            IsPreviewResponse = isPreview;
            IsScreenerResponse = isScreener;
        }

        public StockDataResponseMessage(List<Position> positions, bool isStartup)
        {
            Positions = positions;
            IsStartupResponse = isStartup;
        }

        public StockDataResponseMessage(Position position, bool isStartup)
        {
            Position = position;
            IsStartupResponse = isStartup;
        }
    }
}
