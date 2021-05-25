using ConsoleImportacao.Proxy;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleImportacao
{
    public class FluxodeCaixa
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Nivel1 { get; set; }
        public List<FluxodeCaixa> GetFluxodeCaixa(GovernancaConsolidaSPE.Lists client, ConsoleImportacao.SPE itemSPE, Usuario usuario)
        {
            
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='SPE' /> <Value Type='Lookup'>" + itemSPE.Title + "</Value></Eq></Where>";
            XmlNode listFluxoCaixa = client.GetListItems("Fluxo de Caixa", null, ndQuery, viewFields, "1000", null, null);

            List<int> todos = new List<int>();
            FluxodeCaixa fluxodeCaixa = null;
            List<FluxodeCaixa> lsl = new List<FluxodeCaixa>();
            foreach (XmlNode node in listFluxoCaixa)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<FluxodeCaixa> XMLPersonHelper = new XMLHelper<FluxodeCaixa>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                            todos.Add(f);              

                            if(Convert.ToInt32(item["ows_ID"].Value) == 5)
                            {


                            }
                            if (Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault()) == itemSPE.ID )
                            {
                                fluxodeCaixa = new FluxodeCaixa();
                                fluxodeCaixa.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    fluxodeCaixa.Title = Convert.ToString(item["ows_Title"].Value);


                                lsl.Add(fluxodeCaixa);
                            }
                        }
                    }
                }
            }
            var teste = todos;
            return lsl;
          
          
        }
    }
}
