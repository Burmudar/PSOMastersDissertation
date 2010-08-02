using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PSOFAP.FAPPSO;

namespace PSOFAPConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread t1 = new Thread((ThreadStart)delegate
            {
                FAPPSOAlgorithm algo1 = new FAPPSOAlgorithm(10);
                algo1.PerformCostTest();
                algo1.Initialize();
                algo1.Start();
            });
            //Thread t2 = new Thread((ThreadStart)delegate
            //{
            //    FAPPSOAlgorithm algo1 = new FAPPSOAlgorithm(1000);
            //    algo1.Initialize();
            //    algo1.Start();
            //});
            //Thread t3 = new Thread((ThreadStart)delegate
            //{
            //    FAPPSOAlgorithm algo1 = new FAPPSOAlgorithm(1000);
            //    algo1.Initialize();
            //    algo1.Start();
            //});
            //Thread t4 = new Thread((ThreadStart)delegate
            //{
            //    FAPPSOAlgorithm algo1 = new FAPPSOAlgorithm(1000);
            //    algo1.Initialize();
            //    algo1.Start();
            //});
            //Thread t5 = new Thread((ThreadStart)delegate
            //{
            //    FAPPSOAlgorithm algo1 = new FAPPSOAlgorithm(1000);
            //    algo1.Initialize();
            //    algo1.Start();
            //});
            //Thread t6 = new Thread((ThreadStart)delegate
            //{
            //    FAPPSOAlgorithm algo1 = new FAPPSOAlgorithm(1000);
            //    algo1.Initialize();
            //    algo1.Start();
            //});
            //Thread t7 = new Thread((ThreadStart)delegate
            //{
            //    FAPPSOAlgorithm algo1 = new FAPPSOAlgorithm(1000);
            //    algo1.Initialize();
            //    algo1.Start();
            //});
            //Thread t8 = new Thread((ThreadStart)delegate
            //{
            //    FAPPSOAlgorithm algo1 = new FAPPSOAlgorithm(1000);
            //    algo1.Initialize();
            //    algo1.Start();
            //});
            t1.Start();
            //t2.Start();
            //t3.Start();
            //t4.Start();
            //t5.Start();
            //t6.Start();
            //t7.Start();
            //t8.Start();
        }
    }
}
