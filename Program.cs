using System;
using System.IO;
using System.Text;

namespace WebScrapper
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Results FResults = new Results(@"Resultados.xlsx");
            Log FLog = new Log();
            


            string[] vKeywords = File.ReadAllLines("Keywords.txt", Encoding.UTF7);

            if (vKeywords.Length > 0)
            {
                FLog.writeLog("");
                FLog.writeLog("");                
                FLog.writeLog("Buscas do dia " + DateTime.Today.ToString("d"));
                var SG = new ScrapperGlobo(FResults);
                var SVE = new ScrapperValorEconomico(FResults);
                var SE = new ScrapperEstadao(FResults);
                var SFSP = new ScrapperFolhaDeSP(FResults);                
                foreach (var vWord in vKeywords)
                {
                    Console.WriteLine("Buscando palavra chave " + vWord);
                    FResults.writeString(vWord);
                    SG.search(vWord);
                    SVE.search(vWord);
                    SE.search(vWord);
                    SFSP.search(vWord);                    
                }

                FResults.saveFile();
            }
        }
    }
}
