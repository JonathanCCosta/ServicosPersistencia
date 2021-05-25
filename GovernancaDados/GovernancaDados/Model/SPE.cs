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
    public class SPE  
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string CNPJ { get; set; }
        public string Codigo { get; set; }
        public DateTime? DataConstituicao { get; set; }
        public Complexo Complexo { get; set; }

        public List<SPE> GetListSPEByComplexo(GovernancaDados.GovernancaConsolidaSPE.Lists client, Usuario usuario, Complexo complexo)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
         

            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='Complexo'  LookupId='TRUE' /> <Value Type='Lookup'>" + complexo.ID + "</Value></Eq></Where>";
            XmlNode listItems = client.GetListItems("SPE", null, null, viewFields, null, null, null);
            List<SPE> lsl = new List<SPE>();
            foreach (XmlNode node in listItems)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                            XMLHelper<SPE> XMLPersonHelperSPE = new XMLHelper<SPE>();

                            if (Convert.ToInt32(Convert.ToString(item["ows_Complexo"].Value).Split(';').FirstOrDefault()) ==complexo.ID)
                            {
                                SPE spe = new SPE();

                                if (item["ows_ID"] != null)
                                    spe.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    spe.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_CNPJ"] != null)
                                    spe.CNPJ = Convert.ToString(item["ows_CNPJ"].Value);

                                if (item["ows_Data_x0020_de_x0020_Constitui_x0"] != null)
                                    spe.DataConstituicao = Convert.ToDateTime(Convert.ToString(item["ows_Data_x0020_de_x0020_Constitui_x0"].Value));

                                if (item["ows_CNPJ"] != null)
                                    spe.CNPJ = Convert.ToString(item["ows_CNPJ"].Value);

                                if (item["ows_Complexo"] != null && item["ows_Complexo"].Value != null)
                                {

                                    spe.Complexo = new Complexo();
                                    spe.Complexo.ID = Convert.ToInt32(Convert.ToString(item["ows_Complexo"].Value).Split(';').FirstOrDefault());
                                    spe.Complexo.Title = Convert.ToString(item["ows_Complexo"].Value).Split('#').LastOrDefault();
                                }

                                lsl.Add(spe);
                            }
                        }
                    }
                }
            }
            return lsl;


        }

        public List<SPE> GetListSPEByID(GovernancaDados.GovernancaConsolidaSPE.Lists client, int id, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");


            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='ID'  /> <Value Type='Counter'>" + id + "</Value></Eq></Where>";
            XmlNode listItems = client.GetListItems("SPE", null, null, viewFields, null, null, null);
            List<SPE> lsl = new List<SPE>();
            foreach (XmlNode node in listItems)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                            XMLHelper<SPE> XMLPersonHelperSPE = new XMLHelper<SPE>();

                            if (Convert.ToInt32(Convert.ToString(item["ows_ID"].Value).Split(';').FirstOrDefault()) == id)
                            {
                                SPE spe = new SPE();

                                if (item["ows_ID"] != null)
                                    spe.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    spe.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_CNPJ"] != null)
                                    spe.CNPJ = Convert.ToString(item["ows_CNPJ"].Value);

                                if (item["ows_Data_x0020_de_x0020_Constitui_x0"] != null)
                                    spe.DataConstituicao = Convert.ToDateTime(Convert.ToString(item["ows_Data_x0020_de_x0020_Constitui_x0"].Value));

                                if (item["ows_CNPJ"] != null)
                                    spe.CNPJ = Convert.ToString(item["ows_CNPJ"].Value);

                                if (item["ows_Complexo"] != null && item["ows_Complexo"].Value != null)
                                {

                                    spe.Complexo = new Complexo();
                                    spe.Complexo.ID = Convert.ToInt32(Convert.ToString(item["ows_Complexo"].Value).Split(';').FirstOrDefault());
                                    spe.Complexo.Title = Convert.ToString(item["ows_Complexo"].Value).Split('#').LastOrDefault();
                                }

                                lsl.Add(spe);
                            }
                        }
                    }
                }
            }
            return lsl;


        }
        public List<SPE> GetListSPE(GovernancaDados.GovernancaConsolidaSPE.Lists client, Usuario usuario)
        {
           
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            client.Timeout = 360000;
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode listItems = client.GetListItems("SPE", null, null, viewFields, null, null, null);
            
            List<SPE> lsl = new List<SPE>();
            foreach (XmlNode node in listItems)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                            XMLHelper<SPE> XMLPersonHelperSPE = new XMLHelper<SPE>();
                            SPE spe = new SPE();

                            if (item["ows_ID"] != null)
                                spe.ID =  Convert.ToInt32(item["ows_ID"].Value);

                            if (item["ows_Title"] != null)
                                spe.Title = Convert.ToString(item["ows_Title"].Value);

                            if (item["ows_CNPJ"] != null)
                                spe.CNPJ = Convert.ToString(item["ows_CNPJ"].Value);

                            if (item["ows_Data_x0020_de_x0020_Constitui_x0"] != null)                       
                                spe.DataConstituicao =  Convert.ToDateTime(Convert.ToString(item["ows_Data_x0020_de_x0020_Constitui_x0"].Value));

                            if (item["ows_CNPJ"] != null)
                                spe.CNPJ = Convert.ToString(item["ows_CNPJ"].Value);

                            if (item["ows_Complexo"] != null && item["ows_Complexo"].Value != null)
                            {
                                 
                                spe.Complexo = new Complexo();
                                spe.Complexo.ID =   Convert.ToInt32(Convert.ToString(item["ows_Complexo"].Value).Split(';').FirstOrDefault());
                                spe.Complexo.Title = Convert.ToString(item["ows_Complexo"].Value).Split('#').LastOrDefault();   
                            }

                            lsl.Add(spe);
                        }
                    }
                }
            }
            return lsl;

        }

        public List<SPListItem> GetListSPE(SPWeb web)
        {

            SPList list = web.Lists["SPE"];

            SPQuery query = new SPQuery();
            query.ViewFields = string.Concat(
                          "<FieldRef Name='Complexo' />",/// ou Grupo
                           "<FieldRef Name='Title' />",
                          "<FieldRef Name='CNPJ' />",
                          "<FieldRef Name='Endereco' />",
                          "<FieldRef Name='Data_x0020_de_x0020_Constitui_x0' />",
                           "<FieldRef Name='ParticipacoesSocios' />",
                          "<FieldRef Name='ConselhoAdministracao' />",
                          "<FieldRef Name='Estagio' />",
                          "<FieldRef Name='Site' />",
                               "<FieldRef Name='ImagemSPE' />",
                          "<FieldRef Name='Investimento' />",
                          "<FieldRef Name='DataEntradaOperacaoPlena' />",
                          "<FieldRef Name='DataConstituicao' />"
                          );


             
            
            return list.GetItems(query).Cast<SPListItem>().ToList();

        }


        public List<SPListItem> GetSPEGOV(string urlGovernanca, SPE spe)
        {
            SPListItem SPEs = null;
            // Documentos Balanço Patrimonial 
            List<SPListItem> lsl = new List<SPListItem>();
            try
            {
                using (SPSite site = new SPSite(urlGovernanca))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        SPQuery query = new SPQuery();
                        query.Query = @"<Where><Eq><FieldRef Name='Title' /> <Value  Type='Text'>" + spe.Title + "</Value></Eq></Where>";
                        SPEs = web.Lists["SPE"].GetItems(query).Cast<SPListItem>().FirstOrDefault();

                        if(SPEs != null)
                            lsl.Add(SPEs);
                    }
                }
            }
            catch (Exception e)
            {

            }

           return  lsl;

        }

    }
}
