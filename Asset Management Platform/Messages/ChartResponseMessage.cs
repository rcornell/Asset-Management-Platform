using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class ChartResponseMessage
    {
        public bool ShowAll;
        public bool ShowEquities;
        public bool ShowMutualFunds;
        public ObservableCollection<PositionByWeight> ChartPositions;

        public ChartResponseMessage(ObservableCollection<PositionByWeight> chartPositions, bool showAll, bool showEquities, bool showMutualFunds)
        {
            ChartPositions = chartPositions;
            ShowAll = showAll;
            ShowEquities = showEquities;
            ShowMutualFunds = showMutualFunds;
        }
    }
}
