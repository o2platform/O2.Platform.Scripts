using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using O2.Kernel					 .ExtensionMethods;
using O2.FluentSharp.VisualStudio.ExtensionMethods;
using O2.DotNetWrappers			 .ExtensionMethods;
using O2.Views.ASCX				 .ExtensionMethods;

//O2File:O2_VS_AddIn.cs

namespace O2.FluentSharp.VisualStudio
{
	public class VS2010_TeamMentor_Menu : CommandBase
	{
		
		public VS2010_TeamMentor_Menu(O2_VS_AddIn o2AddIn) : base(o2AddIn)
		{			
			this.create();
		}

		public override void create()
		{
			"before create".error();	
			this.ButtonText = "VS2010_TeamMentor_Menu";			
			this.ToolTip = "Opens the TeamMentor";
			this.TargetMenu = "O2 Platform";
			base.create();			
			this.Execute = () =>
					{
						
						"on Execute".error();	
						var panel = O2AddIn.VS_Dte.createWindowWithPanel(O2AddIn.VS_AddIn, "test window");
						panel.add_LogViewer();

						//"Util - LogViewer.h2".local().executeH2Script();
					};
					"on create".error();	
		}		
	}
}
