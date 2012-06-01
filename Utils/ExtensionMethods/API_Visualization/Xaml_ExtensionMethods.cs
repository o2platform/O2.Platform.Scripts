using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Integration;
using System.Windows;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using System.Windows.Markup;
using System.Windows.Controls;
using System.IO;
using System.Xml;
using O2.API.Visualization.ExtensionMethods;

//O2File:WPF_Controls_ExtensionMethods.cs

//O2Ref:WindowsFormsIntegration.dll
//O2Ref:GraphSharp.dll
//O2Ref:QuickGraph.dll
//O2Ref:GraphSharp.Controls.dll 

namespace O2.XRules.Database.Utils
{
    public static class Xaml_ExtensionMethods
    {
        public static ElementHost add_XamlViewer(this System.Windows.Forms.Control control)
        {
            return control.add_WPF_Host();
        }

        public static UIElement showXaml(this ElementHost elementHost, string xamlCode)
        {
            return (UIElement)elementHost.invokeOnThread(
                () =>
                {
                    try
                    {
                        if (xamlCode.valid())
                        {
                            ParserContext parserContext = new ParserContext();
                            parserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                            parserContext.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                            UIElement childElement = null;
                            var xamlObject = XamlReader.Parse(xamlCode, parserContext);
                            if (xamlObject is UIElement)
                            {
                                var frame = new Frame();
                                frame.Content = (UIElement)xamlObject;
                                childElement = frame;
                                elementHost.Child = childElement;
                                "Xaml code loaded into ElementHost".info();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        "in showXaml: {0}".format(ex.Message).error();		// trying to display the InnerException was trowing an error
                        //ex.log();
                    }
                    return null;
                });
        }

        public static UIElement xaml_CreateUIElement<T>(this string xamlCode)
            where T : UIElement
        {
            return xamlCode.xaml_CreateUIElement(null);
        }

        public static UIElement xaml_CreateUIElement<T>(this string xamlCode, Dictionary<string, string> extraXmlnsValues)
            where T : UIElement
        {
            var newElement = xamlCode.xaml_CreateUIElement(extraXmlnsValues);
            if (newElement is T)
                return (T)newElement;
            return null;
        }

        public static UIElement xaml_CreateUIElement(this string xamlCode)
        {
            return xamlCode.xaml_CreateUIElement(null);
        }

        public static UIElement xaml_CreateUIElement(this string xamlCode, Dictionary<string, string> extraXmlnsValues)
        {
            try
            {
                if (xamlCode.valid())
                {
                    ParserContext parserContext = new ParserContext();
                    parserContext.XamlTypeMapper = new XamlTypeMapper(new string[] { "AvalonDock" });

                    parserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    parserContext.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                    if (extraXmlnsValues.notNull())
                        foreach (var item in extraXmlnsValues)
                            parserContext.XmlnsDictionary.Add(item.Key, item.Value);
                    //parserContext.XmlnsDictionary.Add();
                    var xamlObject = XamlReader.Parse(xamlCode, parserContext);
                    if (xamlObject is UIElement)
                        return (UIElement)xamlObject;
                }
            }
            catch (Exception ex)
            {
                ex.log("in xaml_CreateUIElement", false);
            }
            return null;
        }

        public static T xaml_CreateUIElement<T>(this UIElement uiElement, string xamlCode)
            where T : UIElement
        {
            return uiElement.xaml_CreateUIElement<T>(xamlCode, null);
        }

        public static T xaml_CreateUIElement<T>(this UIElement uiElement, string xamlCode, Dictionary<string, string> extraXmlnsValues)
            where T : UIElement
        {
            var newElement = uiElement.xaml_CreateUIElement(xamlCode, extraXmlnsValues);
            if (newElement is T)
                return (T)newElement;
            return null;
        }

        public static UIElement xaml_CreateUIElement(this UIElement uiElement, string xamlCode)
        {
            return uiElement.xaml_CreateUIElement(xamlCode, null);
        }

        public static UIElement xaml_CreateUIElement(this UIElement uiElement, string xamlCode, Dictionary<string, string> extraXmlnsValues)
        {
            return (UIElement)uiElement.wpfInvoke(
                () =>
                {
                    var newUIElement = xamlCode.xaml_CreateUIElement(extraXmlnsValues);
                    uiElement.add_Control_Wpf(newUIElement);
                    return newUIElement;
                });
        }

        public static string xaml(this UIElement uiElement)
        {
            return uiElement.xaml_Code(true);
        }

        public static string xaml_Code(this UIElement uiElement)
        {
            return uiElement.xaml_Code(true);
        }

        public static string xaml_Code(this UIElement uiElement, bool indent)
        {
            return (string)uiElement.wpfInvoke(
                () =>
                {
                    var stringWriter = new StringWriter();
                    var xmlTextWriter = new XmlTextWriter(stringWriter);
                    xmlTextWriter.Formatting = (indent) ? Formatting.Indented : Formatting.None;



                    //xmlTextWriter.Settings.prop("OutputMethod",XmlOutputMethod.Xml);


                    XamlWriter.Save(uiElement, xmlTextWriter);
                    return stringWriter.str();
                });
        }
    }
}
