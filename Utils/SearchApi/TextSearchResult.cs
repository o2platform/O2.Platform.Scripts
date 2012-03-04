// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Text.RegularExpressions;

namespace O2.XRules.Database.Utils
{
    public class TextSearchResult
    {
        public int length ;
        public Regex RegEx_Used;
		public String File { get; set; }
        public int Line_Number { get; set; }
        public int Position { get; set; }         
        public String Match_Text { get; set; }
        public String Match_Line { get; set; }        

        public TextSearchResult(Regex rRegExUsed, String sMatchText, String sMatchLine, String sFile,
                                int iLineNumber, int iPosition, int iLength)
        {
            this.RegEx_Used = rRegExUsed;
            this.Match_Text = sMatchText;
            this.Match_Line = sMatchLine;
            this.File = sFile;
            this.Line_Number = iLineNumber;
            this.Position = iPosition;
            this.length = iLength;
        }
    }
}
