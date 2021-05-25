using FURNAS.GestaoSPE.PersistenciaGovernanca.Proxy;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Model
{
    public class Obra
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public Empreendimento Emprendimento { get; set; }
        public List<Atividades> Atividades { get; set; }
        public List<Obra> itensObras { get; set; }
        public  KeyValuePair<int, DateTime?>PesoData { get; set; }
        public Obra()
        {

        }
        public Obra(GovernancaConsolidaSPE.Lists client, Empreendimento empreendimento, Usuario usuario )
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
             client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
       
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='Empreendimento'  LookupId='TRUE'/><Value Type='Text'>" + empreendimento.ID + "</Value></Eq></Where>";          
            XmlNode listObras = client.GetListItems("Obra", null, ndQuery, viewFields, null, null, null);
           
            Obra obra = null;
               itensObras = new List<Obra>();
            foreach (XmlNode node in listObras)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<Obra> XMLPersonHelper = new XMLHelper<Obra>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                           obra =  new Obra();

                            if (Convert.ToInt32(Convert.ToString(item["ows_Empreendimento"].Value).Split(';').FirstOrDefault()) == empreendimento.ID)
                            {
                                obra.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    obra.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_Empreendimento"] != null)
                                {
                                    obra.Emprendimento = new Empreendimento();
                                    obra.Emprendimento = empreendimento;
                                }


                                itensObras.Add(obra);
                            }
                        }
                    }
                }
            }
        }
    }
}
