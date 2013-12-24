using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAPConsole.PSO;
using PSOFAPConsole.FAP.Interfaces;
using PSOFAPConsole.FAP;

namespace PSOFAPConsole.FAPPSO
{
    class IndexStatisticalAnalyser : FAPCostFunction
    {
        private int[] channels;
        private double[] infExceeding;
        private List<double> coChInterference;
        private List<double> adjChInterference;
        private List<double> trxInterference;

        public IndexStatisticalAnalyser(FAPModel model) : base(model)
        {
            channels = model.Channels;
            infExceeding = new double[10];
            coChInterference = new List<double>();
            adjChInterference = new List<double>();
            trxInterference = new List<double>();
        }

        private void reset()
        {
            coChInterference.Clear();
            adjChInterference.Clear();
            trxInterference.Clear();
            for (int i = 0; i < infExceeding.Length; i++)
            {
                infExceeding[i] = 0;
            }
        }

        public override double[] Stats(ICell[] position)
        {
            reset();
            double totalInterference = 0;
            if (position.Length == 0)
            {
                Console.WriteLine("No elements in position");
                return new double[19];
            }
            foreach (ICellRelation cellRelation in InterferenceMatrix)
            {
                ICell cell = position[cellRelation.CellIndex[0]];
                ICell interferingCell = position[cellRelation.CellIndex[1]];
                double interference = 0;
                for (int i = 0; i < interferingCell.Frequencies.Length; i++)
                {
                    for (int j = 0; j < cell.Frequencies.Length; j++)
                    {
                        int frequencyA = channels[interferingCell.Frequencies[i].Value];
                        int frequencyB = channels[cell.Frequencies[j].Value];
                        double trxInf = 0.0;
                        if (SameFrequency(frequencyA, frequencyB))
                        {
                            double coChInf = ZeroIfOusideInterferneceThreshold(cellRelation.DA[0]);
                            addCoChannelStats(coChInf);
                            trxInf += coChInf;
                        }
                        else if (base.FrequenciesDifferByOne(frequencyA, frequencyB))
                        {
                            double adjChInf =  ZeroIfOusideInterferneceThreshold(cellRelation.DA[1]);
                            addAdjChannelStats(adjChInf);
                            trxInf += adjChInf;
                        }
                        addTrxStats(trxInf);
                        interference += trxInf;
                    }
                    //cell.Interference += interference;
                }
                totalInterference += interference;

            }
            bool hasAdjElements = adjChInterference.Count > 0 ? true : false;
            bool hasCoChElements = coChInterference.Count > 0 ? true : false;
            bool hasTrxElements = trxInterference.Count > 0 ? true : false;
            return new double[]{totalInterference,hasCoChElements ? coChInterference.Max() : 0, hasCoChElements ? coChInterference.Average() : 0, CalculateStdDev(coChInterference),
            hasAdjElements ? adjChInterference.Max() : 0, hasAdjElements ? adjChInterference.Average() : 0, CalculateStdDev(adjChInterference),
            hasTrxElements ? trxInterference.Max() : 0, hasTrxElements ? trxInterference.Average() : 0, CalculateStdDev(trxInterference),
            infExceeding[0], infExceeding[1], infExceeding[2], infExceeding[3], infExceeding[4],
            infExceeding[5], infExceeding[6], infExceeding[7], infExceeding[8]};
        }

        private static double CalculateStdDev(List<double> values)
        {
            if (values.Count == 0)
                return 0.0;
            double avg = values.Average();
            double sumOfDifferences = values.Select(val => (val - avg) * (val - avg)).Sum();
            return Math.Sqrt(sumOfDifferences / values.Count); 

        }

        private void addTrxStats(double trxInf)
        {
            trxInterference.Add(trxInf);
            if (trxInf >= 0.01 && trxInf < 0.02)
            {
                infExceeding[0] += 1;
            }
            else if (trxInf >= 0.02 && trxInf < 0.03)
            {
                infExceeding[1] += 1;
            }
            else if (trxInf >= 0.03 && trxInf < 0.04)
            {
                infExceeding[2] += 1;
            }
            else if (trxInf >= 0.04 && trxInf < 0.05)
            {
                infExceeding[3] += 1;
            }
            else if (trxInf >= 0.05 && trxInf < 0.1)
            {
                infExceeding[4] += 1;
            }
            else if (trxInf >= 0.1 && trxInf < 0.15)
            {
                infExceeding[5] += 1;
            }
            else if (trxInf >= 0.15 && trxInf < 0.2)
            {
                infExceeding[6] += 1;
            }
            else if (trxInf >= 0.2 && trxInf < 0.5)
            {
                infExceeding[7] += 1;
            }
			else if (trxInf >= 0.5)
            {
                infExceeding[8] += 1;
            }
        }

        private void addAdjChannelStats(double adjChInf)
        {
            adjChInterference.Add(adjChInf);
        }

        private void addCoChannelStats(double coChInf)
        {
            coChInterference.Add(coChInf);
        }

       


    }
}
