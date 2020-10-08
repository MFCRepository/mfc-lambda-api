using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace Utilities.Text
{
    public class XML
    {
            public T Deserialize<T>(string input) where T : class
            {
                /* Converts an XML string into a typed object
                 * 
                 * Usage:
                 * 
                 * var _obj = new <CLASS>();
                 * using (var _reader = new StringReader(XML))
                    {
                        _obj = new Utilities.Text.XML().Deserialize<CLASS>(_reader.ReadToEnd());
                    }
                *
                */
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

                using (StringReader sr = new StringReader(input))
                {
                    return (T)ser.Deserialize(sr);

                }
            }

            public string Serialize<T>(T ObjectToSerialize)
            {
                /*
                 * Converts an XML object into a UTF-8 encoded string
                 * 
                 * Usage:
                 * 
                 * string _xml = Utilities.XML().Serialize<CLASS>(XML-OBJECT);
                 * 
                 */

                XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

                using (StringWriterUtf8 textWriter = new StringWriterUtf8())
                {
                    xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                    return textWriter.ToString();
                }
            }
        }
    }
