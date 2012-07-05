using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;


//DCz: this was converted from the VB file: WolframAlphaEngine.vb
//Author:
//   Full name: Mathias Lykkegaard Lorenzen
//   Nickname: Mathy
//   Email: mathias.lorenzen@flamefusion.net
//   Website: http://fsearch.us

public class WolframAlphaAssumption
{

	private string WA_Word;

	private List<string> WA_Categories = new List<string>();
	public string Word {
		get { return WA_Word; }
		set { WA_Word = value; }
	}

	public List<string> Categories {
		get { return WA_Categories; }
		set { WA_Categories = value; }
	}

}

public class WolframAlphaQuery 
{

	//Private Const MainRoot As String = "http://api.wolframalpha.com/v1/query.jsp"


	private string WA_APIKey;
	public string APIKey {
		get { return WA_APIKey; }
		set { WA_APIKey = value; }
	}

	private string WA_Format;
	private string WA_Substitution;
	private string WA_Assumption;
	private string WA_Query;
	private string WA_PodTitle;
	private int WA_TimeLimit;
	private bool WA_AllowCached;
	private bool WA_Asynchronous;

	private bool WA_MoreOutput;
	public bool MoreOutput {
		get { return WA_MoreOutput; }
		set { WA_MoreOutput = value; }
	}

	public string Format {
		get { return WA_Format; }
		set { WA_Format = value; }
	}

	public bool Asynchronous {
		get { return WA_Asynchronous; }
		set { WA_Asynchronous = value; }
	}

	public bool AllowCaching {
		get { return WA_AllowCached; }
		set { WA_AllowCached = false; }
	}

	public string Query {
		get { return WA_Query; }
		set { WA_Query = value; }
	}

	public int TimeLimit {
		get { return WA_TimeLimit; }
		set { WA_TimeLimit = value; }
	}

	public void AddPodTitle(string PodTitle, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(false)]  // ERROR: Optional parameters aren't supported in C#
bool CheckForDuplicates)
	{
		if (CheckForDuplicates == true && WA_PodTitle.Contains("&PodTitle=" + HttpUtility.UrlEncode(PodTitle))) {
			return;
		}
		WA_PodTitle += "&podtitle=" + HttpUtility.UrlEncode(PodTitle);
	}

	public void AddSubstitution(string Substitution, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(false)]  // ERROR: Optional parameters aren't supported in C#
bool CheckForDuplicates)
	{
		if (CheckForDuplicates == true && WA_Substitution.Contains("&substitution=" + HttpUtility.UrlEncode(Substitution))) {
			return;
		}
		WA_Substitution += "&substitution=" + HttpUtility.UrlEncode(Substitution);
	}

	public void AddAssumption(string Assumption, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(false)]  // ERROR: Optional parameters aren't supported in C#
bool CheckForDuplicates)
	{
		if (CheckForDuplicates == true && WA_Assumption.Contains("&substitution=" + HttpUtility.UrlEncode(Assumption))) {
			return;
		}
		WA_Assumption += "&assumption=" + HttpUtility.UrlEncode(Assumption);
	}

	public void AddAssumption(WolframAlphaAssumption Assumption, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(false)]  // ERROR: Optional parameters aren't supported in C#
bool CheckForDuplicates)
	{
		if (CheckForDuplicates == true && WA_Assumption.Contains("&substitution=" + HttpUtility.UrlEncode(Assumption.Word))) {
			return;
		}
		WA_Assumption += "&assumption=" + HttpUtility.UrlEncode(Assumption.Word);
	}

	public string[] Substitutions {
		get { return WA_Substitution.Split(new string[] { "&substitution=" }, StringSplitOptions.RemoveEmptyEntries); }
	}

	public string[] Assumptions {
		get { return WA_Assumption.Split(new string[] { "&assumption=" }, StringSplitOptions.RemoveEmptyEntries); }
	}

	public string[] PodTitles {
		get { return WA_PodTitle.Split(new string[] { "&assumption=" }, StringSplitOptions.RemoveEmptyEntries); }
	}

	public string FullQueryString {
		get { return "?appid=" + WA_APIKey + "&moreoutput=" + MoreOutput + "&timelimit=" + TimeLimit + "&format=" + WA_Format + "&input=" + WA_Query + WA_Assumption + WA_Substitution; }
	}

	public class WolframAlphaQueryFormat
	{
		public static string Image = "image";
		public static string HTML = "html";
		public static string PDF = "pdf";
		public static string PlainText = "plaintext";
		public static string MathematicaInput = "minput";
		public static string MathematicaOutput = "moutput";
		public static string MathematicaMathMarkupLanguage = "mathml";
		public static string MathematicaExpressionMarkupLanguage = "expressionml";
		public static string ExtensibleMarkupLanguage = "xml";
	}

}

