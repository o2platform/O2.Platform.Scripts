using FluentSharp.WinForms;
using O2.FluentSharp.VisualStudio.ExtensionMethods;

//O2File:O2_VS_AddIn.cs

namespace O2.FluentSharp.VisualStudio
{
	public class O2_LogViewer : CommandBase
	{
		
		public O2_LogViewer(O2_VS_AddIn o2AddIn) : base(o2AddIn)
		{
			this.create();
		}

		public override void create()
		{
			this.ButtonText = "O2 LogViewer";			
			this.ToolTip = "Opens the LogViewer";
			this.TargetMenu = "O2 Platform";
			base.create();			
			this.Execute = () =>
					{
						
						var panel = O2AddIn.VS_Dte.createWindowWithPanel(O2AddIn.VS_AddIn, "test window");
						panel.add_LogViewer();

						//"Util - LogViewer.h2".local().executeH2Script();
					};
		}		
	}
}
