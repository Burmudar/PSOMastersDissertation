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
            BenchName = "Bench-" + BenchName + "(" + Population + ")-" + start.ToLongDateString() + " " + 
			            start.ToLongTimeString().Replace(':', '-') + "#" + this.GetHashCode();
            String filename = BenchName +".csv";
            String statsFilename = "Stats-" + filename;
            Console.WriteLine("Outputting results to <{0}> and Stats to <{1}>", filename, statsFilename);
            using (StreamWriter swriter = new StreamWriter(new FileStream(filename, FileMode.CreateNew)), statsWriter = new StreamWriter(new FileStream(statsFilename, FileMode.CreateNew)))
            {
                swriter.WriteLine("iteration, fitness");
            
                statsWriter.WriteLine("iteration, fitness,co-channel max, co-channel avg, co-channel std, adj-channel max, adj-channel avg, adj-channel std, trx max, trx avg, trx std, ex-0.01, ex-0.02, ex-0.03, ex-0.04, ex-0.05, ex-0.10, ex-0.15, ex-0.20, ex-0.50");
                for (int i = 0; i < 50; i++)
                {
                    Console.Write("Iteration {0} ... ", i + 1);
                    Parallel.ForEach(Particles, EvaluateParticle);
                    UpdateGlobalBest();
                    UpdateSwarmMovement();
                    runFitness.Add(GlobalBest.Fitness);
                    swriter.WriteLine("{0},{1}", i, GlobalBest.Fitness.ToString("F"));
                    double[] stats = Analyser.Stats(GlobalBest.Position);
                    statsWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}," +
                    	"{17},{18},{19}",
                        i, stats[0].ToString("F"), stats[1].ToString("F"), stats[2].ToString("F"), 
						stats[3].ToString("F"), stats[4].ToString("F"), stats[5].ToString("F"), 
						stats[6].ToString("F"), stats[7].ToString("F"), stats[8].ToString("F"), 
						stats[9].ToString("F"), stats[10].ToString("F"), stats[11].ToString("F"), 
						stats[12].ToString("F"), stats[13].ToString("F"), stats[14].ToString("F"), 
						stats[15].ToString("F"), stats[16].ToString("F"), stats[17].ToString("F"), 
						stats[18].ToString("F"));
                    Console.WriteLine("Done");
                    if ((i + 1) % 10 == 0)
                    {
                        Console.WriteLine("Flushing");
                        swriter.Flush();
                        statsWriter.Flush();
                    }
                
                }
                swriter.WriteLine("StdDev {0}, Avg {1}", StdStatisticalAnalyser.CalculateStdDev(runFitness), runFitness.Average());
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
