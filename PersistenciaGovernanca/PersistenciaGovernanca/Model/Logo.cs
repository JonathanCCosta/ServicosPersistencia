
using FURNAS.GestaoSPE.PersistenciaGovernanca.Proxy;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Model
{
    public class Logo
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string LinkFilename { get; set; }
        public byte[] Response { get; set; }

        public List<Logo> GetLogo(GovernancaConsolidaSPE.Lists client,string urlGovernanca, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode listLogo = client.GetListItems("Logo SPE", null, null, viewFields, "100", null, null);
            client.Timeout = 30000;

            Logo logo = null;
            List<Logo> lsl = new List<Logo>();
            foreach (XmlNode node in listLogo)
            {
                try
                {
                    if (node.Name == "rs:data")
                    {
                        for (int f = 0; f < node.ChildNodes.Count; f++)
                        {
                            if (node.ChildNodes[f].Name == "z:row")
                            {
                                XMLHelper<Logo> XMLPersonHelper = new XMLHelper<Logo>();
                                XmlAttributeCollection item = node.ChildNodes[f].Attributes;

                                var tipoBP = string.Empty;

                                logo = new Logo();
                                logo.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    logo.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_LinkFilename"] != null)
                                    logo.LinkFilename = Convert.ToString(item["ows_LinkFilenameNoMenu"].Value);

                                // string docUrl = "http://shpt12:160/secretariagovernanca/Style%20Library/Styles/Images/ImagesHome/portal_banner.gif";
                                //string docUrl = "http://www.sisspehml.cepel.br/dnnet/sp/gep/gestaospe/Logo%20SPE/BAGUARI.jpg";
                                string docUrl = urlGovernanca + "/Logo%20SPE/" + logo.LinkFilename;
                                using (WebClient wc = new WebClient())
                                {
                                    // wc.UseDefaultCredentials = true;
                                    CredentialCache cc = new CredentialCache();
                                    cc.Add(
                                        new Uri(docUrl),
                                        "NTLM",
                                        new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio));
                                    client.Credentials = cc;
                                    wc.Credentials = cc;
                                    byte[] response = wc.DownloadData(docUrl);
                                    logo.Response = response;

                                }


                                lsl.Add(logo);

                            }
                        }
                    }
                }
                catch { }
            }

            return lsl;
        }


        public bool InsertAllLogo(List<Logo> logos, string urlGovernanca)
        {

            try
            {
                using (SPSite site = new SPSite(urlGovernanca))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        /// COMPLEXO E LOGO SPE 

                        SPList list = web.Lists.TryGetList("Logo SPE");


                        foreach (Logo logo in logos)
                        {

                            SPFileCollection filecol = list.RootFolder.Files;//getting all the files which is in pictire library
                            filecol.Add(logo.LinkFilename, logo.Response, true);//uploading the files to picturte library 
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
