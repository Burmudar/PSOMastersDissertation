using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAPConsole.FAP;
using PSOFAPConsole.PSO;
using PSOFAPConsole.PSO.Interfaces;
using PSOFAPConsole.FAP.Interfaces;

namespace PSOFAPConsole.FAPPSO
{
    using ParticleCellArray = Particle<ICell[]>;
    using FitnessFuncCellArray = IFitnessFunction<ICell[]>;
    using ParticleMoveFunction = IMoveFunction<Particle<ICell[]>>;
    using PositionGenCellArray = IPositionGenerator<ICell[]>;
    using PSOFAPConsole.FAPPSO.Functions;

    public class FAPPSOFactory
    {
        private FAPModel model;
        private int population;

        public FAPPSOFactory(FAPModel model,int pop)
        {
            this.model = model;
            population = pop;
        }

        public PSOAlgorithm<ICell[]> CreateFrequencyValueBased(double localCoefficient, double globalCoefficient)
        {
            PositionGenCellArray generator = new FrequencyPositionGenerator(model);
            FitnessFuncCellArray evalFunction = new FAPCostFunction(model);
            ParticleMoveFunction moveFunction = new ParticlePerTrxFunction(model, model.GeneralInformation.Spectrum[0], model.GeneralInformation.Spectrum[1],
                 localCoefficient, globalCoefficient, CreateCollisionResolver());
            ICellIntegrityChecker checker = new GBCViolationChecker(model.GeneralInformation.GloballyBlockedChannels);
            return new FAPPSOAlgorithm(population, evalFunction, moveFunction, generator, checker,GBestFactory.GetStandardSelector());
        }

        public PSOAlgorithm<ICell[]> CreateFrequencyIndexBased(double localCoefficient, double globalCoefficient)
        {
            PositionGenCellArray generator = new FrequencyIndexPositionGenerator(model);
            FitnessFuncCellArray evalFunction = new FAPIndexCostFunction(model);
            ParticleMoveFunction moveFunction = new ParticlePerTrxFunction(model, 0, model.Channels.Length - 1, localCoefficient, globalCoefficient, CreateCollisionResolver());
            ICellIntegrityChecker checker = new GBCIndexBasedViolationChecker(model);
            return new FAPPSOAlgorithm(population, evalFunction, moveFunction, generator, checker, GBestFactory.GetStandardSelector());
        }

        public PSOAlgorithm<ICell[]> CreateIndexMovementBased(double localCoefficient, double globalCoefficient)
        {
            PositionGenCellArray generator = new FrequencyIndexPositionGenerator(model);
            FitnessFuncCellArray evalFunction = new FAPIndexCostFunction(model);
            ParticleMoveFunction moveFunction = new PerTRXChannelIndexFunction(model, localCoefficient,globalCoefficient,CreateCollisionResolver());
            ICellIntegrityChecker checker = new GBCIndexBasedViolationChecker(model);
            return new FAPPSOAlgorithm(population, evalFunction, moveFunction, generator, checker, GBestFactory.GetStandardSelector());
        }

        public PSOAlgorithm<ICell[]> CreateIndexBasedFAPPSOWithGlobalBestCellBuilder(double localCoefficient, double globalCoefficient)
        {
            PositionGenCellArray generator = new FrequencyIndexPositionGenerator(model);
            FitnessFuncCellArray evalFunction = new FAPIndexCostFunction(model);
            ParticleMoveFunction moveFunction = new ParticlePerTrxFunction(model,0, model.Channels.Length - 1, localCoefficient, globalCoefficient, CreateCollisionResolver());
            ICellIntegrityChecker checker = new GBCIndexBasedViolationChecker(model);
            return new FAPPSOAlgorithm(population, evalFunction, moveFunction, generator, checker, GBestFactory.GetGlobalBestCellBuilderSelector());
        }

        public PSOAlgorithm<ICell[]> CreateIndexMovementBasedWithGlobalBestCellBuilder(double localCoefficient, double globalCoefficient)
        {
            PositionGenCellArray generator = new FrequencyIndexPositionGenerator(model);
            FitnessFuncCellArray evalFunction = new FAPIndexCostFunction(model);
            ParticleMoveFunction moveFunction = new PerTRXChannelIndexFunction(model,localCoefficient,globalCoefficient, CreateCollisionResolver());
            ICellIntegrityChecker checker = new GBCIndexBasedViolationChecker(model);
            return new FAPPSOAlgorithm(population, evalFunction, moveFunction, generator, checker, GBestFactory.GetGlobalBestCellBuilderSelector());
        }

        public PSOAlgorithm<ICell[]> CreateIndexBasedFAPPSOWithGlobalBestTRXBuilder(double localCoefficient, double globalCoefficient)
        {
            PositionGenCellArray generator = new FrequencyIndexPositionGenerator(model);
            FitnessFuncCellArray evalFunction = new FAPIndexCostFunction(model);
            ParticleMoveFunction moveFunction = new ParticlePerTrxFunction(model, 0, model.Channels.Length - 1, localCoefficient, globalCoefficient, CreateCollisionResolver());
            ICellIntegrityChecker checker = new GBCIndexBasedViolationChecker(model);
            return new FAPPSOAlgorithm(population, evalFunction, moveFunction, generator, checker, GBestFactory.GetGlobalBestCellBuilderSelector());
        }

        public PSOAlgorithm<ICell[]> CreateIndexMovementBasedWithGlobalBestTRXBuilder(double localCoefficient, double globalCoefficient)
        {
            PositionGenCellArray generator = new FrequencyIndexPositionGenerator(model);
            FitnessFuncCellArray evalFunction = new FAPIndexCostFunction(model);
            ParticleMoveFunction moveFunction = new PerTRXChannelIndexFunction(model, localCoefficient,globalCoefficient,CreateCollisionResolver());
            ICellIntegrityChecker checker = new GBCIndexBasedViolationChecker(model);
            return new FAPPSOAlgorithm(population, evalFunction, moveFunction, generator, checker, GBestFactory.GetGlobalBestCellBuilderSelector());
        }

        protected AbstractCollisionResolver CreateCollisionResolver()
        {
            return new RandomCollisionResolver(model.Channels);
        }

    }
}
