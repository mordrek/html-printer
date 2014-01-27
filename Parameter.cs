using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace html_printer
{
    public enum ParameterType { key, xpath, xml, file }
    public class Parameter
    {
        public Parameter() { }
        public Parameter(ParameterType aType, string aKey, string aValue)
        {
            Type = aType;
            Key = aKey;
            Value = aValue;
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public ParameterType Type { get; set; }
        public static Parameter Parse(string str)
        {
            // Cut off any quote
            if (str.StartsWith("\"") && str.EndsWith("\""))
                str = str.Substring(1, str.Length - 2);

            // Cut off the prefix
            if (str.StartsWith("/param-", StringComparison.InvariantCultureIgnoreCase))
                str = str.Substring("/param-".Length);

            int eqPos = str.IndexOf("=");
            if (eqPos < 0)
                throw new Exception("A parameter must be on the form /param-TYPE:KEY=VALUE or /param-TYPE=VALUE");
            string value = str.Substring(eqPos + 1);
            str = str.Substring(0, eqPos);

            string key = "";
            int typePos = str.IndexOf(":");
            string sType = str;
            if (typePos >= 0)
            {
                sType = str.Substring(0, typePos);
                key = str.Substring(typePos + 1);
            }
            ParameterType type = ParameterType.key;
            
            type = (ParameterType)Enum.Parse(typeof(ParameterType), sType);
            str = str.Substring(typePos + 1);

            return new Parameter(type, key, value);
        }
    }
}
