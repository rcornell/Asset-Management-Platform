﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class TradePreviewMessage
    {
        public Security Security;

        public TradePreviewMessage(Security security)
        {
            Security = security;
        }
    }
}
