using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public class ParticipacaoSPEService
    {
        public string SetParticipacao(List<ParticipacaoSPE> participacaoSPE)
        { 
            string retorno = string.Empty;
            //try
            //{

            if (participacaoSPE != null && participacaoSPE.Count() > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < participacaoSPE.Count(); j++)
                    {
                        sb.AppendFormat("{0}  {1}%; ", participacaoSPE[j].Empresa,participacaoSPE[j].Participacao);
                        sb.AppendLine();
                    }

                    // empresas = itemParticipacao["Empresa_x003a_Nome_x0020_da_x002"]; esse campo existe em SPE produção

                    sb.AppendLine();
                    //itemGovernanca["ParticipacoesSocios"] = ;
                    retorno = sb.ToString();
                }

           // }
           // catch (Exception)
            //{
             

          //  }
             
            return retorno;

        }
    }
}
