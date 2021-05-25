using ConsoleImportacao.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public class DiretoriaService
    {
        public string SetDiretoria(List<Diretoria> diretorias)
        {
            string retorno = string.Empty;
            StringBuilder sb = new StringBuilder();
           // try
            //{
            for (int j = 0; j < diretorias.Count(); j++)
                sb.AppendFormat("{0}, ", diretorias[j].Nome);

                sb.AppendLine();
                // itemGovernanca["Diretoria"] = sb.ToString();
                retorno = sb.ToString();

           // }
           // catch (Exception)
            //{
               // retorno = null;

          //  }
            return retorno;
        }
    }
}