public class WolframAlphaValidationResult
{

	private string WA_ParseData;
	private List<WolframAlphaAssumption> WA_Assumptions;
	private bool WA_Success;
	private bool WA_Error;

	private double WA_Timing;
	public bool Success {
		get { return WA_Success; }
		set { WA_Success = value; }
	}

	public string ParseData {
		get { return WA_ParseData; }
		set { WA_ParseData = value; }
	}

	public List<WolframAlphaAssumption> Assumptions {
		get { return WA_Assumptions; }
		set { WA_Assumptions = value; }
	}

	public bool ErrorOccured {
		get { return WA_Error; }
		set { WA_Error = value; }
	}

	public double Timing {
		get { return WA_Timing; }
		set { WA_Timing = value; }
	}

}

public class WolframAlphaEngine
{


	private string WA_APIKey;
	
	private WolframAlphaQueryResult WA_QueryResult;

	private WolframAlphaValidationResult WA_ValidationResult;
	
	//public WolframAlphaEngine(string APIKey)
	public WolframAlphaEngine()
	{
		//WA_APIKey = APIKey;
	}

	public string APIKey {
		get { return WA_APIKey; }
		set { WA_APIKey = value; }
	}

	public WolframAlphaQueryResult QueryResult {
		get { return WA_QueryResult; }
	}

	public WolframAlphaValidationResult ValidationResult {
		get { return WA_ValidationResult; }
	}



	public WolframAlphaValidationResult ValidateQuery(WolframAlphaQuery Query)
	{
		if (Query.APIKey == "") {
			if (this.APIKey == "") {
				throw new Exception("To use the Wolfram Alpha API, you must specify an API key either through the parsed WolframAlphaQuery, or on the WolframAlphaEngine itself.");
			}
			Query.APIKey = this.APIKey;
		}

		if (Query.Asynchronous == true && Query.Format == WolframAlphaQuery.WolframAlphaQueryFormat.HTML) {
			throw new Exception("Wolfram Alpha does not allow asynchronous operations while the format for the query is not set to \"HTML\".");
		}		
		System.Net.HttpWebRequest WebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://preview.wolframalpha.com/api/v1/validatequery.jsp" + Query.FullQueryString);
		WebRequest.KeepAlive = true;
		string Response = new StreamReader(WebRequest.GetResponse().GetResponseStream()).ReadToEnd();

		return ValidateQuery(Response);

	}


	public WolframAlphaValidationResult ValidateQuery(string Response)
	{
		XmlDocument Document = new XmlDocument();
		WolframAlphaValidationResult Result = null;
		try {
			Document.LoadXml(Response);
			Result = ValidateQuery(Document); 
		} catch {
		}
		Document = null;

		return Result;

	}


