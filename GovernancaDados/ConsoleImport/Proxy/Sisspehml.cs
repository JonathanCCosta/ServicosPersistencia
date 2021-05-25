using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ConsoleImportacao.GovernancaConsolidaSPE;

namespace ConsoleImportacao.Proxy
{
    public   class Sisspehml
    {

       



    }
    public class XMLHelper<T>
    {
        public string ToXML(T value, string xmlRootAttribute = null)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(value.GetType(), new XmlRootAttribute(xmlRootAttribute));
            var settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, value, emptyNamepsaces);
                return stream.ToString();
            }
        }

        public T FromXML(string XMLData, string xmlRootAttribute = null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttribute));
            T XMLObject = (T)serializer.Deserialize(new StringReader(XMLData));

            return XMLObject;
        }

    }

}
