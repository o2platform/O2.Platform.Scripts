// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.WinForms;
using AxWMPLib;
//O2Ref:Interop.WMPLib.dll
//O2Ref:AxInterop.WMPLib.dll

namespace O2.XRules.Database.APIs
{

	public class API_WindowsMediaPlayer_Test
	{
		public void launch()
		{
			"Windows Media Player".showAsForm<API_WindowsMediaPlayer>();		
		}
	}
    public class API_WindowsMediaPlayer : Panel
    {    
    	public AxWindowsMediaPlayer MediaPlayer { get; set; }

        public API_WindowsMediaPlayer()
    	{    	    	
			setup();			
		}  
		
		public API_WindowsMediaPlayer setup()
		{
			MediaPlayer = new AxWindowsMediaPlayer(); 
			MediaPlayer.fill();
			this.add_Control(MediaPlayer);									
			return this;
		}
    }
    
    public static class API_WindowsMediaPlayer_ExtensionMethods_Core
    {
		public static API_WindowsMediaPlayer play(this API_WindowsMediaPlayer mediaPlayer, string pathToWmf)
		{
			return (API_WindowsMediaPlayer)mediaPlayer.invokeOnThread(
				()=>{
						"loading video file :{0}".info(pathToWmf);
						mediaPlayer.MediaPlayer.URL = pathToWmf;
						return mediaPlayer;
					});
		}
		public static API_WindowsMediaPlayer play(this API_WindowsMediaPlayer mediaPlayer)
		{
			return mediaPlayer.start();
		}
		
		public static API_WindowsMediaPlayer start(this API_WindowsMediaPlayer mediaPlayer)
		{
			return (API_WindowsMediaPlayer)mediaPlayer.invokeOnThread(
				()=>{
						mediaPlayer.MediaPlayer.Ctlcontrols.play();
						return mediaPlayer;
					});
		}
		
		public static API_WindowsMediaPlayer stop(this API_WindowsMediaPlayer mediaPlayer)
		{
			return (API_WindowsMediaPlayer)mediaPlayer.invokeOnThread(
				()=>{
						mediaPlayer.MediaPlayer.Ctlcontrols.stop();
						return mediaPlayer;
					});
		}
		
		public static API_WindowsMediaPlayer fastForward(this API_WindowsMediaPlayer mediaPlayer)
		{
			return (API_WindowsMediaPlayer)mediaPlayer.invokeOnThread(
				()=>{
						mediaPlayer.MediaPlayer.Ctlcontrols.fastForward();
						return mediaPlayer;
					});
		}
		
		public static API_WindowsMediaPlayer fastReverse(this API_WindowsMediaPlayer mediaPlayer)
		{
			return (API_WindowsMediaPlayer)mediaPlayer.invokeOnThread(
				()=>{
						mediaPlayer.MediaPlayer.Ctlcontrols.fastReverse();
						return mediaPlayer;
					});
		}
												
    }
    
    public static class API_WindowsMediaPlayer_ExtensionMethods_WinForms
    {        	
    	
    	public static API_WindowsMediaPlayer add_Video<T>(this T control)
    		where T : Control
    	{
    		return control.add_WindowsMediaPlayer();
    	}
    	
    	public static API_WindowsMediaPlayer add_MediaPlayer<T>(this T control)
    		where T : Control
    	{    		
    		return control.add_WindowsMediaPlayer();
    	}
    	
    	public static API_WindowsMediaPlayer add_WindowsMediaPlayer<T>(this T control)
    		where T : Control
    	{    		    		
    		return control.add_Control<API_WindowsMediaPlayer>();
    	}
    	
    	public static AxWindowsMediaPlayer add_WindowsMediaPlayers<T>(this T control)
    		where T : Control
    	{    		
    		return (AxWindowsMediaPlayer)control.invokeOnThread(
    			()=>{
    					var mediaPlayer = new AxWindowsMediaPlayer(); 
						mediaPlayer.fill();
						control.add_Control(mediaPlayer);
						return mediaPlayer;
    				});
    			
				    		
    	}
    }
}
