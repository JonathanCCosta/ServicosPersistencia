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
    public class ParticipacaoSPE
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Empresa { get; set; }
        public double Participacao { get; set; }
        public List<ParticipacaoSPE> GetParticipacoes(GovernancaDados.GovernancaConsolidaSPE.Lists client, SPE itemSPE, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");


            ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='SPE' /> <Value Type='Lookup'>" + itemSPE.Title + "</Value></Eq></Where>";
            XmlNode listParticipacao = client.GetListItems("Participação SPE", null, ndQuery, viewFields, "100", null, null);


            ParticipacaoSPE participacaoSPE = null;
            List<ParticipacaoSPE> lsl = new List<ParticipacaoSPE>();
            foreach (XmlNode node in listParticipacao)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<ParticipacaoSPE> XMLPersonHelper = new XMLHelper<ParticipacaoSPE>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;

                            if (Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault()) == Convert.ToInt32(itemSPE.ID))
                            {
                                participacaoSPE = new ParticipacaoSPE();
                                participacaoSPE.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    participacaoSPE.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_Empresa"] != null)
                                    participacaoSPE.Empresa = Convert.ToString(item["ows_Empresa"].Value).Split('#').LastOrDefault();

                                if (item["ows_Participa_x00e7__x00e3_o"] != null)
                                    participacaoSPE.Participacao = Convert.ToDouble(item["ows_Participa_x00e7__x00e3_o"].Value);

                                lsl.Add(participacaoSPE);
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }



            return lsl;

        }
    }
}
