using System;
using HtmlAgilityPack;


namespace WebScrapper
{
    class ScrapperValorEconomico : Scrapper
    {
        private Results FResults;
        private string FURL;
        private Log FLog;
        public ScrapperValorEconomico(Results vResults)
        {
            Page = new HtmlWeb();
            BaseURL = "https://valor.globo.com/busca/?q=KEYWORD&order=recent&from=" + FormatSeachDate(); //Base URL Search
            FResults = vResults;
            FLog = new Log();
        }


        private string FormatSeachDate()
        {
            DateTime thisDay = DateTime.Today;
            string Today = thisDay.ToString("s").Substring(0, 10) + "T23%3A59%3A59-0300";
            string PastDate = thisDay.AddDays(-2).ToString("s") + "-0300";
            string FormattedDate = PastDate + "&to=" + Today;

            return FormattedDate.Replace(":", "%3A");

        }
        public override void search(string vKeyword)
        {
            Console.WriteLine("Iniciando busca Valor Econômico");

            try
            {
                string vWord = BaseURL.Replace("KEYWORD", prepareKeyword(vKeyword));
                Document = Page.Load(vWord);
                Console.WriteLine("Selecionando resultados");
                var vLiNodes = Document.DocumentNode.SelectNodes("//li");
                if (vLiNodes != null) searchListResults(vLiNodes, vKeyword);
            }
            catch (Exception e)
            {                
                Console.WriteLine("Erro -> {0}", e.Message);
                FLog.writeLog("Erro na palavra chave " + vKeyword + " no Valor econômico");
                FLog.writeLog("Descrição do erro: " + e.Message);
            }
        }

        private string prepareKeyword(string vKeyword)
        {
            //Since OGlobo search links comes with letter '+' between words, the program needs to substitute any
            //space characters for '+'
            return vKeyword.Replace(' ', '+');
        }

        private void searchListResults(HtmlNodeCollection LiNodes, string Keyword)
        {
            foreach (var vLiNode in LiNodes)
            {
                searchLinkAndDate(vLiNode);
            }
        }

        private void searchLinkAndDate(HtmlNode vNode)
        {
            var vChildNodes = vNode.ChildNodes;

            if (vChildNodes != null)
            {
                foreach (var Node in vChildNodes)
                {
                    if (Node.GetAttributeValue("href", "") != "" && Node.GetAttributeValue("class", "") == "")
                    {
                        var vURL = Node.GetAttributeValue("href", string.Empty);                        
                        FResults.writeLink(extractDestinyURL(vURL));
                    }

                    if (vNode.GetAttributeValue("class", "") == "widget--info__meta")
                    {
                        FResults.writeDate(vNode.InnerText.Trim());
                    }

                    searchLinkAndDate(Node);
                }
            }
        }

        private string extractDestinyURL(string vURL)
        {
            if (vURL == "") { return vURL; }
            int vURlStart;
            int vUrlEnd;


            //The destiny URL is inside a redirection URL so the program extracts it based on their link pattern
            vURlStart = vURL.IndexOf("https");
            vUrlEnd = vURL.IndexOf("&abExperiment");

            if (vURlStart <= 0 || vUrlEnd <= 0) return "https:" + vURL;


            //Characters like : and / are represented by %3A and %2F inside the desnity URL, so the program substitutes them
            //for the right characters
            vURL = vURL.Substring(vURlStart, vUrlEnd - vURlStart);
            vURL = vURL.Replace("%3A", ":");
            vURL = vURL.Replace("%2F", "/");

            FURL = vURL;
            return vURL;
        }

    }
}
