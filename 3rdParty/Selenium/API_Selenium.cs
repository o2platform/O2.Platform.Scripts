// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
//Installer:Selenium_Installer.cs!Selenium\net40\WebDriver.dll
//O2File:SeleniumWebDrivers_Setup.cs

namespace O2.XRules.Database.APIs
{
    public class API_Selenium
    {    
		public IWebDriver WebDriver {get;set;}
		
    }
    public static class API_Selenium_ExtensionMethods
    {
    	public static API_Selenium setTarget_Chrome(this API_Selenium selenium)
    	{
    		selenium.WebDriver = "Selenium_ChromeDriver".o2Cache<IWebDriver>(SeleniumWebDrivers_Setup.ChromeDriverSetUp);
    		return selenium;    		
    	}
    	
    	public static API_Selenium setTarget_Firefox(this API_Selenium selenium)
    	{
    		selenium.WebDriver = "Selenium_FirefoxDriver".o2Cache<IWebDriver>(() => new FirefoxDriver());
    		return selenium;    		
    	}
    	
    	public static API_Selenium setTarget_IE(this API_Selenium selenium)
    	{
    		selenium.WebDriver = "Selenium_InternetExplorerDriver".o2Cache<IWebDriver>(SeleniumWebDrivers_Setup.IEDriverSetUp);
    		return selenium;    		
    	}
    }
}

