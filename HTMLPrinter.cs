using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace html_printer
{
    public class HTMLPrinter
    {
        protected Size _size;
        public Size Size { get { return _size; } set { _size = value; OnResize(); } }

        public bool Scrollbars { get { return _browser.ScrollBarsEnabled; } set { _browser.ScrollBarsEnabled = value; } }
             
        
        protected Dictionary<string, string> _parameters = new Dictionary<string,string>();
        /// <summary>
        /// Parameters used to change the content of the HTML
        /// A parameter can be written in two ways:
        /// 1) key=val
        /// Any text in the HTML that is equal to {{key}} will be replaced by value
        /// 2) id.attr = val
        /// The html is searched for tags with the given id. The attribute attr of the tag will be set to the value val
        /// </summary>
        public Dictionary<string, string> Parameters { get { return _parameters; } set { _parameters = value; } }

        System.Windows.Forms.WebBrowser _browser;
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
            ApplyParameters();
            Bitmap image = new Bitmap(_browser.Bounds.Width, _browser.Bounds.Height);
            _browser.DrawToBitmap(image,_browser.Bounds);
            return image;
        }
        public void PrintToFile(string file)
        {
            ApplyParameters();
            Bitmap bmp = Print();
            bmp.Save(file);
        }

        protected virtual void OnResize()
        {
            _browser.ClientSize = Size;
            
        }

        protected void ApplyParameters()
        {
            
            
            foreach (KeyValuePair<string, string> kvp in Parameters)
            {
                if (kvp.Key.Contains('.')==false)
                     ApplyParameter(kvp.Key, kvp.Value);

            }

            WaitForBrowse();

            foreach (KeyValuePair<string, string> kvp in Parameters)
            {
                if (kvp.Key.Contains('.'))
                    ApplyIdAttribute(kvp.Key.Substring(0, kvp.Key.IndexOf('.')), kvp.Key.Substring(kvp.Key.IndexOf('.') + 1), kvp.Value);

            }
        }

        protected void ApplyParameter(string key, string value)
        {
            string curText = _browser.DocumentText.Replace("{{" + key + "}}", value);
            _browser.DocumentText=curText;
        }
        protected void ApplyIdAttribute(string id, string attr, string value)
        {
            HtmlElement element = _browser.Document.GetElementById(id);
            element.SetAttribute(attr, value);
           
        }

        protected void WaitForBrowse()
        {
            while (_browser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
            _docLoadedEvent.WaitOne(5000);
        }


        protected virtual void _browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            _docLoadedEvent.Set();
        }
    }
}
