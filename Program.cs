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

            bool scrollbars = false;
            bool overwrite = true;
            List<Parameter> parameters = new List<Parameter>();
            
            foreach (string s in args)
            {
                if (s.StartsWith("/src=", StringComparison.InvariantCultureIgnoreCase))
                    src = s.Substring("/src=".Length);
                if (s.StartsWith("/dst=", StringComparison.InvariantCultureIgnoreCase))
                    dst = s.Substring("/dst=".Length);
                if (s.StartsWith("/size=", StringComparison.InvariantCultureIgnoreCase))
                    size = s.Substring("/size=".Length);

                if (s.StartsWith("/param-", StringComparison.InvariantCultureIgnoreCase))
                {
                    parameters.Add(Parameter.Parse(s));
                }
                
                if (s.StartsWith("/scrollbars=", StringComparison.InvariantCultureIgnoreCase))
                    scrollbars = bool.Parse(s.Substring("/scrollbars=".Length));
                if (s.StartsWith("/overwrite=", StringComparison.InvariantCultureIgnoreCase))
                    overwrite = bool.Parse(s.Substring("/overwrite=".Length));
                
            }


            if (src == null)
            {
                Console.WriteLine("html-printer");
                Console.WriteLine("Usage: html-printer /src=<SourceURL> /dst=<destinationFilename> (Optional settings)");

                Console.WriteLine("Optional settings:");
                Console.WriteLine("  /size=<width>,<height>");
                Console.WriteLine("  /scrollbars=<true/false> (default false)");
                Console.WriteLine("  /overwrite=<true/false> (default true)");
                Console.WriteLine("  /param-key:<key>=<value> (replaces {{<key>}} in HTML with <value>)");
                Console.WriteLine("  /param-xpath:<path>=<value> (replaces value of the given xpath in the HTML)");
                Console.WriteLine("  /param-xml=<xmlfile> (selects the given file as source for xpaths in values)");
                Console.WriteLine("  /param-file=<paramFile> (loads parameters from a file. One paramvalue statement per line (including the slash)).");
            }
            else
            {
                if (File.Exists(dst) && overwrite == false)
                {
                    Console.WriteLine("File already exists: Exiting");
                    return;
                }

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
