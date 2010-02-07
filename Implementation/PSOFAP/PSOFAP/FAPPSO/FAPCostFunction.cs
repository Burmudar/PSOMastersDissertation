using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAP.PSO.Interfaces;
using PSOFAP.FAPModel.Interfaces;

namespace PSOFAP.FAPPSO
{
    public class FAPCostFunction : IFitnessFunction<ICell[]>
    {
        public ICellRelation[] InterferenceMatrix { get; set; }
		public double MinTolerableInterference {get; set;}
        public FAPCostFunction(FAPModel.FAPModel model)
        {
            InterferenceMatrix = model.InterferenceMatrix.ToArray();
			MinTolerableInterference = model.GeneralInformation.MinTolerableInterference;
        }

        #region IFitnessFunction<ICell[]> Members

        public double Evaluate(ICell[] position)
        {
            double costValue = 0;
            costValue = doCalculation(position, costValue);
            return costValue;
        }


							
		private Boolean IsInterferenceValueIsAboveThreshold(double interferenceValue)
		{
			return interferenceValue >= MinTolerableInterference;
		}

        private double doCalculation(ICell[] position, double costValue)
        {
            foreach (ICellRelation cellRelation in InterferenceMatrix)
            {
                ICell cell = position[cellRelation.CellIndex[0]];
                ICell interferingCell = position[cellRelation.CellIndex[1]];
                double value = 0;
                for (int i = 0; i < interferingCell.Frequencies.Length; i++)
                {
                    for (int j = 0; j < cell.Frequencies.Length; j++)
                    {
                        if (interferingCell.Frequencies[i] == cell.Frequencies[j])
                        {
							if(IsInterferenceValueIsAboveThreshold(cellRelation.DA[0]))
							{
                            	value += cellRelation.DA[0];
							}
                        }
                        else if (Math.Abs(interferingCell.Frequencies[i] - cell.Frequencies[j]) == 1)
                        {
							if(IsInterferenceValueIsAboveThreshold(cellRelation.DA[1]))
							{
                            	value += cellRelation.DA[1];
							}
                        }
                    }
                }
                costValue += value;

            }
            return costValue;
        }

        #endregion
    }
}
