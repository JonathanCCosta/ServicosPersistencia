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
    public class EscritorioSPE
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string TipoLogradouro { get; set; }
        public string Numeral { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string Tipo { get; set; }




        public EscritorioSPE GetEscritorio(GovernancaConsolidaSPE.Lists client, SPE itemSPE, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element,"Query","");



            ndQuery.InnerXml = @" <Where><And><Eq><FieldRef Name='Tipo_x0020_de_x0020_Escrit_x00f3' /><Value Type='Choice'>Sede</Value></Eq><Eq><FieldRef Name='SPE'  LookupId='TRUE' /> <Value Type='Lookup'>" + itemSPE.ID + "</Value></Eq></And></Where>";
            XmlNode listEscritorio = client.GetListItems("Escritório SPE", null, ndQuery, viewFields, "1", null, null);


     
            EscritorioSPE escritorioSPE = null;
         
            foreach (XmlNode node in listEscritorio)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<EscritorioSPE> XMLPersonHelper = new XMLHelper<EscritorioSPE>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;

                          
                            if (Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault()) == Convert.ToInt32(itemSPE.ID))
                            {
                                  escritorioSPE = new EscritorioSPE();
                                escritorioSPE.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    escritorioSPE.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_Tipo_x0020_Logradouro"] != null)
                                    escritorioSPE.TipoLogradouro = Convert.ToString(item["ows_Tipo_x0020_Logradouro"].Value);

                                if (item["ows_N_x00fa_meral"] != null)
                                    escritorioSPE.Numeral = Convert.ToString(item["ows_N_x00fa_meral"].Value);

                                if (item["ows_Bairro"] != null)
                                    escritorioSPE.Bairro = Convert.ToString(item["ows_Bairro"].Value);

                                if (item["ows_Estado"] != null)
                                    escritorioSPE.Estado = Convert.ToString(item["ows_Estado"].Value);

                                if (item["ows_CEP"] != null)
                                    escritorioSPE.CEP = Convert.ToString(item["ows_CEP"].Value);

                                if (item["ows_Tipo_x0020_de_x0020_Escrit_x00f3"] != null)
                                    escritorioSPE.Tipo = Convert.ToString(item["ows_Tipo_x0020_de_x0020_Escrit_x00f3"].Value); ;
                                

                            }
                        }
                    }
                }
            }
            return escritorioSPE;
          //  SPListItem endereco = null;
            // try
            // {                
          //  SPQuery query = new SPQuery();
         ////   query.Query = @"<Where><Eq><FieldRef Name='SPE' /> <Value Type='Lookup'>" + itemSPE.Title + "</Value></Eq></Where>";
           // return null;
            // return web.Lists["Escritório SPE"].GetItems(query).Cast<SPListItem>().First();                 
            // }
            // catch (Exception e)
            // {
            //endereco = null;
            // }
            //itemGovernanca["Endereco"] 
           // return endereco;
        }
    }
}
