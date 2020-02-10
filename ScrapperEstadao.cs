using System;
using HtmlAgilityPack;

namespace WebScrapper
{
    class ScrapperEstadao : Scrapper
    {
        private Results FResults;
        private string FURL;
        private Log FLog;
        public ScrapperEstadao(Results vResults)
        {
            Page = new HtmlWeb();
            BaseURL = "https://busca.estadao.com.br/?tipo_conteudo=Todos&quando=" + FormatSeachDate() + "&q="; //Base URL Search
            FResults = vResults;
            FLog = new Log();
        }

        private string FormatSeachDate()
        {
            DateTime thisDay = DateTime.Today;
            string Today = thisDay.ToString("d");
            string PastDate = thisDay.AddDays(-2).ToString("d");
            string FormattedDate = PastDate + "-" + Today;

            return FormattedDate.Replace("/", "%2F");

        }
        public override void search(string vKeyword)
        {
            Console.WriteLine("Iniciando busca Estadão");

            try
            {
                Document = Page.Load(BaseURL + prepareKeyword(vKeyword));
                Console.WriteLine("Selecionando resultados");
                var vDivNodes = Document.DocumentNode.SelectNodes("//div[@class='box ']");
                if (vDivNodes != null) searchListResults(vDivNodes, vKeyword);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro -> {0}", e.Message);
                FLog.writeLog("Erro na palavra chave " + vKeyword + " no Estadão ");
                FLog.writeLog("Descrição do erro: " + e.Message);
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
                    if (Node.GetAttributeValue("href", "") != "" && Node.GetAttributeValue("class", "") == "link-title")
                    {
                        var vURL = Node.GetAttributeValue("href", string.Empty);
                        FResults.writeLink(vURL);
                    }

                    if (vNode.GetAttributeValue("class", "") == "data-posts")
                    {
                        FResults.writeDate(vNode.InnerText.Trim());
                    }

                    searchLinkAndDate(Node);
                }
            }
        }
    }
}
