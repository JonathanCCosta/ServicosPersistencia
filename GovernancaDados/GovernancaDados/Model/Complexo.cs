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
    public class Complexo
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Logo { get; set; }
        public int LogoID { get; set; }
        public string Descricao { get; set; }
        public int IDComplexoGov { get; set; }
        public int ID_Complexo { get; set; }
        public List<Complexo> GetALLComplexosGov(GovernancaDados.GovernancaConsolidaSPE.Lists client, string urlGovernanca, Usuario usuario)
        {
            List<Complexo> complstlexos = new List<Complexo>();

            // Documentos Balanço Patrimonial 
            try
            {
                using (SPSite site = new SPSite(urlGovernanca))
                {
                    using (SPWeb web = site.OpenWeb())
                    {                       
                        Complexo complexo = null;
                        SPListItemCollection complexos = web.Lists["Complexo SPE"].GetItems();

                        foreach (SPListItem item in complexos)
                        {
                            complexo = new Complexo();
                            complexo.ID = Convert.ToInt32(item["ID"]);
                            complexo.ID_Complexo = Convert.ToInt32(item["ID"]);
                            if (item["Title"] != null)
                                complexo.Title = Convert.ToString(item["Title"]);

                         /*   if (item["Logo"] != null)
                            {
                                var nomeLogo = Convert.ToString(item["Logo"]).Split('#').LastOrDefault();
                                var urLogo = urlGovernanca + "/LogoSPE/" + nomeLogo.Split('/').LastOrDefault(); ;
                                complexo.Logo = urLogo;
                                complexo.LogoID = Convert.ToInt32(Convert.ToString(item["Logo"]).Split(';').FirstOrDefault());
                            }*/

                            if (item["DescricaoComplexo"] != null)
                                complexo.Descricao = Convert.ToString(item["DescricaoComplexo"]);

                            complstlexos.Add(complexo);
                        }


                    }
                }
            }
            catch (Exception e)
            {

            }



            return complstlexos;

        }

        public List<Complexo> GetALLComplexos(GovernancaDados.GovernancaConsolidaSPE.Lists client, string urlGovernanca, Usuario usuario)
        {

            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode listComplexo = client.GetListItems("Grupo", null, null, viewFields, null, null, null);
            List<Complexo> lsl = new List<Complexo>();

            Complexo complexo = null;
            foreach (XmlNode node in listComplexo)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<Complexo> XMLPersonHelper = new XMLHelper<Complexo>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;

                            complexo = new Complexo();
                            complexo.ID = Convert.ToInt32(item["ows_ID"].Value);
                            complexo.ID_Complexo = Convert.ToInt32(item["ows_ID"].Value);
                            if (item["ows_Title"] != null)
                                complexo.Title = Convert.ToString(item["ows_Title"].Value);

                            if (item["ows_Logo"] != null)
                            {
                                var nomeLogo = Convert.ToString(item["ows_Logo"].Value).Split('#').LastOrDefault();
                                var urLogo = urlGovernanca + "/LogoSPE/" + nomeLogo.Split('/').LastOrDefault(); ;

                                complexo.Logo = urLogo;
                                complexo.LogoID = Convert.ToInt32(Convert.ToString(item["ows_Logo"].Value).Split(';').FirstOrDefault());



                            }

                            if (item["ows_Descri_x00e7__x00e3_o"] != null)
                                complexo.Descricao = Convert.ToString(item["ows_Descri_x00e7__x00e3_o"].Value);

                            lsl.Add(complexo);

                        }
                    }
                }
            }


            return lsl;

        }
        public Complexo GetComplexo(GovernancaDados.GovernancaConsolidaSPE.Lists client, SPE itemSPE, string urlGovernanca, Usuario usuario)
        {

            /// COMPLEXO E LOGO SPE 

            //SPListItem itemComplexo=null;

            //var idComplexoSPE = Convert.ToString(itemSPE["Complexo"]).Split(';').FirstOrDefault();
            // return  web.Lists["Grupo"].GetItemById(Convert.ToInt32(idComplexoSPE));

            //var idComplexo = Convert.ToString(itemGovernanca["Complexo"]).Split(';').FirstOrDefault();
            // new SPFieldLookupValue(itemComplexo.Title);


            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode listComplexo = client.GetListItems("Grupo", null, null, viewFields, null, null, null);

            Complexo complexo = null;
            foreach (XmlNode node in listComplexo)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<Complexo> XMLPersonHelper = new XMLHelper<Complexo>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;

                            if (Convert.ToInt32(item["ows_ID"].Value) == itemSPE.Complexo.ID)
                            {
                                complexo = new Complexo();
                                complexo.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    complexo.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_Logo"] != null)
                                {
                                    complexo.Logo = Convert.ToString(item["ows_Logo"].Value);
                                    complexo.LogoID = Convert.ToInt32(Convert.ToString(item["ows_Logo"].Value).Split(';').FirstOrDefault());
                                }

                                if (item["ows_Descri_x00e7__x00e3_o"] != null)
                                    complexo.Descricao = Convert.ToString(item["ows_Descri_x00e7__x00e3_o"].Value);

                                // complexo.IDComplexoGov = GetIDComplexoGOV(urlGovernanca, complexo);

                            }
                        }
                    }
                }
            }
            return complexo;


            //  return itemComplexo;

        }

        public int GetIDComplexoGOV(string urlGovernanca, Complexo complexo)
        {
            SPListItem complexos = null;
            // Documentos Balanço Patrimonial 
            try
            {
                using (SPSite site = new SPSite(urlGovernanca))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        SPQuery query = new SPQuery();
                        query.Query = @"<Where><Eq><FieldRef Name='Title' /> <Value  Type='Text'>" + complexo.Title + "</Value></Eq></Where>";
                        complexos = web.Lists["Complexo SPE"].GetItems(query).Cast<SPListItem>().FirstOrDefault();
                    }
                }
            }
            catch (Exception e)
            {

            }

            return complexos != null ? complexos.ID : 0;

        }

        public bool InsertAllComplexoGOV(List<Complexo> complexos, string urlGovernanca)
        {

            try
            {
                using (SPSite site = new SPSite(urlGovernanca))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        /// COMPLEXO E LOGO SPE 

                        SPList list = web.Lists.TryGetList("Complexo SPE");


                        foreach (Complexo complexo in complexos)
                        {
                            SPListItem NewItem = list.Items.Add();
                            {
                                web.AllowUnsafeUpdates = true;

                                NewItem["Title"] = complexo.Title;
                                NewItem["DescricaoComplexo"] = complexo.Descricao;
                                NewItem["ID_Complexo"] = complexo.ID;
                                NewItem.Update();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }


            return true;
        }
    }
}
