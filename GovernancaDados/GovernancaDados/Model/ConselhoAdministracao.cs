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
    public class ConselhoAdministracao
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Grupo { get; set; }
        public string Nome { get; set; }

        public List<ConselhoAdministracao> GetConselhoAdministracao(GovernancaDados.GovernancaConsolidaSPE.Lists client, SPE itemSPE, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            client.Timeout = System.Threading.Timeout.Infinite;

            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = @"<Where><And><Eq><FieldRef Name='Grupo_x0020_da_x0020_Administra_' /><Value Type='Choice'>Conselho de Administração</Value></Eq><Eq><FieldRef Name='SPE' /> <Value Type='Lookup'>" + itemSPE.Title + "</Value></Eq></And></Where>";
            XmlNode listDiretoria = client.GetListItems("Agentes da Governança", null, ndQuery, viewFields, null, null, null);

            ConselhoAdministracao conselhoAdministracao = null;
            List<ConselhoAdministracao> lsl = new List<ConselhoAdministracao>();
            foreach (XmlNode node in listDiretoria)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<ConselhoAdministracao> XMLPersonHelper = new XMLHelper<ConselhoAdministracao>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;
                            string diretoriaGrupo = string.Empty;
                            int idSPE = Convert.ToInt32(Convert.ToString(item["ows_SPE"].Value).Split(';').FirstOrDefault());

                            if (item["ows_Grupo_x0020_da_x0020_Administra_"] != null)
                                diretoriaGrupo = Convert.ToString(item["ows_Grupo_x0020_da_x0020_Administra_"].Value).Split('#').LastOrDefault().ToLower().Trim();

                            if (idSPE == itemSPE.ID && diretoriaGrupo == "conselho de administração")
                            {
                                conselhoAdministracao = new ConselhoAdministracao();
                                conselhoAdministracao.ID = Convert.ToInt32(item["ows_ID"].Value);

                                if (item["ows_Title"] != null)
                                    conselhoAdministracao.Title = Convert.ToString(item["ows_Title"].Value);

                                if (item["ows_Nome"] != null)
                                    conselhoAdministracao.Nome = Convert.ToString(item["ows_Nome"].Value).Split('#').LastOrDefault();

                                if (item["ows_Grupo_x0020_da_x0020_Administra_"] != null)
                                    conselhoAdministracao.Grupo = Convert.ToString(item["ows_Grupo_x0020_da_x0020_Administra_"]);

                                lsl.Add(conselhoAdministracao);
                            }
                        }
                    }
                }
            }
            return lsl;
            /*
            List<SPListItem> itemConselho = null;
           // try
            //{
                StringBuilder sb = new StringBuilder();
                SPQuery query = new SPQuery();

                query.Query = @"<Where><And><Eq><FieldRef Name='Grupo_x0020_da_x0020_Administra_' /><Value Type='Choice'>Conselho de Administração</Value></Eq><Eq><FieldRef Name='SPE' /> <Value Type='Lookup'>" + itemSPE.Title + "</Value></Eq></And></Where>";
              //  itemConselho = web.Lists["Agentes da Governança"].GetItems(query).Cast<SPListItem>().ToList();
            //}
            //catch (Exception)
            //{
                //itemConselho = null;

            //}
            return itemConselho;*/
        } 
    }
}
