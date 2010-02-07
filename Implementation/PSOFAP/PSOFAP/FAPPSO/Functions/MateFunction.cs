using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAP.PSO.Interfaces;
using PSOFAP.FAPModel.Interfaces;

namespace PSOFAP.FAPPSO
{
    public class MateFunction : IMoveFunction<ICell[]>
    {
        FAPPositionGenerator Generator;
		float basePercentage {get; set;}
		float blockPercentage {get; set;}
		int copyAmount {get; set;}
		int blockSize {get; set;}
		
        public MateFunction(FAPPositionGenerator gen,float percentage,float blockPercentage)
        {
            Generator = gen;
			basePercentage = percentage;
			this.blockPercentage = blockPercentage;
        }

        #region IMoveFunction<ICell[]> Members

        public ICell[] MoveTowards(ICell[] from, ICell[] to)
        {
            copyAmount = defineHowMuchOfACellToCopy(from.Length);
            int c2 = (int)(to.Length * 0.6);
            ICell[] random = Generator.GeneratePosition();
            for (int c1 = (int)(from.Length * 0.4); c1 < from.Length; c1++)
            {
                from[c1] = to[c1];
            }
            for (int i = 0; i < from.Length / 2; i++)
            {
                from[i] = random[i];
            }
            return from;
        }


        #endregion
		
		private int defineHowMuchOfACellToCopy(int length)
		{
			return (int)(length * basePercentage);
		}
		
		private int defineBlockSize(int length)
		{
			return (int)(length * blockSize);
		}
		
		private int[] createBlockIndexArray(int blockAmount,int length)
		{
			int[] blockIndex = new int[length / blockAmount + 1];
			for(int i = 1; i < blockIndex.Length; i++)
			{
				blockIndex[i] = blockAmount * i;
			}
			return blockIndex;
		}
		
		
    }
}
