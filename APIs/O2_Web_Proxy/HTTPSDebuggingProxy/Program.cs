using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.Threading;
//using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
//using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:CacheEntry.cs
//O2File:CacheKey.cs
//O2File:ProxyCache.cs
//O2File:ProxyServer.cs
namespace HTTPProxyServer
{
    class Program
    {
        private static void PrintUsageInfo()
        {
            Console.WriteLine("matt-dot-net proxy server usage: ");
            Console.WriteLine("\t-? = this help");
            Console.WriteLine("\t-h = dump headers to standard output");
            Console.WriteLine("\t-p = dump post data to standard output");
            Console.WriteLine("\t-r = dump response data to standard output (WARNING: redirect your output to a file)");
            Console.WriteLine();
            Console.WriteLine("\tNote: Data dumping kills performance");
            Console.WriteLine();
            Console.WriteLine("\tDisclaimer: This proxy server is for testing and development purposes only.");
            Console.WriteLine("\t            Installing this server on a network without the knowledge of others is not");
            Console.WriteLine("\t            condoned by the author. This proxy server presents a security risk for");
            Console.WriteLine("\t            users who do not understand SSL certificates and browser security.");
            Console.WriteLine();
            Console.WriteLine("\tAuthor: Matt McKnight <matt_mcknight@live.com>");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args.Length <= 3)
                {
                    Regex argRegHelp = new Regex(@"^(/|-)\?$");
                    Regex argRexH = new Regex("^(/|-)h$");
                    Regex argRexP = new Regex("^(/|-)p$");
                    Regex argRexR = new Regex("^(/|-)r$");

                    foreach (String s in args)
                    {
                        if (argRexH.IsMatch(s.ToLower()))
                            ProxyServer.Server.DumpHeaders = true;
                        else if (argRexP.IsMatch(s.ToLower()))
                            ProxyServer.Server.DumpPostData = true;
                        else if (argRexR.IsMatch(s.ToLower()))
                            ProxyServer.Server.DumpResponseData = true;
                        else
                        {
                            PrintUsageInfo();
                            return;
                        }
                    }
                }
                else if (args.Length > 4) 
                {
                    PrintUsageInfo();
                    return;
                }
            }

            //if (ProxyServer.Server.Start())
            var cert = @"cert.cer".local();
            if (ProxyServer.Server.Start(cert))
            {
                Console.WriteLine(String.Format("Server started on {0}:{1}...Press enter key to end",ProxyServer.Server.ListeningIPInterface,ProxyServer.Server.ListeningPort));
                Console.ReadLine();
                Console.WriteLine("Shutting down");
                ProxyServer.Server.Stop();
                Console.WriteLine("Server stopped...");
            }
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }



}
