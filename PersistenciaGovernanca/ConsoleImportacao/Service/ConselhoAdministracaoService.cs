using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public  class ConselhoAdministracaoService
    {
        public string SetConselhoAdministracao(List<ConselhoAdministracao> conselhoAdministracaos)
        {
            string retorno = string.Empty;
           // try
            //{
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < conselhoAdministracaos.Count(); j++)
                    sb.AppendFormat("{0}, ", conselhoAdministracaos[j].Nome);

                sb.AppendLine();
                //itemGovernanca["ConselhoAdministracao"] = sb.ToString();
                retorno = sb.ToString();
           // }
            //catch (Exception)
           // {
                //retorno = null;
           // }
            return retorno;
        }
    }
}
