using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FURNAS.GestaoSPE.GovernancaDados;
using FURNAS.GestaoSPE.PersistenciaGovernanca.Proxy;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Model
{
    public class Contato
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }

        public Contato GetContato(GovernancaDados.GovernancaConsolidaSPE.Lists client, SPE itemSPE, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            client.Timeout = 360000;
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode listDocumentosBP = client.GetListItems("Contato SPE", null, null, viewFields, null, null, null);
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = @" <Where><And><Eq><FieldRef Name='Tipo_x0020_Contato' /><Value Type='Choice'>Página Web</Value></Eq><Eq><FieldRef Name='SPE' /> <Value Type='Lookup'>" + itemSPE.Title + "</Value></Eq></And></Where>";
            XmlNode listParticipacao = client.GetListItems("Contato SPE", null, ndQuery, viewFields, "1", null, null);

            Contato contato = null;
          
            foreach (XmlNode node in listDocumentosBP)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<Contato> XMLPersonHelper = new XMLHelper<Contato>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                            var tipo = Convert.ToString(item["ows_Tipo_x0020_Contato"].Value);
                            var spe = Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault());

                            if (Convert.ToInt32(item["ows_ID"].Value) == 11)
                            {


                            }
                            if (spe ==  itemSPE.ID && tipo == "Página Web")
                            {
                                var tipoBP = string.Empty;

                                contato = new Contato();
                                contato.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    contato.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_Tipo_x0020_Contato"] != null)
                                    contato.Tipo = Convert.ToString(item["ows_Tipo_x0020_Contato"].Value);

                            }
                          
                        }
                    }
                }
            }
            return contato;
        }
    }
}
