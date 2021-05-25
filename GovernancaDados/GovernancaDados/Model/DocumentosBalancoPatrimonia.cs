using FURNAS.GestaoSPE.PersistenciaGovernanca.Proxy;
using FURNAS.GestaoSPE.GovernancaDados.Utilities;
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
    public class DocumentosBalancoPatrimonia
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Tipo{ get; set; }
        public string LinkFilename { get; set; }
        public byte[]  Response    { get; set; }
        public DateTime Modified { get; set; }

        public DocumentosBalancoPatrimonia GetDocumentosBalancoPatrimonial(GovernancaDados.GovernancaConsolidaSPE.Lists client, SPE itemSPE, Usuario usuario, string UrlSISPE, SPWeb web)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
          //  XmlNode listDocumentosBP = client.GetListItems("Documentos Balanço Patrimonial", null, null, viewFields, "20", null, null);
           XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");


           // ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='Tipo_x0020_Documento_x0020_Balan_x00e7_o_x0020_Patrimonial' /><Value Type='Lookup'>Demonstrações Financeiras</Value></Eq></Where><OrderBy><FieldRef Name='Modified'' Ascending='False'/></OrderBy>";
           ndQuery.InnerXml = @"<Where><And><Eq><FieldRef Name='Tipo_x0020_Documento_x0020_Balan_x00e7_o_x0020_Patrimonial' LookupId='TRUE'/><Value Type='Lookup'>2</Value></Eq><Eq><FieldRef Name='SPE'  LookupId='TRUE' /> <Value Type='Lookup'>" + itemSPE.ID + "</Value></Eq></And></Where><OrderBy><FieldRef Name='Modified' Ascending='False'/></OrderBy>";

           XmlNode listDocumentosBP = client.GetListItems("Demonstrações Financeiras", null, ndQuery, viewFields, null, null, null);

  
           
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

                                if (item["ows_Modified"] != null)
                                    documentosBP.Modified = Convert.ToDateTime(item["ows_Modified"].Value);

                                lsl.Add(documentosBP);
                            }
                        }
                    }
                }
            }
            DocumentosBalancoPatrimonia ultimoBP = lsl.Count() > 0 ?  lsl.FirstOrDefault() : null;
            try
            {
                if (ultimoBP != null)
                {
                    string docUrl = UrlSISPE + "/Documentos Documento Balano Patrimonial/" + ultimoBP.LinkFilename;

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
            }
            catch(Exception ex)
            {
                LogHelper.GravarLog(web, ex, "Erro Download Documento BP");
            }
            return ultimoBP;
           
        }
    }
}
