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
            BaseURL = "https://search.folha.uol.com.br/search?q=KEYWORD&periodo=semana&sd=&sd=&ed=&ed=&site=todos"; //Base URL Search
            FResults = vResults;
            FLog = new Log();
        }

        /* Since Folha broke the custom date search, we started to use the automatic filter
         private string FormatSeachDate()
        {
            DateTime thisDay = DateTime.Today;
            string Today = thisDay.ToString("d");
            string PastDate = thisDay.AddDays(-2).ToString("d");
            string FormattedDate = PastDate + "&sd=&ed=" + Today;

            return FormattedDate.Replace("/", "%2F");

        }
        */
        public override void search(string vKeyword)
        {
            Console.WriteLine("Iniciando busca Folha de SP");

            try
            {
                string vWord = BaseURL.Replace("KEYWORD", prepareKeyword(vKeyword));
                Document = Page.Load(vWord);
                Console.WriteLine("Selecionando resultados");

                /*When the search can't find the keyword, it offers related searchs and show us a message with related words 
                  wich is shown on a div with a "message info" class. Since related searchs are not interesting for us,
                  we skip the search
                */
                if (!hasMessage_info(Document))
                {
                    var vDivNodes = Document.DocumentNode.SelectNodes("//div[@class='c-headline__content']");
                    if (vDivNodes != null) searchListResults(vDivNodes, vKeyword);
                } else
                {
                    Console.WriteLine("Palavra chave não encontrada, pulando resultados relacionados");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro -> {0}", e.Message);
                FLog.writeLog("Erro na palavra chave " + vKeyword + " na folha de SP");
                FLog.writeLog("Descrição do erro: " + e.Message);
                FLog.writeLog(" ");
            }
        }

        private bool hasMessage_info(HtmlDocument document)
        {
            var vMessage_infoNode = document.DocumentNode.SelectSingleNode("//div[@class='message info']");

            if (vMessage_infoNode == null)
            {
                return false;
            }
            else
            {
                if (vMessage_infoNode.InnerText != "\n")
                {
                    return true;
                } else
                {
                    return false;
                }
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
