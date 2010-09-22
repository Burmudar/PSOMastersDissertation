using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PSOFAPConsole.FAPPSO;
using PSOFAPConsole.FAP;
using PSOFAPConsole.PSO;
using PSOFAPConsole.FAP.Interfaces;

namespace PSOFAPConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread t1 = new Thread((ThreadStart)delegate
            {
                FAPModelFactory modelFactory = new FAPModelFactory();
                FAPPSOFactory factory = new FAPPSOFactory(modelFactory.CreateSiemens1Model(),500);
                PSOAlgorithm<ICell[]> algo1 = factory.CreateIndexBasedFAPPSOWithGlobalBestTRXBuilder(1.52, 3.41);
                algo1.Start();
            });           
            t1.Start();
        }
    }
}
