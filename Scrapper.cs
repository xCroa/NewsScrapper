using HtmlAgilityPack;



namespace WebScrapper
{
    class Scrapper
    {
        private HtmlWeb FPage;
        private string FUrl;
        private HtmlDocument FDocument;
        private string FKeywords;
        private string FBaseURL;        

        public HtmlWeb Page { get => FPage; set => FPage = value; }
        public HtmlDocument Document { get => FDocument; set => FDocument = value; }        
        public string BaseURL { get => FBaseURL; set => FBaseURL = value; }

        public virtual void search(string vKeyword) { }
            

       /* public void testar(string vText)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "aeho";
            PdfPage page = pdf.AddPage();
            

            XGraphics gfx = XGraphics.FromPdfPage(page);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

            gfx.DrawString(vText, font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

            const string filename = "HelloWorld.pdf";
            pdf.Save(filename);
        }

    */
    }
}
