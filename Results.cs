using System;
using OfficeOpenXml;
using System.IO;
using System.Drawing;

namespace WebScrapper
{   
    class Results
    {
        private string FFileName;
        private ExcelPackage FExcelFile;        
        private int FLine;
        private ExcelWorksheet FWS;


        public Results(string FileName)
        {
            FFileName = FileName;
            File.Delete(@FFileName);
            FExcelFile = new ExcelPackage(new FileInfo(@FFileName));            
            FLine = 1;                
            FWS = FExcelFile.Workbook.Worksheets.Add("Results");
        }

        public void writeString(string vText)
        {
            FLine++;
            FLine++;
            FWS.Cells["A" + FLine.ToString()].Value = vText;
            FLine++;
        }

        public void writeLink(string vLink)
        {
            FLine++;
            FWS.Cells["A" + FLine.ToString()].Style.Font.Color.SetColor(Color.Blue);
            FWS.Cells["A" + FLine.ToString()].Style.Font.UnderLine = true;
            try
            {
                FWS.Cells["A" + FLine.ToString()].Hyperlink = new Uri(vLink, UriKind.Absolute);
            } catch (Exception e)
            {

            }
         
        }

        public void writeDate(string vDate)
        {
            FWS.Cells["B" + FLine.ToString()].Value = vDate;
        }
        public void saveFile()
        {
            FExcelFile.SaveAs(new FileInfo(@FFileName));
            ((IDisposable)FExcelFile).Dispose();
        }
    }
}
