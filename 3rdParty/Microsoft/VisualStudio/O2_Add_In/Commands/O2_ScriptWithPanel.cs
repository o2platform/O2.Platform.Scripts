//O2File:O2_VS_AddIn.cs

namespace FluentSharp.VisualStudio.Utils
{
	public class O2_ScriptWithPanel : CommandBase
	{

		public O2_ScriptWithPanel(O2_VS_AddIn o2AddIn) : base(o2AddIn)
		{
			this.create();			
		}

		public override void create()
		{
			var title = "O2 REPL Script Editor - with Panel";
			this.ButtonText = title;
			this.ToolTip = "Opens the O2 Script GUI (with a top panel)";
			this.TargetMenu = "O2 Platform";
			base.create();
			this.Execute = () =>
						{														
							var o2Script = "ascx_Quick_Development_GUI.cs.o2";
							var type	 = "ascx_Panel_With_Inspector";
							O2AddIn.add_WinForm_Control_from_O2Script(title, o2Script, type, 500,300);
						};
		}
		
	}
}
