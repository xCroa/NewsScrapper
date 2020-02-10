using System;
using HtmlAgilityPack;


namespace WebScrapper
{
    class ScrapperGlobo : Scrapper
    {
        private Results FResults;
        private Log FLog;
        public ScrapperGlobo(Results vResults)
        {            
            Page = new HtmlWeb();
            BaseURL = "https://oglobo.globo.com/busca/?q="; //Base URL Search
            FResults = vResults;
            FLog = new Log();
        }

        public override void search(string vKeyword)
        {
            Console.WriteLine("Iniciando busca OGlobo");
            
            try
            {                
                Document = Page.Load(BaseURL + prepareKeyword(vKeyword));                
                Console.WriteLine("Selecionando resultados");             
                var vLiNodes = Document.DocumentNode.SelectNodes("//li");                    
                if (vLiNodes != null) searchListResults(vLiNodes, vKeyword);
            } catch (Exception e)
            {
                Console.WriteLine("Erro -> {0}", e.Message);
                FLog.writeLog("Erro na palavra chave " + vKeyword + " no site O Globo");
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
                    if (Node.GetAttributeValue("href", "") != "" && Node.GetAttributeValue("title", "") != "")
                    {
                        var vURL = Node.GetAttributeValue("href", string.Empty);
                        FResults.writeLink(extractDestinyURL(vURL));
                    }

                    if (vNode.GetAttributeValue("class", "") == "tempo-decorrido" || vNode.GetAttributeValue("class", "") == "busca-tempo-decorrido")
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
            vUrlEnd = vURL.IndexOf("&amp;t");

            if (vURlStart <= 0 || vUrlEnd <= 0) return "https:" + vURL;


            //Characters like : and / are represented by %3A and %2F inside the desnity URL, so the program substitutes them
            //for the right characters
            vURL = vURL.Substring(vURlStart, vUrlEnd - vURlStart);
            vURL = vURL.Replace("%3A", ":");
            vURL = vURL.Replace("%2F", "/");

            return vURL;
        }

    }
}
