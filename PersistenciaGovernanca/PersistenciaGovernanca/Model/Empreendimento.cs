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
    public class Empreendimento
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Descricao { get; set; }
        public string FaseEmpreendimento { get; set; }
        public int FaseEmpreendimentoID { get; set; }
        public List<Obra> Obra { get; set; }
        public List<string> FaseComplexo { get; set; }
        public DateTime Criado { get; set; }


        public KeyValuePair<int, DateTime?> PesoData { get; set; }
        public SPE SPE { get; set; }
        public List<Empreendimento> GetEmpreendimentos(List<Empreendimento> empreendimentos, List<SPE> spesPorComplexo)
        {
            List<int> idsSPE = spesPorComplexo.Select(x => x.ID).ToList();

           return empreendimentos.Where(x => x.SPE != null && idsSPE.Contains(x.SPE.ID)).ToList();
             
        }

        public List<Empreendimento> GetEmpreendimentos(GovernancaConsolidaSPE.Lists client,Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            client.Timeout =  360000;
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            string query = string.Empty;           
          
            XmlNode listEmpreendimento = client.GetListItems("Empreendimento", null, null, viewFields, "10000", null, null);
            Empreendimento empreendimento = null;
            List<Empreendimento> lsl = new List<Empreendimento>();
            foreach (XmlNode node in listEmpreendimento)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<Empreendimento> XMLPersonHelper = new XMLHelper<Empreendimento>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;

                            var ows_SPE = item["ows_SPE"] != null ? Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault()) : 0;

                                if (ows_SPE != 0)
                                {                                 
                                    empreendimento = new Empreendimento();
                                    empreendimento.ID = Convert.ToInt32(item["ows_ID"].Value);

                                    if (item["ows_Title"] != null)
                                        empreendimento.Title = Convert.ToString(item["ows_Title"].Value);

                                    if (item["ows_Descri_x00e7__x00e3_o"] != null)
                                        empreendimento.Descricao = Convert.ToString(item["ows_Descri_x00e7__x00e3_o"].Value);

                                    if (item["ows_Fase_x0020_Empreendimento"] != null)
                                    {
                                        empreendimento.FaseEmpreendimentoID = Convert.ToInt32(Convert.ToString(item["ows_Fase_x0020_Empreendimento"].Value).Split(';').FirstOrDefault());
                                        empreendimento.FaseEmpreendimento = Convert.ToString(item["ows_Fase_x0020_Empreendimento"].Value).Split('#').LastOrDefault();                                     
                                    }
                                    if (item["ows_SPE"] != null)
                                    {

                                        empreendimento.SPE = new SPE();
                                        empreendimento.SPE.ID = Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault());
                                        empreendimento.SPE.Title = Convert.ToString(item["ows_SPE"].Value).Split('#').LastOrDefault();
                                    }
                                    lsl.Add(empreendimento);
                                }                                 
                            }
                        }
                    }
                }
              return lsl;
        }

        public  Empreendimento GetEmpreendimento(GovernancaConsolidaSPE.Lists client, SPE itemSPE, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='SPE' LookupId='TRUE'/> <Value Type='Lookup'>" + itemSPE.ID + "</Value></Eq></Where>";
            XmlNode listEmpreendimento = client.GetListItems("Empreendimento", null, ndQuery, viewFields, null, null, null);

            Empreendimento empreendimento = null;
            List<Empreendimento> lsl = new List<Empreendimento>();
            foreach (XmlNode node in listEmpreendimento)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<Empreendimento> XMLPersonHelper = new XMLHelper<Empreendimento>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;

                            var ows_SPE = item["ows_SPE"] != null ? Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault()) : 0;

                            if (ows_SPE != 0)
                            {
                                if (ows_SPE == Convert.ToInt32(itemSPE.ID))
                                {
                                    empreendimento = new Empreendimento();
                                    empreendimento.ID = Convert.ToInt32(item["ows_ID"].Value);

                                    if (item["ows_Title"] != null)
                                        empreendimento.Title = Convert.ToString(item["ows_Title"].Value);

                                    if (item["ows_Descri_x00e7__x00e3_o"] != null)
                                        empreendimento.Descricao = Convert.ToString(item["ows_Descri_x00e7__x00e3_o"].Value);

                                    if (item["ows_Fase_x0020_Empreendimento"] != null)
                                    {
                                        empreendimento.FaseEmpreendimentoID = Convert.ToInt32(Convert.ToString(item["ows_Fase_x0020_Empreendimento"].Value).Split(';').FirstOrDefault());
                                        empreendimento.FaseEmpreendimento = Convert.ToString(item["ows_Fase_x0020_Empreendimento"].Value).Split('#').LastOrDefault();


                                    }

                                    if (item["ows_SPE"] != null )
                                    {

                                        empreendimento.SPE = new SPE();
                                        empreendimento.SPE.ID = Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault());
                                        empreendimento.SPE.Title = Convert.ToString(item["ows_SPE"].Value).Split('#').LastOrDefault();
                                    }
                                   // lsl.Add(empreendimento);
                                }
                            }
                        }
                    }
                }
            }
            return empreendimento;
        }
    }
}
