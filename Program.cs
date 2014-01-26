using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace html_printer
{
    class Program
    {
         [STAThread]
        static void Main(string[] args)
        {
            string src = null;
            string dst = null;
            string size = null;
            foreach (string s in args)
            {
                if (s.StartsWith("/src=", StringComparison.InvariantCultureIgnoreCase))
                    src = s.Substring("/src=".Length);
                if (s.StartsWith("/dst=", StringComparison.InvariantCultureIgnoreCase))
                    dst = s.Substring("/dst=".Length);
                if (s.StartsWith("/size=", StringComparison.InvariantCultureIgnoreCase))
                    size = s.Substring("/size=".Length);
            }
            if (src == null || dst == null)
            {
                Console.WriteLine("html-printer");
                Console.WriteLine("Usage: html-printer /src=SourceURL /dst=DestinationFilename (/size=width,height)");
            }
            else
            {
                HTMLPrinter printer = new HTMLPrinter();
                if (size != null)
                    printer.Size = new Size(int.Parse(size.Substring(0, size.IndexOf(","))),
                                            int.Parse(size.Substring(size.IndexOf(",") + 1)));
                printer.Browse(src);
                printer.PrintToFile(dst);
            }
        }
    }
}
