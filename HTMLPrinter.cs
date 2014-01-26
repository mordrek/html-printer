using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace html_printer
{
    public class HTMLPrinter
    {
        protected Size _size;
        public Size Size { get { return _size; } set { _size = value; OnResize(); } }

        System.Windows.Forms.WebBrowser _browser;
        protected AutoResetEvent _docLoadedEvent = new AutoResetEvent(false);

        public HTMLPrinter()
        {
            _browser = new System.Windows.Forms.WebBrowser();
            _browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(_browser_DocumentCompleted);
            Size = new Size(800,600);
        }



        public void Browse(string URL)
        {
            _browser.Navigate(URL);
            while (_browser.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();
            _docLoadedEvent.WaitOne(5000);
        }


        public Bitmap Print()
        {
            Bitmap image = new Bitmap(_browser.Bounds.Width, _browser.Bounds.Height);
            _browser.DrawToBitmap(image,_browser.Bounds);
            return image;
        }
        public void PrintToFile(string file)
        {
            Bitmap bmp = Print();
            bmp.Save(file);
        }

        protected virtual void OnResize()
        {
            _browser.ClientSize = Size;
            
        }

        protected virtual void _browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            _docLoadedEvent.Set();
        }
    }
}
