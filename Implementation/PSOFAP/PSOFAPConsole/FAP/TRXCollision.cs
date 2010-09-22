using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSOFAPConsole.FAP
{
    public class TRXCollision
    {
        public TRX TRX { get; protected set; }
        public int Index { get; protected set; }

        public TRXCollision(TRX trx, int index)
        {
            TRX = trx;
            Index = index;
        }
    }
}
