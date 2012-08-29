using System.IO;
using System.Web;
using System.Diagnostics;

var context = HttpContext.Current;
var request = context.Request;
var response = context.Response;

var taint = request["UserName"];

response.Write(taint);
response.Redirect(taint);
Process.Start(taint);
File.OpenRead(taint);


//O2Tag:SkipGlobalCompilation