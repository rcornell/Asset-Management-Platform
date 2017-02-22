using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class PositionMessage
    {
        public List<Position> Positions;
        public Position Position;
        public bool IsStartup;

        public PositionMessage(List<Position> positions, bool isStartup)
        {
            Positions = positions;
            IsStartup = isStartup; 
        }

        public PositionMessage(Position position, bool isStartup)
        {
            Position = position;
            IsStartup = isStartup;
        }
    }
}
