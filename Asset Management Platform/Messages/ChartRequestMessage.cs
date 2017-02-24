using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class ChartRequestMessage
    {
        public bool ShowAll;
        public bool ShowEquities;
        public bool ShowMutualFunds;
        public List<Position> Positions;

        public ChartRequestMessage(List<Position> positions, bool showAll, bool showEquities, bool showMutualFunds)
        {
            Positions = positions;
            ShowAll = showAll;
            ShowEquities = showEquities;
            ShowMutualFunds = showMutualFunds;
        }
    }
}
