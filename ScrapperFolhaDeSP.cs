using System;
using HtmlAgilityPack;

namespace WebScrapper
{
    class ScrapperFolhaDeSP : Scrapper
    {
        private Results FResults;
        private string FURL;
        private Log FLog;
        public ScrapperFolhaDeSP(Results vResults)
        {
            Page = new HtmlWeb();
            BaseURL = "https://search.folha.uol.com.br/search?q=KEYWORD&periodo=personalizado&sd=" + FormatSeachDate() + "&site=todos"; //Base URL Search
            FResults = vResults;
            FLog = new Log();
        }

        private string FormatSeachDate()
        {
            DateTime thisDay = DateTime.Today;
            string Today = thisDay.ToString("d");
            string PastDate = thisDay.AddDays(-2).ToString("d");
            string FormattedDate = PastDate + "&ed=" + Today;

            return FormattedDate.Replace("/", "%2F");

        }
        public override void search(string vKeyword)
        {
            Console.WriteLine("Iniciando busca Folha de SP");

            try
            {
                string vWord = BaseURL.Replace("KEYWORD", prepareKeyword(vKeyword));
                Document = Page.Load(vWord);
                Console.WriteLine("Selecionando resultados");
                var vDivNodes = Document.DocumentNode.SelectNodes("//div[@class='c-headline__content']");
                if (vDivNodes != null) searchListResults(vDivNodes, vKeyword);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro -> {0}", e.Message);
                FLog.writeLog("Erro na palavra chave " + vKeyword + " na folha de SP");
                FLog.writeLog("Descrição do erro: " + e.Message);
                FLog.writeLog(" ");
            }
        }

        private string prepareKeyword(string vKeyword)
        {
            //Since OGlobo search links comes with letter '+' between words, the program needs to substitute any
            //space characters for '+'
            return vKeyword.Replace(' ', '+');
        }

        private void searchListResults(HtmlNodeCollection DivNodes, string Keyword)
        {
            foreach (var vDivNode in DivNodes)
            {
                searchLinkAndDate(vDivNode);
            }
        }

        private void searchLinkAndDate(HtmlNode vNode)
        {
            var vChildNodes = vNode.ChildNodes;

            if (vChildNodes != null)
            {
                foreach (var Node in vChildNodes)
                {
                    if (Node.GetAttributeValue("href", "") != "")
                    {
                        var vURL = Node.GetAttributeValue("href", string.Empty);
                        FResults.writeLink(vURL);
                    }

                    if (vNode.GetAttributeValue("itemprop", "") == "datePublished")
                    {
                        FResults.writeDate(vNode.GetAttributeValue("datetime", ""));
                    }

                    searchLinkAndDate(Node);
                }
            }
        }
    }
}
