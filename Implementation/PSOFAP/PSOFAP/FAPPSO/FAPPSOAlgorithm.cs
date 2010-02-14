using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAP.PSO;
using PSOFAP.PSO.Interfaces;
using PSOFAP.FAPModel.Interfaces;

namespace PSOFAP.FAPPSO
{
    public class FAPPSOAlgorithm : PSOAlgorithm<ICell[]>
    {
        public List<Particle<ICell[]>> Particles { get; set;}
        public Particle<ICell[]> GlobalBest { get; set; }
        public int Population { get; set; }
        public IFitnessFunction<ICell[]> EvaluationFunction { get; set; }
        public IMoveFunction<Particle<ICell[]>> VelocityFunction { get; set; }
        public FAPPositionGenerator Generator { get; set; }
        private Boolean fakeMode = false;
        public FAPPSOAlgorithm(int amount)
        {
            Particles = new List<Particle<ICell[]>>();
            Population = amount;
            
        }

        #region PSOAlgorithm<T> Members

        public void Initialize()
        {
            FAPModel.FAPModel model = new PSOFAP.FAPModel.FAPModel("../../FAPModel/Problems/siemens1");
            model.createModel();
            Generator = new FAPPositionGenerator(model);
            for (int i = 0; i < Population; i++)
            {
                Particles.Add(new Particle<ICell[]>(Generator.GeneratePosition()));
            }
            EvaluationFunction = new FAPCostFunction(model);
            //VelocityFunction = new PerTRXFunction(model.GeneralInformation.Spectrum,model.GeneralInformation.GloballyBlockedChannels);
            //VelocityFunction = new MateFunction((FAPPositionGenerator) generator);
            VelocityFunction = new ParticlePerTrxFunction(model.GeneralInformation);
        }
		
		public void PerfromCostAssignmentTest()
		{
			FAPModel.FAPModel model = new PSOFAP.FAPModel.FAPModel("../../FAPModel/Problems/siemens1");
            model.createModel();
            
			model.LoadTestData("../../FAPModel/Problems/VanOnselen.ass");
			EvaluationFunction = new FAPCostFunction(model);
			Console.WriteLine("Cost: {0}",EvaluationFunction.Evaluate(model.Cells));
		}

        public Particle<ICell[]> GetGlobalBest()
        {
            return GlobalBest;
        }

        public void Start()
        {
            int i = 0;
            double fitness = 0.0;
            int j = 0;
            while (true)
            {
                foreach (Particle<ICell[]> particle in Particles)
                {
                    EvaluateParticle(particle);
                }
                UpdateSwarmMovement();
                i++;
                if (GlobalBest.Fitness != fitness)
                {
                        fitness = GlobalBest.Fitness;
                }
                else
                {
					if(j == 5)
					{
						fakeMode = true;
						j++;
					}
					else{
						j++;
						if(j == 10)
						{
							j = 0;
							fakeMode = false;
						}
					}
                }
                Console.WriteLine("Hashcode: {0} Fitness: {1} Using FakeGlobalBest: {2} j = {3}",this.GetHashCode(),GlobalBest.Fitness,fakeMode,j);
            }
        }

        public void UpdateSwarmMovement()
        {
            foreach (Particle<ICell[]> particle in Particles)
            {
                VelocityFunction.MoveTowards(particle, GlobalBest);
            }
        }

        public void EvaluateParticle(Particle<ICell[]> particle)
        {
            double cost = particle.Evaluate(EvaluationFunction);
            if(IsGlobalBest(cost) && fakeMode == false)
            {
                GlobalBest = (Particle<ICell[]>)particle.Clone();
            }
        }

        #endregion

        public bool IsGlobalBest(double cost)
        {
            if (GlobalBest == null)
            {
                return true;
            }
            if (cost < GlobalBest.Fitness)
            {
                return true;
            }
            return false;
        }
    }
}
