using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

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
            string format = "png";
            bool scrollbars = false;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            
            foreach (string s in args)
            {
                if (s.StartsWith("/src=", StringComparison.InvariantCultureIgnoreCase))
                    src = s.Substring("/src=".Length);
                if (s.StartsWith("/dst=", StringComparison.InvariantCultureIgnoreCase))
                    dst = s.Substring("/dst=".Length);
                if (s.StartsWith("/size=", StringComparison.InvariantCultureIgnoreCase))
                    size = s.Substring("/size=".Length);

                if (s.StartsWith("/param:", StringComparison.InvariantCultureIgnoreCase))
                {
                    string keyval = s.Substring("/param:".Length);
                    string[] parts = keyval.Split('=');
                    parameters[parts[0]] = parts[1];
                }
                if (s.StartsWith("/param=", StringComparison.InvariantCultureIgnoreCase))
                {
                    string file = s.Substring("/param=".Length);
                    string[] content = File.ReadAllLines(file);
                    foreach (string line in content)
                    {
                        if (line != null && line.Contains("="))
                        {
                            string[] parts = line.Split('=');
                            parameters[parts[0]] = parts[1];
                        }
                    }
                }
                if (s.StartsWith("/scrollbars=", StringComparison.InvariantCultureIgnoreCase))
                    scrollbars = bool.Parse(s.Substring("/scrollbars=".Length));
                
            }
            if(dst==null && src!=null)
                dst = src+"."+format;

            if (src == null)
            {
                Console.WriteLine("html-printer");
                Console.WriteLine("Usage: html-printer /src=SourceURL /dst=destinationFilename (Optional settings)");

                Console.WriteLine("Optional settings:");
                Console.WriteLine("  /size=width,height");
                Console.WriteLine("  /scrollbars=true/false (default false)");
                Console.WriteLine("  /param:key=value (replaces {{key}} in HTML with value)");
                Console.WriteLine("  /param:id.attr=value (replaces value of attr in the element with given id in the HTML)");
                Console.WriteLine("  /param=paramFile");
            }
            else
            {
                HTMLPrinter printer = new HTMLPrinter();
                if (size != null)
                    printer.Size = new Size(int.Parse(size.Substring(0, size.IndexOf(","))),
                                            int.Parse(size.Substring(size.IndexOf(",") + 1)));
                printer.Browse(src);
                printer.Scrollbars = scrollbars;
                printer.Parameters = parameters;
                printer.PrintToFile(dst);
            }
        }
    }
}
