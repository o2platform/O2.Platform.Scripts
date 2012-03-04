// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using ActiveUp.Net.Mail;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
//O2Ref:ActiveUp.Net.Smtp.dll
//O2Ref:ActiveUp.Net.Dns.dll
//O2Ref:ActiveUp.Net.Common.dll

namespace O2.XRules.Database.APIs
{
    public class API_ActiveUp_SendEmail
    {    	
    	public Dictionary<string,string> To { get; set; }//syntax: (email, name)
    	public string From_Name { get; set; }
    	public string From_Email { get; set; }
    	public string Subject { get; set; }
    	public string Body { get; set; }
    	public string Body_SpoofEmailAlertFooter { get; set; }
    	public ActiveUp.Net.Mail.Message _message { get; set; }
    	
    	public API_ActiveUp_SendEmail()
    	{
    		To = new Dictionary<string,string>();    		    		
    		
    		Body_SpoofEmailAlertFooter = "NOTE: this is a spoofed email, i.e. this was not sent by the current contact show in the To field.".line() + 
				    					 "      this email was sent using an O2 Platform (http://o2platform.com) script that is designed to show".line() + 
				    					 "      how easy it is to send spoofed emails ";
    	}
    	
    	public ActiveUp.Net.Mail.Message buildMessageObject()
    	{
    		_message = new ActiveUp.Net.Mail.Message();
			_message.From = new Address(From_Email,From_Name);
			foreach(var item in To)    		
				_message.To.Add(new Address(item.Key,item.Value)); //syntax: (email, name)
	
			_message.Subject = Subject;
			_message.BodyText.Text = Body.line().line() + Body_SpoofEmailAlertFooter;
			return _message;
    	}
    	
    	public bool sendEmail()
    	{
    		return sendEmail(true);
    	}
    	
    	public bool sendEmail(bool sendEmail)
    	{    	
    		"In send email".info();
    		try
    		{    	
    			buildMessageObject();
				"about to send message".info();
				if (sendEmail)
					SmtpClient.DirectSend(_message);
				"message sent".info();
				return true;
			}
			catch(Exception ex)
			{
				ex.log();
				return false;
			}
    	}
    	
    	public bool sendEmail(string fromEmail, string fromName, string toEmail, string toName, string subject ,string body)
    	{
    		return sendEmail(fromEmail, fromName, toEmail, toName, subject, body, true);
    	}
    	
    	public bool sendEmail(string fromEmail, string fromName, string toEmail, string toName, string subject ,string body, bool sendIt)
    	{
    		this.From_Email = fromEmail;
			this.From_Name = fromName;
			this.To.add(toEmail, toName);
			this.Subject = subject;
			this.Body = body;
			return  this.sendEmail(sendIt);
    	}
    }
}
