using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
    
namespace O2.XRules.Database.APIs
{    

	public interface IWebscarabConversation
    {
        string COOKIE { get; set; }
        List<string> CRLF_GET { get; set; }
        string id { get; set; }
        string METHOD { get; set; }
        string ORIGIN { get; set; }
        string request { get; set; }
        string response { get; set; }
        string RESPONSE_SIZE { get; set; }
        List<string> SET_COOKIE { get; set; }
        string STATUS { get; set; }
        string TAG { get; set; }
        string URL { get; set; }
        string WHEN { get; set; }
        List<string> XSS_GET { get; set; }
        List<string> XSS_POST { get; set; }
    }
    public class WebscarabConversation : IWebscarabConversation
    {
        public string COOKIE { get; set; }
        public List<string> CRLF_GET { get; set; }
        public string id { get; set; }
        public string METHOD { get; set; }
        public string ORIGIN { get; set; }
        public string request { get; set; }
        public string response { get; set; }
        public string RESPONSE_SIZE { get; set; }
        public List<string> SET_COOKIE { get; set; }
        public string STATUS { get; set; }
        public string TAG { get; set; }
        public string URL { get; set; }
        public string WHEN { get; set; }
        public List<string> XSS_GET { get; set; }
        public List<string> XSS_POST { get; set; }

        public WebscarabConversation()
        {
            this.XSS_GET = new List<string>();
            this.CRLF_GET = new List<string>();
            this.SET_COOKIE = new List<string>();
            this.XSS_POST = new List<string>();
        }
        
    }
}

