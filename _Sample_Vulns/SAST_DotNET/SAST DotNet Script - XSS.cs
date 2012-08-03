//Smallest XSS Example in world :)
using System.Web;

var request = HttpContext.Current.Request;
var response = HttpContext.Current.Response;

response.Write(request["UserName"]);