	public WolframAlphaValidationResult ValidateQuery(XmlDocument Response)
	{
		System.Threading.Thread.Sleep(1);

		XmlNode MainNode = Response.SelectNodes("/validatequeryresult")[0];

		WA_ValidationResult = new WolframAlphaValidationResult();		
		WA_ValidationResult.Success = MainNode.Attributes["success"].Value.toBool();
		WA_ValidationResult.ErrorOccured = MainNode.Attributes["error"].Value.toBool();
		WA_ValidationResult.Timing = MainNode.Attributes["timing"].Value.toDouble();		
		WA_ValidationResult.ParseData = MainNode.SelectNodes("parsedata")[0].InnerText;
		WA_ValidationResult.Assumptions = new List<WolframAlphaAssumption>();


		foreach (XmlNode Node in MainNode.SelectNodes("assumptions")) {
			System.Threading.Thread.Sleep(1);

			WolframAlphaAssumption Assumption = new WolframAlphaAssumption();

			Assumption.Word = Node.SelectNodes("word")[0].InnerText;

			XmlNode SubNode = Node.SelectNodes("categories")[0];


			foreach (XmlNode ContentNode in SubNode.SelectNodes("category")) {
				System.Threading.Thread.Sleep(1);

				Assumption.Categories.Add(ContentNode.InnerText);

			}

			WA_ValidationResult.Assumptions.Add(Assumption);

		}

		return WA_ValidationResult;

	}



	public WolframAlphaQueryResult LoadResponse(WolframAlphaQuery Query)
	{
		if (Query.APIKey == "") {
			if (this.APIKey == "") {
				throw new Exception("To use the Wolfram Alpha API, you must specify an API key either through the parsed WolframAlphaQuery, or on the WolframAlphaEngine itself.");
			}
			Query.APIKey = this.APIKey;
		}

		if (Query.Asynchronous == true && Query.Format == WolframAlphaQuery.WolframAlphaQueryFormat.HTML) {
			throw new Exception("Wolfram Alpha does not allow asynchronous operations while the format for the query is not set to \"HTML\".");
		}
		
		var serverAPIv2 = "http://api.wolframalpha.com/v2/query";
		var WebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(serverAPIv2 + Query.FullQueryString);
		//System.Net.HttpWebRequest WebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://preview.wolframalpha.com/api/v1/query.jsp" + Query.FullQueryString);
		WebRequest.KeepAlive = true;
		string Response = new StreamReader(WebRequest.GetResponse().GetResponseStream()).ReadToEnd();

		return LoadResponse(Response);

	}

	public WolframAlphaQueryResult LoadResponse(string Response)
	{
		XmlDocument Document = new XmlDocument();
		WolframAlphaQueryResult Result = null;
		try {
			Document.LoadXml(Response);
			Result = LoadResponse(Document);
		} catch {
		}
		Document = null;

		return Result;

	}

	public WolframAlphaQueryResult LoadResponse(XmlDocument Response)
	{
		System.Threading.Thread.Sleep(1);

		XmlNode MainNode = Response.SelectNodes("/queryresult")[0];
		WA_QueryResult = new WolframAlphaQueryResult();
		WA_QueryResult.Success = MainNode.Attributes["success"].Value.toBool();
		WA_QueryResult.ErrorOccured = MainNode.Attributes["error"].Value.toBool();
		WA_QueryResult.NumberOfPods = MainNode.Attributes["numpods"].Value.toInt();
		WA_QueryResult.Timing = MainNode.Attributes["timing"].Value.toDouble();
		WA_QueryResult.TimedOut = MainNode.Attributes["timedout"].Value;
		WA_QueryResult.DataTypes = MainNode.Attributes["datatypes"].Value;
		WA_QueryResult.Pods = new List<WolframAlphaPod>();


		foreach (XmlNode Node in MainNode.SelectNodes("pod")) {
			System.Threading.Thread.Sleep(1);

			WolframAlphaPod Pod = new WolframAlphaPod();

			Pod.Title = Node.Attributes["title"].Value;
			Pod.Scanner = Node.Attributes["scanner"].Value;
			Pod.Position = Node.Attributes["position"].Value.toInt();
			Pod.ErrorOccured = Node.Attributes["error"].Value.toBool();
			Pod.NumberOfSubPods = Node.Attributes["numsubpods"].Value.toInt();
			Pod.SubPods = new List<WolframAlphaSubPod>();


			foreach (XmlNode SubNode in Node.SelectNodes("subpod")) {
				System.Threading.Thread.Sleep(1);

				WolframAlphaSubPod SubPod = new WolframAlphaSubPod();
				SubPod.Title = SubNode.Attributes["title"].Value;


				foreach (XmlNode ContentNode in SubNode.SelectNodes("plaintext")) {
					System.Threading.Thread.Sleep(1);

					SubPod.PodText = ContentNode.InnerText;

				}


				foreach (XmlNode ContentNode in SubNode.SelectNodes("img")) {
					System.Threading.Thread.Sleep(1);

					WolframAlphaImage Image = new WolframAlphaImage();
					Image.Location = new Uri(ContentNode.Attributes["src"].Value);
					Image.HoverText = ContentNode.Attributes["alt"].Value;
					Image.Title = ContentNode.Attributes["title"].Value;
					Image.Width = ContentNode.Attributes["width"].Value.toInt();
					Image.Height = ContentNode.Attributes["height"].Value.toInt();
					SubPod.PodImage = Image;

				}

				Pod.SubPods.Add(SubPod);

			}

			WA_QueryResult.Pods.Add(Pod);

		}

		return WA_QueryResult;

	}

	

}

