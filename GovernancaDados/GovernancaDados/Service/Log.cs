using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Service
{
    public class Log
    {
        public string Titulo { get; set; }
        public string Coluna { get; set; }
        public Exception Excecao { get; set; }
        public string Classe { get; set; }
        public string Linha { get; set; }
        public Exception Comentario { get; set; }
        public string url { get; set; }
        public Exception Exception { get; set; }
        public void GravarLog(SPWeb web,Log log)
        {
            SPList oList = web.Lists["log"];
            SPListItem ListItem = oList.Items.Add();
            ListItem["Title"] = log.Titulo;
            ListItem["coluna"] = log.Coluna;
            ListItem["excecao"] = log.Excecao;
            ListItem["classe"] = log.Classe;
            ListItem["linha"] = log.Linha;
            ListItem["comentario"] = log.Comentario;
            ListItem["url"] = log.url;
            ListItem.Update();

        }
    }
}
