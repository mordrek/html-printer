using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Xml.XPath;
using System.Xml;

namespace html_printer
{
    public class HTMLPrinter
    {
        protected Size _size;
        public Size Size { get { return _size; } set { _size = value; OnResize(); } }

        public bool Scrollbars { get { return _browser.ScrollBarsEnabled; } set { _browser.ScrollBarsEnabled = value; } }

        protected XPathDocument _xmlsource = null;
        protected XPathNavigator _xmlnavigator = null;
        protected List<Parameter> _parameters = new List<Parameter>();
        /// <summary>
        /// Parameters used to change the content of the HTML
        /// </summary>
        public List<Parameter> Parameters { get { return _parameters; } set { _parameters = value; } }

        System.Windows.Forms.WebBrowser _browser;
        protected XmlDocument _docXml = null;
        protected XPathNavigator _docNavigator = null;
        protected AutoResetEvent _docLoadedEvent = new AutoResetEvent(false);

        public HTMLPrinter()
        {
            _browser = new System.Windows.Forms.WebBrowser();
            _browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(_browser_DocumentCompleted);
            _browser.ScrollBarsEnabled = false;
            Size = new Size(800,600);
        }

        

        public void Browse(string URL)
        {
            try
            {
                if (URL.StartsWith("file://", StringComparison.InvariantCultureIgnoreCase))
                {
                    URL = Path.GetFullPath(URL.Replace("file://", "./"));
                    _browser.DocumentText = File.ReadAllText(URL);
                }
                else
                    _browser.Navigate(URL);
                _browser.Refresh();
                WaitForBrowse();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while trying to browse towards " + URL);
                throw ex;
            }

        }


        public Bitmap Print()
        {
            ApplyParameters(Parameters);
            Bitmap image = new Bitmap(_browser.Bounds.Width, _browser.Bounds.Height);
            _browser.DrawToBitmap(image,_browser.Bounds);
            return image;
        }
        public void PrintToFile(string file)
        {
            ApplyParameters(Parameters);
            Bitmap bmp = Print();
            bmp.Save(file);
        }

        protected virtual void OnResize()
        {
            _browser.ClientSize = Size;
            
        }

        protected void ApplyParameters(List<Parameter> parameters)
        {
            foreach (Parameter param in parameters)
            {
                switch (param.Type)
                {
                    case ParameterType.file:
                        string[] content = File.ReadAllLines(param.Value);
                        List<Parameter> fileParameters = new List<Parameter>();
                        foreach (string line in content)
                        {
                            if (line != null && line.Contains("="))
                            {
                                fileParameters.Add(Parameter.Parse(line));
                            }
                        }
                        ApplyParameters(fileParameters);
                        break;
                    case ParameterType.key:
                        string curText = _browser.DocumentText.Replace("{{" + param.Key + "}}", GetParamValue(param.Value));
                        _browser.DocumentText=curText;
                        WaitForBrowse();
                        break;
                    case ParameterType.xpath:
                        if (_docNavigator != null)
                        {
                            string val = GetParamValue(param.Value);
                            foreach (XPathNavigator node in _docNavigator.Select(param.Key))
                            {
                                node.SetValue(val);
                            }
                            using (var stringWriter = new StringWriter())
                            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                            {
                                _docXml.WriteTo(xmlTextWriter);
                                xmlTextWriter.Flush();
                                _browser.DocumentText = stringWriter.GetStringBuilder().ToString();
                                WaitForBrowse();
                            }
                            
                        }
                        break;
                    case ParameterType.xml:
                        _xmlsource = new XPathDocument(param.Value);
                        _xmlnavigator = _xmlsource.CreateNavigator();
                        break;
                }
            }
            
            
        }

        protected string GetParamValue(string value)
        {
            if(_xmlnavigator == null)
                return value;

            XPathNavigator nav= _xmlnavigator.SelectSingleNode(value);
            if(nav!=null)
            return nav.Value;

            return value;
        }

        protected void WaitForBrowse()
        {
            while (_browser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
            _docLoadedEvent.WaitOne(5000);
        }


        protected virtual void _browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                _docXml = new XmlDocument();
                _docXml.LoadXml(_browser.DocumentText);
                _docNavigator = _docXml.CreateNavigator();
            }
            catch
            { }
            _docLoadedEvent.Set();
        }
    }
}
