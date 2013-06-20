using System;
using System.Linq;
using System.Collections.Generic;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
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
	}
}