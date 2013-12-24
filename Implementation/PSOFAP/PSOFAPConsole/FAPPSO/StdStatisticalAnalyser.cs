using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAPConsole.PSO;
using PSOFAPConsole.FAP.Interfaces;
using PSOFAPConsole.FAP;

namespace PSOFAPConsole.FAPPSO
{
    class StdStatisticalAnalyser : FAPCostFunction
    {
        private Particle<ICell[]> particle;
        private int[] channels;
        private double[] infExceeding;
        private List<double> coChInterference;
        private List<double> adjChInterference;
        private List<double> trxInterference;

        public StdStatisticalAnalyser(FAPModel model) : base(model)
        {
            channels = model.Channels;
            infExceeding = new double[10];
        }

        public override double[] Stats(ICell[] position)
        {
            double totalInterference = 0;
            foreach (ICellRelation cellRelation in InterferenceMatrix)
            {
                ICell cell = position[cellRelation.CellIndex[0]];
                ICell interferingCell = position[cellRelation.CellIndex[1]];
                double interference = 0;
                for (int i = 0; i < interferingCell.Frequencies.Length; i++)
                {
                    for (int j = 0; j < cell.Frequencies.Length; j++)
                    {
                        double trxInf = 0.0;
                        if (SameFrequency(interferingCell.FrequencyHandler[i], cell.FrequencyHandler[j]))
                        {
                            double coChInf = ZeroIfOusideInterferneceThreshold(cellRelation.DA[0]);
                            addCoChannelStats(coChInf);
                            trxInf += coChInf;
                        }
                        else if (FrequenciesDifferByOne(interferingCell.FrequencyHandler[i],cell.FrequencyHandler[j]))
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
            return new double[]{totalInterference,coChInterference.Max(), coChInterference.Average(), CalculateStdDev(coChInterference),
            adjChInterference.Max(), adjChInterference.Average(), CalculateStdDev(adjChInterference),
            trxInterference.Max(), trxInterference.Average(), CalculateStdDev(trxInterference),
            infExceeding[0], infExceeding[1], infExceeding[2], infExceeding[3], infExceeding[4],
            infExceeding[5], infExceeding[6], infExceeding[7], infExceeding[8]};
        }

        public static double CalculateStdDev(List<double> values)
        {
            double avg = values.Average();
            double sumOfDifferences = values.Select(val => (val - avg) * (val - avg)).Sum();
            return Math.Sqrt(sumOfDifferences / values.Count); 

        }

        private void addAdjChannelStats(double adjChInf)
        {
            adjChInterference.Add(adjChInf);
        }

        private void addCoChannelStats(double coChInf)
        {
            coChInterference.Add(coChInf);
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
            else
            {
                infExceeding[8] += 1;
            }
        }


    }
}
