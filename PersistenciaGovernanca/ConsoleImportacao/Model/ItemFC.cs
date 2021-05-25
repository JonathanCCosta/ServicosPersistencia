using ConsoleImportacao.Proxy;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleImportacao.Model
{
    public class ItemFC
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Nivel1 { get; set; }
        public string Valor { get; set; }
        public FluxodeCaixa FC { get; set; }
        public List<ItemFC> GetItemFC(GovernancaConsolidaSPE.Lists client, List<FluxodeCaixa> fluxodeCaixa, Usuario usuario)
        {

            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode listFC= client.GetListItems("Item FC", null, null, viewFields, "5000", null, null);

            ItemFC itemFC = null;
            List<ItemFC> lslItemFC = new List<ItemFC>();
            foreach (XmlNode node in listFC)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<ItemFC> XMLPersonHelper = new XMLHelper<ItemFC>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                            string n1 = string.Empty;

                        
                            if (item["ows_N_x00ed_vel_x0020_1_x0020_FC"] != null)
                                n1 = Convert.ToString(item["ows_N_x00ed_vel_x0020_1_x0020_FC"].Value).Split('#').LastOrDefault();

                           if (n1 != null && n1 == "(-) Investimentos")
                           // if (n1 != null && n1 == "(-) Amortização")                            
                            {
                                itemFC = new ItemFC();
                                itemFC.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    itemFC.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_N_x00ed_vel_x0020_1_x0020_FC"] != null)
                                    itemFC.Nivel1 = n1;

                                if (item["ows_Valor"] != null)
                                     itemFC.Valor = item["ows_Valor"].Value;

                               

                                if (item["ows_FC"] != null)
                                    itemFC.FC = new FluxodeCaixa() { Title = Convert.ToString(item["ows_FC"].Value).Split('#').LastOrDefault(), ID = Convert.ToInt32(Convert.ToString(item["ows_FC"].Value).Split(';').FirstOrDefault())  };

                                lslItemFC.Add(itemFC);
                            }
                        }
                    }
                }
            }
            List<ItemFC> lslItemFCAux = new List<ItemFC>();
            foreach (FluxodeCaixa itemFLuxo in fluxodeCaixa)
            {
               
                foreach (ItemFC itenFC in lslItemFC)
                {
                    if (itenFC.FC.ID == itemFLuxo.ID)
                        lslItemFCAux.Add(itenFC);
                }
            }
            return lslItemFCAux;
            // try
            // {
            /*
                var lstFluxo = web.Lists["Fluxo de Caixa"].GetItems().Cast<SPListItem>().ToList();
                var lstFluxoSPE = lstFluxo.Select(x => x["SPE"].ToString().Split('#').LastOrDefault()).ToList();
                var num = lstFluxo.Count();
          

                foreach (SPListItem fluxo in lstFluxo)
                {
                    if (fluxo["SPE"].ToString().Split('#').LastOrDefault().ToLower().Trim() == itemSPE.Title.ToLower().Trim())
                           {
                        SPQuery query = new SPQuery();
                        query.Query = "<Where><And><Eq><FieldRef Name='N_x00ed_vel_x0020_1_x0020_FC' /><Value Type='Lookup'>(-) Investimentos</Value></Eq><Eq><FieldRef Name='FC' /><Value Type='Lookup'>" + fluxo.Title + "</Value></Eq></And></Where>";
                       // query.Query = "<Where><Eq><FieldRef Name='FC' /><Value Type='Lookup'>" + fluxo.Title + "</Value></Eq></Where>";
                        //query.Query = "<Where><Eq><FieldRef Name='N_x00ed_vel_x0020_1_x0020_FC' /><Value Type='Lookup'>(-) Investimentos</Value></Eq></Where>";
                        
                        var collectionFC = web.Lists["Item FC"].GetItems(query).Cast<SPListItem>().ToList();

                        foreach (var item in collectionFC)
                        {
                            lstItem.Add(item);
                        }
                    }
                }
                */
            // }
            // catch (Exception)
            // {
            //lstItem = null;

            //   }
            
        }
    }
}
