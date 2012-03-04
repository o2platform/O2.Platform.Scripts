using HacmeBank_v2_Website.dataClasses;
using System.Web.Services.Protocols;
using System;
namespace HacmeBank_v2_Website.ascx
{
	public class PostMessageForm : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox txtText;
		protected System.Web.UI.WebControls.Label lblErrorMessage;
		protected System.Web.UI.WebControls.TextBox txtSubject;
		protected System.Web.UI.WebControls.Label lblPostedMessages;
		protected void btnPostMessage_Click(object sender, System.EventArgs e)
		{
			if ("" == txtSubject.Text)
				lblErrorMessage.Text = "Error : You have to enter a Message Subject<br/>";
			if ("" == txtText.Text)
				lblErrorMessage.Text += "Error : You have to enter a Message Text<br/>";
			if ("" == lblErrorMessage.Text) {
				string userId = (string)Session["userID"].ToString();
				string messageSubject = txtSubject.Text;
				string messageText = txtText.Text;
				Global.objUsersCommunity.PostMessage("", userId, messageSubject, messageText);
			}
			LoadPostedMessages();
		}
		private void LoadPostedMessages()
		{
			lblPostedMessages.Text = "";
			object[] allPostedMessages = Global.objUsersCommunity.GetPostedMessages("");
			dataClasses.postedMessage[] postedMessages = new HacmeBank_v2_Website.dataClasses.postedMessage[allPostedMessages.Length];
			for (int i = 0; i < allPostedMessages.Length; i++) {
				object[] postedMessage = (object[])allPostedMessages[i];
				postedMessages[i] = new dataClasses.postedMessage();
				postedMessages[i].messageID = (decimal)postedMessage[0];
				postedMessages[i].userID = (decimal)postedMessage[1];
				postedMessages[i].messageDate = (DateTime)postedMessage[2];
				postedMessages[i].messageSubject = (string)postedMessage[3];
				postedMessages[i].messageText = (string)postedMessage[4];
			}
			lblPostedMessages.Text = "<table width=\"100%\" border=0>";
			foreach (dataClasses.postedMessage objPostedMessage in postedMessages) {
				object[] userDetails = Global.objUserManagement.GetUserDetail_using_userID("", objPostedMessage.userID.ToString());
				string userName = userDetails[1].ToString();
				lblPostedMessages.Text += "<tr><td width=200 valign=top  bgcolor='#F5EFFF'>" + "\t<b>" + objPostedMessage.messageSubject + "\t</b><br/> (by: " + userName + "\ton: " + objPostedMessage.messageDate.ToShortDateString() + ")" + " </td>";
				lblPostedMessages.Text += "<td valign=top bgcolor='#F5EFFF'>";
				lblPostedMessages.Text += objPostedMessage.messageText;
				lblPostedMessages.Text += "</td></tr>";
				lblPostedMessages.Text += "<tr><td>    <br>    </td></tr>";
			}
		}
	}
}
public class WS_UserManagement : System.Web.Services.Protocols.SoapHttpClientProtocol
{
	[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserDetail_using_userID", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
	public object[] GetUserDetail_using_userID(string sessionID, string userID)
	{
		object[] results = this.Invoke("GetUserDetail_using_userID", new object[] {
			sessionID,
			userID
		});
		return ((object[])(results[0]));
	}
}
namespace System
{
	public class Object
	{
		public virtual string ToString()
		{
			throw new System.Exception("O2 Auto Generated Method");
		}
	}
	public class DateTime : System.ValueType, System.IComparable, System.IFormattable, System.IConvertible, System.Runtime.Serialization.ISerializable, System.IComparable, System.IEquatable
	{
		public string ToShortDateString()
		{
			throw new System.Exception("O2 Auto Generated Method");
		}
	}
	public class Decimal : System.ValueType, System.IFormattable, System.IComparable, System.IConvertible, System.IComparable, System.IEquatable
	{
		public virtual string ToString()
		{
			throw new System.Exception("O2 Auto Generated Method");
		}
	}
	public class Array : System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable
	{
		public int Length {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
namespace System.Web.Services.Protocols
{
	public class SoapHttpClientProtocol : System.Web.Services.Protocols.HttpWebClientProtocol, System.ComponentModel.IComponent, System.IDisposable
	{
		protected object[] Invoke(string methodName, object[] parameters)
		{
			throw new System.Exception("O2 Auto Generated Method");
		}
	}
	public class SoapParameterStyle : System.Enum, System.IComparable, System.IFormattable, System.IConvertible
	{
		public static const System.Web.Services.Protocols.SoapParameterStyle Wrapped;
	}
}
namespace HacmeBank_v2_Website.dataClasses
{
	public class postedMessage
	{
		public string messageText;
		public string messageSubject;
		public DateTime messageDate;
		public decimal userID;
		public decimal messageID;
		public postedMessage()
		{
			throw new System.Exception("O2 Auto Generated Method");
		}
	}
}
namespace HacmeBank_v2_Website
{
	public class Global : System.Web.HttpApplication
	{
		public static WS_UserManagement objUserManagement;
		public static WS_UsersCommunity objUsersCommunity;
	}
}
namespace System.Web.Services.Description
{
	public class SoapBindingUse : System.Enum, System.IComparable, System.IFormattable, System.IConvertible
	{
		public static const System.Web.Services.Description.SoapBindingUse Literal;
	}
}
namespace System.Web.UI
{
	public class UserControl : System.Web.UI.TemplateControl, System.ComponentModel.IComponent, System.IDisposable, System.Web.UI.IParserAccessor, System.Web.UI.IUrlResolutionService, System.Web.UI.IDataBindingsAccessor, System.Web.UI.IControlBuilderAccessor, System.Web.UI.IControlDesignerAccessor, System.Web.UI.IExpressionsAccessor, System.Web.UI.INamingContainer, System.Web.UI.IFilterResolutionService, System.Web.UI.IAttributeAccessor, System.Web.UI.INonBindingContainer, System.Web.UI.IUserControlDesignerAccessor
	{
		public System.Web.SessionState.HttpSessionState Session {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
