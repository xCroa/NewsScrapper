using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WebScrapper
{
    class Log
    {
        private string FFileName;
        public Log()
        {
            FFileName = "Log.txt";
        }

        public void writeLog(string vText)
        {
            using (StreamWriter LogFile = new StreamWriter(@FFileName, true))
            {                
                LogFile.WriteLine(vText);
            }
        }

    }
}
