var http = require('http');

var handleRequest = function(req,res)
	{
		console.log("Handling request for: " + req.url);
		res.writeHead(200, { 'Content-Type' : 'text/html'});		
		res.end('Hello From O2');
	};
	
var server = http.createServer(handleRequest);
server.listen(8125,"127.0.0.1");
console.log(server);
console.log("Browse to http://127.0.0.1:8125");