public class WolframAlphaQueryResult
{

	private List<WolframAlphaPod> WA_Pods;
	private bool WA_Success;
	private bool WA_Error;
	private int WA_NumberOfPods;
	private string WA_DataTypes;
	private string WA_TimedOut;
	private double WA_Timing;

	private double WA_ParseTiming;
	public List<WolframAlphaPod> Pods {
		get { return WA_Pods; }
		set { WA_Pods = value; }
	}

	public bool Success {
		get { return WA_Success; }
		set { WA_Success = value; }
	}

	public bool ErrorOccured {
		get { return WA_Error; }
		set { WA_Error = value; }
	}

	public int NumberOfPods {
		get { return WA_NumberOfPods; }
		set { WA_NumberOfPods = value; }
	}

	public string DataTypes {
		get { return WA_DataTypes; }
		set { WA_DataTypes = value; }
	}

	public string TimedOut {
		get { return WA_TimedOut; }
		set { WA_TimedOut = value; }
	}

	public double Timing {
		get { return WA_Timing; }
		set { WA_Timing = value; }
	}

	public double ParseTiming {
		get { return WA_ParseTiming; }
		set { WA_ParseTiming = value; }
	}

}

public class WolframAlphaPod
{

	private List<WolframAlphaSubPod> WA_SubPods;
	private string WA_Title;
	private string WA_Scanner;
	private int WA_Position;
	private bool WA_Error;

	private int WA_NumberOfSubPods;
	public List<WolframAlphaSubPod> SubPods {
		get { return WA_SubPods; }
		set { WA_SubPods = value; }
	}

	public string Title {
		get { return WA_Title; }
		set { WA_Title = value; }
	}

	public string Scanner {
		get { return WA_Scanner; }
		set { WA_Scanner = value; }
	}

	public int Position {
		get { return WA_Position; }
		set { WA_Position = value; }
	}

	public bool ErrorOccured {
		get { return WA_Error; }
		set { WA_Error = value; }
	}

	public int NumberOfSubPods {
		get { return WA_NumberOfSubPods; }
		set { WA_NumberOfSubPods = value; }
	}

}

public class WolframAlphaSubPod
{

	private string WA_Title;
	private string WA_PodText;

	private WolframAlphaImage WA_PodImage;
	public string Title {
		get { return WA_Title; }
		set { WA_Title = value; }
	}

	public string PodText {
		get { return WA_PodText; }
		set { WA_PodText = value; }
	}

	public WolframAlphaImage PodImage {
		get { return WA_PodImage; }
		set { WA_PodImage = value; }
	}

}

public class WolframAlphaImage
{

	private Uri WA_Location;
	private int WA_Width;
	private int WA_Height;
	private string WA_Title;

	private string WA_HoverText;
	public Uri Location {
		get { return WA_Location; }
		set { WA_Location = value; }
	}

	public int Width {
		get { return WA_Width; }
		set { WA_Width = value; }
	}

	public int Height {
		get { return WA_Height; }
		set { WA_Height = value; }
	}

	public string Title {
		get { return WA_Title; }
		set { WA_Title = value; }
	}

	public string HoverText {
		get { return WA_HoverText; }
		set { WA_HoverText = value; }
	}

}
