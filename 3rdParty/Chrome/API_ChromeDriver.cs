using System.Linq;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
//Installer:Selenium_Installer.cs!Selenium\net40\WebDriver.dll
//O2File:SeleniumWebDrivers_Setup.cs
namespace O2.XRules.Database.APIs
{
	public class API_ChromeDriver
	{
	
	}
			
	public static class API_ChromeDriver_ExtensionMethods
	{
		public static ChromeDriver open(this ChromeDriver chromeDriver, string url)
		{
			chromeDriver.Navigate().GoToUrl(url);
			return chromeDriver;
		}
		
		public static List<IWebElement> elements(this ChromeDriver chromeDriver)
		{
			return (from element in chromeDriver.FindElements(By.XPath("//*"))
					select element).toList();
		}
		public static List<string> ids(this List<IWebElement> elements)
		{
			return (from element in elements
					select element.GetAttribute("Id")).Distinct().toList().removeEmpty();
		}
		public static List<string> tagNames(this List<IWebElement> elements)
		{
			return (from element in elements
					select element.TagName).Distinct().toList();
		}
	}
}