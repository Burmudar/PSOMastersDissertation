using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAPConsole.PSO.Interfaces;
using PSOFAPConsole.FAP.Interfaces;
using PSOFAPConsole.FAP;
using System.Threading.Tasks;
using System.Threading;

namespace PSOFAPConsole.FAPPSO
{
    public class FAPIndexCostFunction : FAPCostFunction
    {
        private int[] channels;
        public FAPIndexCostFunction(FAPModel model)
            : base(model)
        {
            channels = model.Channels;
        }

        protected override double CalculateInterfernece(ICell[] position)
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
                        int frequencyA = channels[interferingCell.Frequencies[i].Value];
                        int frequencyB = channels[cell.Frequencies[j].Value];
                        double trxInf = 0;
                        if (SameFrequency(frequencyA, frequencyB))
                        {
                            trxInf = ZeroIfOusideInterferneceThreshold(cellRelation.DA[0]);
                            interference += trxInf;
                        }
                        else if (base.FrequenciesDifferByOne(frequencyA, frequencyB))
                        {
                            trxInf = ZeroIfOusideInterferneceThreshold(cellRelation.DA[1]);
                            interference += trxInf;
                        }
                        cell.FrequencyHandler.SetSingleTrxInterference(j, trxInf);
                    }
                    //cell.Interference += interference;
                }
                cell.Interference += interference;
                totalInterference += interference;

            }
            return totalInterference;
        }
    }
}
