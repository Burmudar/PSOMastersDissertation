using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSOFAPConsole.FAP.Interfaces;
using PSOFAPConsole.FAP;
using PSOFAPConsole.FAPPSO;
using System.Threading.Tasks;
using System.Net.Mail;

namespace PSOFAPConsole.PSO
{
    public class BenchRunner
    {
        public  delegate PSOAlgorithm<ICell[]> pso(int pop,FAPModel model,double c1,double c2);
        public delegate FAPModel ModelCreators();
        public List<int> Populations { get; set; }
        public List<ModelCreators> Models { get; set; }
        public List<pso> PSOCreate { get; set; }

        public BenchRunner()
        {
            Populations = new List<int>();
            Models = new List<ModelCreators>();
            PSOCreate = new List<pso>();
            FAPModelFactory modelFactory = new FAPModelFactory();
            Models.Add(modelFactory.CreateSiemens1Model);
            Models.Add(modelFactory.CreateSiemens2Model);
            Models.Add(modelFactory.CreateSiemens3Model);
            Models.Add(modelFactory.CreateSiemens4Model);
            Populations.Add(20);
            Populations.Add(50);
            Populations.Add(100);
            FAPPSOFactory factory = new FAPPSOFactory();
            PSOCreate.Add(factory.CreateFrequencyIndexBased);
            PSOCreate.Add(factory.CreateIndexBasedFAPPSOWithGlobalBestCellBuilder);
            PSOCreate.Add(factory.CreateIndexBasedFAPPSOWithGlobalBestTRXBuilder);
            PSOCreate.Add(factory.CreateIndexMovementBased);
            PSOCreate.Add(factory.CreateIndexMovementBasedWithGlobalBestCellBuilder);
            PSOCreate.Add(factory.CreateIndexMovementBasedWithGlobalBestTRXBuilder);
        }

        public void StartBenchmark()
        {
            foreach (int population in Populations)
            {
                Console.WriteLine("Starting benchmark with population size <{0}>", population);
                foreach (ModelCreators modelcreator in Models)
                {
                    FAPModel model = modelcreator();
                    foreach (pso psocreator in PSOCreate)
                    {
                        PSOAlgorithm<ICell[]> fapPso = psocreator(population, model, 0.41, 0.59);
                        fapPso.Start();
                        
                        
                    }
					sendBenchmarks(model.GeneralInformation.ScenarioID, " Benchmarks Population "+ population);
                } 
               
            }
        }

        private void clean(String name) 
        {
            string[] files = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.csv");
            foreach(string f in files)
            {
                if (f.Contains(name))
                {
                    continue;
                }
                else
                {
                    System.IO.File.Delete(f);
                }
            }
        }

		private void sendBenchmarks(String scenarioId, String subject)
        {
            try
            {
                string[] files = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.csv");
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("william.bezuidenhout@gmail.com");
                mail.To.Add("william.bezuidenhout@gmail.com");
                mail.Subject = System.Environment.MachineName + " --- " +subject;
                mail.Body = "Benchmarks in CSV format";

                foreach (string csv in files)
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(csv);
                    mail.Attachments.Add(attachment);
                }
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("william.bezuidenhout@gmail.com", "I am w1ll14m b3zu!d3nh0ut");
                SmtpServer.EnableSsl = true;
                Console.WriteLine("Mailing {0} benchmarks to william.bezuidenhout@gmail.com", files.Length);
                SmtpServer.Send(mail);
                Console.WriteLine("Mail successfully sent!");
				clean(scenarioId);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed sending benchmarks. Got error: \n {0}", ex.Message);
            }
            
        }
    }
}
