using ConsoleImportacao.Proxy;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleImportacao
{
    public class DocumentosBalancoPatrimonia
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Tipo{ get; set; }
        public string LinkFilename { get; set; }
        public byte[]  Response    { get; set; }

        public DocumentosBalancoPatrimonia GetDocumentosBalancoPatrimonial(GovernancaConsolidaSPE.Lists client, ConsoleImportacao.SPE itemSPE, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode listDocumentosBP = client.GetListItems("Documentos Balanço Patrimonial", null, null, viewFields, null, null, null);
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");


            ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='Tipo_x0020_Documento_x0020_Balan_x00e7_o_x0020_Patrimonial' /><Value Type='Lookup'>Demonstrações Financeiras</Value></Eq></Where><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy>";
            XmlNode listParticipacao = client.GetListItems("Documentos Balanço Patrimonial", null, ndQuery, viewFields, "1", null, null);


            
               //try
               //{
               //     XmlNode retNode = client.GetAttachmentCollection("Documentos Balanço Patrimonial", "786");
               //}
               //catch (System.Web.Services.Protocols.SoapException ex)
               //{

               //      var msg = "Message:\n" + ex.Message + "\nDetail:\n" + ex.Detail.InnerText + "\nStackTrace:\n" + ex.StackTrace;
               //}
           
           
           
            DocumentosBalancoPatrimonia documentosBP = null;
            List<DocumentosBalancoPatrimonia> lsl = new List<DocumentosBalancoPatrimonia>();
            foreach (XmlNode node in listDocumentosBP)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<DocumentosBalancoPatrimonia> XMLPersonHelper = new XMLHelper<DocumentosBalancoPatrimonia>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                        
                            var tipoBP = string.Empty;

                            if (item["ows_Tipo_x0020_Documento_x0020_Balan_x00e7_o_x0020_Patrimonial"] != null)
                                tipoBP = Convert.ToString(item["ows_Tipo_x0020_Documento_x0020_Balan_x00e7_o_x0020_Patrimonial"].Value).Split('#').LastOrDefault();

                            if (Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault()) == itemSPE.ID && tipoBP == "Demonstrações Financeiras")
                            {
                                documentosBP = new DocumentosBalancoPatrimonia();
                                documentosBP.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    documentosBP.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_Tipo_x0020_Documento_x0020_Balan_x00e7_o_x0020_Patrimonial"] != null)
                                    documentosBP.Tipo = tipoBP;

                                if (item["ows_LinkFilename"] != null)
                                    documentosBP.LinkFilename = Convert.ToString(item["ows_LinkFilename"].Value);

                                lsl.Add(documentosBP);
                            }
                        }
                    }
                }
            }
            DocumentosBalancoPatrimonia ultimoBP = lsl.Count() > 0 ?  lsl.LastOrDefault() : null;

            if (ultimoBP != null)
            {
                string docUrl = "http://www.sisspehml.cepel.br/dnnet/sp/gep/gestaospe/Documentos%20Documento%20Balano%20Patrimonial/" + ultimoBP.LinkFilename;

                using (WebClient wc = new WebClient())
                {
                    wc.UseDefaultCredentials = true;
              
                    CredentialCache cc = new CredentialCache();
                    cc.Add(
                        new Uri(docUrl),
                        "NTLM",
                        new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio));
                    client.Credentials = cc;
                    wc.Credentials = cc;
                    byte[] response = wc.DownloadData(docUrl);
                    ultimoBP.Response = response;
                }
            }
            return ultimoBP;
            /*
            SPListItem itemBP = null;
           // try
           // {
                SPQuery query = new SPQuery();
                query.Query = @"<Where><Eq><FieldRef Name='Tipo_x0020_Documento_x0020_Balan_x00e7_o_x0020_Patrimonial' /><Value Type='Lookup'>Demonstrações Financeiras</Value></Eq></Where><OrderBy><FieldRef Name='Modified' Ascending='True' /><FieldRef Name='Created' Ascending='True' /></OrderBy>";
             //  itemBP =   web.Lists["Documentos Balanço Patrimonial"].GetItems(query).Cast<SPListItem>().LastOrDefault();
            //}
           // catch (Exception)
            //{
                //itemBP = null;
           // }*/
           // return itemBP;
        }
    }
}
