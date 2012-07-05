// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.Ascx.MainGUI;
using O2.Views.ASCX.O2Findings;
using O2.External.SharpDevelop.ExtensionMethods;
using System.ComponentModel;

//O2Ref:O2_FluentSharp_BCL.dll

namespace O2.XRules.Database.Utils
{	
/*	public static class _Extra_extensionMethods_Component
	{
		public static List<Tuple<object,Delegate>> eventHandlers(this Component component)			
		{
			var mappedEvents = new List<Tuple<object,Delegate>>();
			var events = (EventHandlerList)component.prop("Events");
			var next = events.field("head");
			while(next!=null)
			{
				var key = next.field("key");
				
				var handler = (Delegate)next.field("handler");	
				mappedEvents.Add(new Tuple<object,Delegate>(key,handler));
				next = next.field("next");
			}
			return mappedEvents;
		}
		
		public static string eventHandlers_MethodSignatures(this Component component)			
		{
			var signatures = "";
			foreach(var eventHandler in component.eventHandlers())
				if (eventHandler.Item2 != null)
					signatures += eventHandler.Item2.Method.str().line();
			return signatures;
		}
		
		public static T remove_EventHandler<T>(this T component, string eventId)
			where T : Component
		{			
			var eventIdObject =  typeof(T).ctor().field(eventId);
			
			//get private 'Events' property
			var events = (EventHandlerList)component.prop("Events");
			//invoke private method 'find' in order to get the  SelectedIndexChanged event entry 
			var listEntry = events.invoke("Find", eventIdObject);
			//get the private field 'handler'
			var handler = (EventHandler)listEntry.field("handler");
			//now that we have the EventHandler object we can remove it normaly
			events.invoke("RemoveHandler", eventIdObject, handler);
			return (T)component;
		}				
		
		public static T remove_Event_SelectedIndexChanged<T>(this T component)
			where T : Component
		{
			return component.remove_EventHandler("EVENT_SELECTEDINDEXCHANGED");
		}
		
	}	
*/	
	//this has to stay here due to the use of the Tuple
	public static class _Extra_extensionMethods_Component
	{
		public static List<Tuple<object,Delegate>> eventHandlers(this Component component)			
		{
			var mappedEvents = new List<Tuple<object,Delegate>>();
			var events = (EventHandlerList)component.prop("Events");
			var next = events.field("head");
			while(next!=null)
			{
				var key = next.field("key");
				
				var handler = (Delegate)next.field("handler");	
				mappedEvents.Add(new Tuple<object,Delegate>(key,handler));
				next = next.field("next");
			}
			return mappedEvents;
		}
		
		public static string eventHandlers_MethodSignatures(this Component component)			
		{
			var signatures = "";
			foreach(var eventHandler in component.eventHandlers())
				if (eventHandler.Item2 != null)
					signatures += eventHandler.Item2.Method.str().line();
			return signatures;
		}
		
		public static T remove_EventHandler<T>(this T component, string eventId)
			where T : Component
		{			
			var eventIdObject =  typeof(T).ctor().field(eventId);
			
			//get private 'Events' property
			var events = (EventHandlerList)component.prop("Events");
			//invoke private method 'find' in order to get the  SelectedIndexChanged event entry 
			var listEntry = events.invoke("Find", eventIdObject);
			//get the private field 'handler'
			var handler = (EventHandler)listEntry.field("handler");
			//now that we have the EventHandler object we can remove it normaly
			events.invoke("RemoveHandler", eventIdObject, handler);
			return (T)component;
		}				
		
		public static T remove_Event_SelectedIndexChanged<T>(this T component)
			where T : Component
		{
			return component.remove_EventHandler("EVENT_SELECTEDINDEXCHANGED");
		}
		
	}
	
}    	