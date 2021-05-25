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
    public class Atividades
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Grupo { get; set; }
        public DateTime? DataTerminoANEEL { get; set; }
        public DateTime? DataTerminoREPROGROGRAMADO { get; set; }
        public DateTime? DataTerminoREALIZADO { get; set; }
        public int PesoDataTermino { get; set; }
        public Dictionary<int, DateTime?> PesoData { get; set; }


       public  Atividades()
        {
            this.DataTerminoANEEL = null;
            this.DataTerminoREALIZADO = null;
            this.DataTerminoREPROGROGRAMADO = null;
        }

        public KeyValuePair<int, DateTime?> GetDataMaiorAtividade(GovernancaConsolidaSPE.Lists client, Obra obra, Usuario usuario)
        {
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
           
            client.Credentials = new NetworkCredential(usuario.Login, usuario.Senha, usuario.Dominio);
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = @"<Where><Eq><FieldRef Name='Obra'  LookupId='TRUE' /> <Value Type='Lookup'>" + obra.ID + "</Value></Eq></Where>";
            XmlNode listAtividade = client.GetListItems("Atividade", null, ndQuery, viewFields, "1000", null, null);

            Atividades atividade = null;
            PesoData = new Dictionary<int, DateTime?>();
            DateTime? maior = null;
            List<Atividades> lsl = new List<Atividades>();
            foreach (XmlNode node in listAtividade)
            {
                if (node.Name == "rs:data")
                {
                    for (int f = 0; f < node.ChildNodes.Count; f++)
                    {
                        if (node.ChildNodes[f].Name == "z:row")
                        {
                            XMLHelper<Atividades> XMLPersonHelper = new XMLHelper<Atividades>();
                            XmlAttributeCollection item = node.ChildNodes[f].Attributes;

                            atividade = new Atividades();
                          
                            var atividadeMacro = item["ows_Title"] != null ? Convert.ToString(item["ows_Title"].Value) : string.Empty;
                            var grupo = item["ows_Grupo"] != null ? Convert.ToString(item["ows_Grupo"].Value).Split('#').LastOrDefault() : string.Empty;
                            var obraAtividade = item["ows_Obra"] != null ? Convert.ToString(item["ows_Obra"].Value).Split('#').LastOrDefault() : string.Empty;

                            if (Convert.ToInt32(item["ows_ID"].Value) == 659)
                            {


                            }
                            if (atividadeMacro == "ENTRADA EM OPERAÇÃO" && "DATAS PARA INDICADORES E RELATÓRIOS" == grupo)
                            {
                                atividade.Title = Convert.ToString(item["ows_Title"].Value);
                                if (obra.Title.ToLower().Trim() == obraAtividade.ToLower().Trim())
                                {
                                    atividade.ID = Convert.ToInt32(item["ows_ID"].Value);

                                    if (item["ows_Data_x0020_T_x00e9_rmino_x0020_A"] != null)
                                        atividade.DataTerminoANEEL = Convert.ToDateTime(item["ows_Data_x0020_T_x00e9_rmino_x0020_A"].Value);

                                    if (item["ows_Data_x0020_T_x00e9_rmino_x0020_R"] != null)
                                        atividade.DataTerminoREPROGROGRAMADO = Convert.ToDateTime(item["ows_Data_x0020_T_x00e9_rmino_x0020_R"].Value);

                                    if (item["ows_Data_x0020_Fim_x0020_Realizado"] != null)
                                        atividade.DataTerminoREALIZADO = Convert.ToDateTime(item["ows_Data_x0020_Fim_x0020_Realizado"].Value);

                                    if (atividade.DataTerminoREALIZADO != null)
                                    {
                                        PesoDataTermino = 3;
                                        maior = atividade.DataTerminoREALIZADO;

                                    }
                                    else
                                    {
                                        if (atividade.DataTerminoREPROGROGRAMADO != null)
                                        {

                                            PesoDataTermino = 2;
                                            maior = atividade.DataTerminoREPROGROGRAMADO;
                                        }
                                        else if (atividade.DataTerminoANEEL != null)
                                        {
                                            PesoDataTermino = 1;
                                            maior = atividade.DataTerminoANEEL;
                                        }     
                                    }


                                    if (PesoData.Count() == 0)
                                    {
                                        PesoData.Add(PesoDataTermino, maior);
                                    }
                                    else if ( PesoData.TryGetValue(PesoDataTermino, out maior))
                                    { 
                                        if (maior.Value >PesoData.LastOrDefault().Value)
                                            PesoData.Add(PesoDataTermino, maior);
                                    }

                                   

                                    lsl.Add(atividade);
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }



            var sortedDict = from entry in PesoData orderby entry.Key ascending select entry;
            var sortedDict2 = from entry in PesoData orderby entry.Value ascending select entry;
            return PesoData.OrderBy(x => x.Key).OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value).FirstOrDefault();
          //  var  maiorDicionario = PesoData.OrderBy(x => x.Key).OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value).FirstOrDefault();
             //  Dictionary<int, DateTime?> retorno =  new Dictionary<int, DateTime?>();
             //  retorno.Add(maiorDicionario.Key, maiorDicionario.Value);
              // return retorno;
           // return PesoData.OrderBy(x => x.Key).OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
         
        
           
         }


    }
}
