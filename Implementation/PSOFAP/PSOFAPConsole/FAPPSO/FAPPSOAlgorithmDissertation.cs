using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAPConsole.FAP.Interfaces;
using PSOFAPConsole.PSO;
using PSOFAPConsole.PSO.Interfaces;
using System.Diagnostics;
using PSOFAPConsole.FAPPSO;
using PSOFAPConsole.FAP;

namespace PSOFAPConsole.FAPPSO
{
    using ParticleCellArray = Particle<ICell[]>;
    using FitnessFuncCellArray = IFitnessFunction<ICell[]>;
    using ParticleMoveFunction = IMoveFunction<Particle<ICell[]>>;
    using PositionGenCellArray = IPositionGenerator<ICell[]>;
    using GlobalBestSelector = IGlobalBestSelector<Particle<ICell[]>>;
    using System.Threading.Tasks;
    using System.IO;

    public class FAPPSOAlgorithm : PSOAlgorithm<ICell[]>
    {
        public List<ParticleCellArray> Particles { get; set;}
        public ParticleCellArray GlobalBest { get; set; }
        public int Population { get; set; }
        public FitnessFuncCellArray EvaluationFunction { get; set; }
        public ParticleMoveFunction VelocityFunction { get; set; }
        public PositionGenCellArray Generator { get; set; }
        public GlobalBestSelector Selector { get; set; }
        public FAPCostFunction Analyser { get; set; }
        private ICellIntegrityChecker cellIntegrityChecker;
        public String BenchName { get; set; }
        private List<double> runFitness;

        public FAPPSOAlgorithm(string benchName,int population,FitnessFuncCellArray evalFunction, 
			ParticleMoveFunction moveFunction, PositionGenCellArray positionGenerator, ICellIntegrityChecker checker,
			GlobalBestSelector selector, FAPCostFunction analyser)
        {
            BenchName = benchName;
            Particles = new List<ParticleCellArray>();
            Population = population;
            EvaluationFunction = evalFunction;
            VelocityFunction = moveFunction;
            Generator = positionGenerator;
            cellIntegrityChecker = checker;
            Selector = selector;
            Analyser = analyser;
            BenchName += "("+moveFunction.GetType().Name+")";
            BenchName += "("+selector.GetConstructionMethodName()+")";
            BenchName += "[" + evalFunction.GetType().Name + "]";
            Console.WriteLine(BenchName);
            runFitness = new List<double>();
            Initialize();
        }

        #region PSOAlgorithm<T> Members

        public void Initialize()
        {
            for (int i = 0; i < Population; i++)
            {
                Particles.Add(new Particle<ICell[]>(Generator.GeneratePosition()));
            }
        }

        public Particle<ICell[]> GetGlobalBest()
        {
            return GlobalBest;
        }

        public void Start()
        {
            DateTime start = DateTime.Now;
            for (int i = 0; i < 50; i++)
            {
                Parallel.ForEach(Particles, EvaluateParticle);
                UpdateGlobalBest();
                UpdateSwarmMovement();
                runFitness.Add(GlobalBest.Fitness);
            }
    }

        private void UpdateGlobalBest()
        {
            GlobalBest = Selector.FindGlobalBest(Particles);
            GlobalBest.Evaluate(EvaluationFunction);
        }

        //Applies the velocity function to each particle in the swarm
        public void UpdateSwarmMovement()
        {
            Parallel.ForEach(Particles, particle => particle.MoveTowards(GlobalBest,VelocityFunction));
        }

        public void EvaluateParticle(Particle<ICell[]> particle)
        {
            double cost = particle.Evaluate(EvaluationFunction);
        }

        #endregion
    }
}
