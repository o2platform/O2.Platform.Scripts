var http = require('http');
var util = require('util');

var handleRequest = function(req,res)
	{
		console.log(req.url);
		res.writeHead(200, { 'Content-Type' : 'text/html'});
		res.write('<html><body>');
		res.write("<h2>Hello from NodeJS</h2>");
		res.write("<h3>Request object</h3><pre>" + util.inspect(req) + "</pre>");		
		res.write("<h3>Response object</h3><pre>" + util.inspect(res) + "</pre>");		
		res.end('</body></html>');
	};
var server = http.createServer(handleRequest);
console.log(server.listen(8125,"127.0.0.1"));
