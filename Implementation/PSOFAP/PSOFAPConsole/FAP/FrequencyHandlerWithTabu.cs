using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSOFAPConsole.FAP
{
    public class FrequencyHandlerWithTabu : FrequencyHandler
    {
        public Queue<TRX> TabuList { get; set; }

        public FrequencyHandlerWithTabu(BasicCell cell)
            : base(cell)
        {
        }

        public FrequencyHandlerWithTabu(FrequencyHandlerWithTabu frequencyHandler)
            : base(frequencyHandler)
        {
        }
    }
}
