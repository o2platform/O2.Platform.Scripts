// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.DotNetWrappers.ExtensionMethods;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium;

//O2Ref:Selenium\net40\WebDriver.dll

//O2File:API_Selenium.cs

namespace O2.XRules.Database.APIs
{
    public class SeleniumWebDrivers_Setup
    {
        //Defining Chrome and IE WebDrivers downloads. For IE there might be required either x32 or  x64 installers
        public static string ChromeDriverDownloadLink	{ get; set; }
		public static string IEDriverDownloadLinkx32	{ get; set; }
		public static string IEDriverDownloadLinkx64	{ get; set; }
		public static string DriversFolder				{ get; set; }	

	    static SeleniumWebDrivers_Setup()
	    {
			ChromeDriverDownloadLink	= @"http://chromedriver.googlecode.com/files/chromedriver_win_26.0.1383.0.zip";
			IEDriverDownloadLinkx32		= @"http://selenium.googlecode.com/files/IEDriverServer_Win32_2.33.0.zip";
			IEDriverDownloadLinkx64		= @"https://selenium.googlecode.com/files/IEDriverServer_x64_2.33.0.zip";
			// we need to think a bit more on where we put this temp folder
			// will create folder in the root of the O2 Temp (which is inside the user's temp folder)
			DriversFolder = @"..\_TM_UnitTests".tempDir(false); 
            //DriversFolder = @"C:\temp";   // this not good, specially since a normal user account should not be able to write there
		    //DriversFolder.startProcess(); // use this to open the temp folder in windows explorer
	    }

	    public static ChromeDriver ChromeDriverSetUp()
        {
            Action ensureDriverExists =() =>
                {
                    const string chromeDriverExe = "chromedriver.exe";
					var fullPath = DriversFolder.pathCombine(chromeDriverExe);
                    if (fullPath.fileExists().isFalse())
                    {
                       var downloadedZipFile = ChromeDriverDownloadLink.download();
					   downloadedZipFile.unzip_File(DriversFolder);
                    }
                };
            Action configureEnvironmentPath =() =>
                {
                    var currentEnvironmentPath = Environment.GetEnvironmentVariable("Path");

					if (currentEnvironmentPath.contains(DriversFolder).isFalse())
                    {
						var newEnvironmentPath = "{0};{1}".info(currentEnvironmentPath, DriversFolder);
                        "Setting Environment Path to: {0}".format(newEnvironmentPath);
                        Environment.SetEnvironmentVariable("Path", newEnvironmentPath);
                    }
                    else
						"Environment Path already had: {0}".info(DriversFolder);
                };
			ensureDriverExists();
            configureEnvironmentPath();
			return new ChromeDriver();
        }

		public static InternetExplorerDriver IEDriverSetUp()
        {
            
            Action ensureDriverExists =
                () =>
                {
                    const string ieDriverExe = "IEDriverServer.exe.exe";
					var fullPath = DriversFolder.pathCombine(ieDriverExe);
                    var is64BitOperatingSystem = Environment.Is64BitOperatingSystem;

                    if (fullPath.fileExists().isFalse())
                    {
						var downloadUrl = is64BitOperatingSystem ? IEDriverDownloadLinkx64 : IEDriverDownloadLinkx32;

                        var downloadedZipFile = downloadUrl.download();
						downloadedZipFile.unzip_File(DriversFolder);
                    }
                };
            Action configureEnvironmentPath =
                () =>
                {
                    var currentEnvironmentPath = Environment.GetEnvironmentVariable("Path");

					if (currentEnvironmentPath.contains(DriversFolder).isFalse())
                    {
						var newEnvironmentPath = "{0};{1}".info(currentEnvironmentPath, DriversFolder);
                        "Setting Environment Path to: {0}".format(newEnvironmentPath);
                        Environment.SetEnvironmentVariable("Path", newEnvironmentPath);
                    }
                    else
						"Environment Path already had: {0}".info(DriversFolder);
                };
			ensureDriverExists();
            configureEnvironmentPath();
		    var IEDriver = new InternetExplorerDriver();
		    IEDriver.Manage().Cookies.DeleteAllCookies();
			return  IEDriver;
        }
    }
}
