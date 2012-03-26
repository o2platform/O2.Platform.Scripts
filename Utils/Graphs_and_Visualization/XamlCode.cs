// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.XRules.Database.APIs
{
    public class XamlCode
    {   
    	public static string Label()
    	{
    		return  "<Label ".line() +
					"	FontSize=\"20\">".line() +
					"	This is new DesignerCanvas".line() +
					"</Label>";
    	}
    	public static string Rectangle()
    	{
    		return  "<Rectangle".line()+
					"    Width=\"200\"".line()+
					"    Height=\"100\"".line()+
					"    Fill=\"Blue\"".line()+
					"    Stroke=\"Black\"".line()+
					"    Opacity=\"0.2\"".line()+
					"    StrokeThickness=\"3\" />".line();
    	}
    	
    	public static string Simple_Button()
    	{
    		var xamlCode =  "<Button>".line() + 
    					    " click me".line() + 
    					    "</Button>";
    		return xamlCode;
    	}
    	
    	public static string Button_With_Style()
    	{
    		return new_Button("Click me with sytle");
    	}
    	
		public static string new_Button(string text)
		{
			var button_Height = "30";
			var button_Margin = "2";
			var button_CornerRadius = "10";
			//var color_Button = "YellowGreen";
			var color_Button = "LightBlue";
			var color_IsMouseOver = "Azure";//"Gold";
			var color_IsPressed = "LightGray";
			return new_Button(text, button_Height,button_Margin, button_CornerRadius,color_Button, color_IsMouseOver, color_IsPressed);
		}
		
		public static string new_Button(string text, string button_Height, string button_Margin, string button_CornerRadius, string color_Button, string color_IsMouseOver, string color_IsPressed)
		{
			var xamlCode =  "  <Button Height=\"" + button_Height + "\" Content=\"" + text + "\" Margin=\"" + button_Margin + "\" >".line() +
							"    <Button.Template>".line()+
							"      <ControlTemplate TargetType=\"{x:Type Button}\">".line()+
							"        <Border x:Name=\"Border\" Background=\""+ color_Button +"\" CornerRadius=\"" + button_CornerRadius + "\">".line()+
							"          <ContentPresenter VerticalAlignment=\"Center\" HorizontalAlignment=\"Center\" />"+
							"        </Border>".line()+
							"        <ControlTemplate.Triggers>".line()+
							"          <Trigger Property=\"IsMouseOver\" Value=\"True\">".line()+
							"            <Setter TargetName=\"Border\" Property=\"Background\" Value=\"" + color_IsMouseOver+ "\" />".line()+
							"          </Trigger>".line()+
							"          <Trigger Property=\"IsPressed\" Value=\"True\">".line()+
							"            <Setter TargetName=\"Border\" Property=\"Background\" Value=\"" + color_IsPressed + "\" />".line()+
							"          </Trigger>".line()+
							"        </ControlTemplate.Triggers>".line()+
							"      </ControlTemplate>".line()+
							"    </Button.Template>".line()+
							"  </Button>".line();
			return xamlCode;
		}
		
		public static string link()
		{
			return link("Click me","5");
		}
		
		public static string link(string text, string margin)
		{
			return  "<Button Margin=\"{0}\" Content=\"{1}\" Cursor=\"Hand\" Focusable=\"False\">".format(margin, text).line()+
					 "        <Button.Template>".line()+
					 "            <ControlTemplate TargetType=\"Button\">".line()+
					 "                <TextBlock TextDecorations=\"Underline\">".line()+
					 "                    <ContentPresenter />".line()+
					 "                </TextBlock>".line()+
					 "            </ControlTemplate>".line()+
					 "        </Button.Template>".line()+
					 "        <Button.Style>".line()+
					 "            <Style TargetType=\"Button\">".line()+
					 "                <Setter Property=\"Foreground\" Value=\"Blue\" />".line()+
					 "                <Style.Triggers>".line()+
					 "                    <Trigger Property=\"IsMouseOver\" Value=\"true\">".line()+
					 "                        <Setter Property=\"Foreground\" Value=\"Red\" />".line()+
					 "                    </Trigger>".line()+
					 "                </Style.Triggers>".line()+
					 "            </Style>".line()+
					 "        </Button.Style>".line()+
					 "    </Button>".line();

		}
    }      
}
