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
        public bool IsStartup;

        public StockDataResponseMessage(List<Security> securities, bool isStartup)
        {
            Securities = securities;
            IsStartup = isStartup;
        }

        public StockDataResponseMessage(Security security)
        {
            Security = security;
        }

        public StockDataResponseMessage(List<Position> positions, bool isStartup)
        {
            Positions = positions;
            IsStartup = isStartup;
        }

        public StockDataResponseMessage(Position position, bool isStartup)
        {
            Position = position;
            IsStartup = isStartup;
        }
    }
}
