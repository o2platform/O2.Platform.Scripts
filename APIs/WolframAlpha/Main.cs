	using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;

//O2File:WolframAlphaEngine.cs

public class WolframAlphaWrapperExample
{
	

	WolframAlphaEngine Engine;
	
	public void Output(string Data, int Indenting, System.ConsoleColor Color)
	{
		Data = new string(' ', Indenting * 4) + Data;

		Console.ForegroundColor = Color;
		Console.WriteLine(Data);
		Console.ForegroundColor = ConsoleColor.White;

		/*StreamWriter Writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Wolfram Alpha wrapper log.log", true);
		Writer.WriteLine(Data);
		Writer.Close(); 
		Writer.Dispose();*/

	}


	public void Main(string apiKey, string WolframAlphaSearchTerms)
	{
		Engine = new WolframAlphaEngine();
		
		//Try to delete the log file if it already exists.
		/*try {
			File.Delete(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Wolfram Alpha wrapper log.log");
		} catch {
		}*/

		//Define what our application ID is.
		string WolframAlphaApplicationID = "beta824g1";

		//Define what we want to search for.
		//string WolframAlphaSearchTerms = "england";

		//Print out what we're about to do in the console.
		Output("Getting response for the search terms \"" + WolframAlphaSearchTerms , 0, ConsoleColor.White);

		//Use the engine to get a response, from the application ID specified, and the search terms.
		//Engine.LoadResponse(WolframAlphaSearchTerms, WolframAlphaApplicationID);
		
		var query = new WolframAlphaQuery();
		query.APIKey = apiKey;
		query.Format = "plaintext";
		query.Query = WolframAlphaSearchTerms; //"england"; //"what day is today?"; 
		//Engine.LoadResponse(WolframAlphaSearchTerms);
		Engine.LoadResponse(query);
		
		//Print out a message saying that the last task was successful.
		Output("Response injected.", 0, ConsoleColor.White);

		//Make 2 empty spaces in the console.
		Output("", 0, ConsoleColor.White);
		
		Output("Response details", 1, ConsoleColor.Blue);
		
		"Showing details for Engine: {0}".info(Engine);	
		
		//Print out how many different pods that were found.
		Output("Pods found: " + Engine.QueryResult.NumberOfPods, 1, ConsoleColor.White);
		Output("Query pasing time: " + Engine.QueryResult.ParseTiming + " seconds", 1, ConsoleColor.White);
		Output("Query execution time: " + Engine.QueryResult.Timing + " seconds", 1, ConsoleColor.White);

		int PodNumber = 1;


		foreach (WolframAlphaPod Item in Engine.QueryResult.Pods) {
			//Make an empty space in the console.
			Output("", 0, ConsoleColor.White);

			Output("Pod " + PodNumber, 2, ConsoleColor.Red);

			Output("Sub pods found: " + Item.NumberOfSubPods, 2, ConsoleColor.White);
			Output("Title: \"" + Item.Title + "\"", 2, ConsoleColor.White);
			Output("Position: " + Item.Position, 2, ConsoleColor.White);

			int SubPodNumber = 1;


			foreach (WolframAlphaSubPod SubItem in Item.SubPods) {
				Output("", 0, ConsoleColor.White);

				Output("Sub pod " + SubPodNumber, 3, ConsoleColor.Magenta);
				Output("Title: \"" + SubItem.Title + "\"", 3, ConsoleColor.White);
				Output("Pod text: \"" + SubItem.PodText + "\"", 3, ConsoleColor.White);
				if (SubItem.PodImage.notNull())
				{
					Output("Pod image title: \"" + SubItem.PodImage.Title + "\"", 3, ConsoleColor.White);
					Output("Pod image width: " + SubItem.PodImage.Width, 3, ConsoleColor.White);
					Output("Pod image height: " + SubItem.PodImage.Height, 3, ConsoleColor.White);
					Output("Pod image location: \"" + SubItem.PodImage.Location.ToString() + "\"", 3, ConsoleColor.White);
					Output("Pod image description text: \"" + SubItem.PodImage.HoverText + "\"", 3, ConsoleColor.White);
				}
				SubPodNumber += 1;

			}

			PodNumber += 1;


		}

		//Make an empty space in the console.
		Output("", 0, ConsoleColor.White);

		//Make the application stay open until there is user interaction.
		//Output("All content has been saved to " + System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Wolfram Alpha wrapper log.log. Press a key to close the example.", 0, ConsoleColor.Green);
		//Console.ReadLine();

	}
}
