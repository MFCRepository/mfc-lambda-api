using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities.Text
{
    public class StringWriterUtf8 : System.IO.StringWriter
    {
        /* 
         * overrides default UTF-16 encoding and outputs UTF-8 
         * needed for some older systems that can't handle UTF-16
         * 
         * Usage:
         * 
         * StringWriterUtf8 stringWriter = new StringWriterUtf8();
         * //write to stringWriter object and get value
         * stringWriter.ToString();
         *
         */

        public override System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }
    }
}
