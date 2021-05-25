using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Service
{
    public class AtividadesService
    {

        public KeyValuePair<int, DateTime?> CompararData(KeyValuePair<int, DateTime?> itemAtual, KeyValuePair<int, DateTime?> itemNovo)
        {
            if (itemNovo.Key == 0) return itemAtual;
            //if (itemNovo.Key == 0 && itemAtual.Key != 0) return itemAtual;
            if (itemAtual.Key > itemNovo.Key)
            {
                if (itemAtual.Value > itemNovo.Value)
                {
                    itemNovo = new KeyValuePair<int, DateTime?>(itemAtual.Key, itemAtual.Value);
                }
            }
            else
            {
                if (itemAtual.Key == itemNovo.Key)
                {
                    if (itemAtual.Value > itemNovo.Value)
                    {
                        itemNovo = new KeyValuePair<int, DateTime?>(itemAtual.Key, itemAtual.Value);
                    }
                }
            }

            return itemNovo;
        }





      /*  public DateTime? SetAtividades(List<Atividades> atividades)
        {
            DateTime? retorno = new DateTime();
            //try
           // {

            foreach (Atividades atividade in atividades)
                {
                    DateTime? maior =null;
                    // Convert.ToDateTime(Convert.ToString(atividades["Data_x0020_In_x00ed_cio_x0020_AN"]));
                   // Convert.ToDateTime(Convert.ToString(atividades["Data_x0020_In_x00ed_cio_x0020_Re"]));
                    // Convert.ToDateTime(Convert.ToString(atividades["Data_x0020_In_x00ed_cio_x0020_Re0"]));
                    if (atividade != null)
                    {
                        if (atividade.DataTerminoANEEL != null && atividade.DataTerminoREPROGROGRAMADO != null)
                        {
                            if (atividade.DataTerminoANEEL != null && atividade.DataTerminoREPROGROGRAMADO == null)
                                maior = atividade.DataTerminoANEEL;

                            if (atividade.DataTerminoANEEL == null && atividade.DataTerminoREPROGROGRAMADO != null)
                                maior = atividade.DataTerminoREPROGROGRAMADO;

                            if (maior == null)
                            {
                                if (atividade.DataTerminoANEEL > atividade.DataTerminoREPROGROGRAMADO)
                                    maior = atividade.DataTerminoANEEL;
                                else
                                    maior = atividade.DataTerminoREPROGROGRAMADO;
                            }
                        }
                        if (atividade.DataTerminoREALIZADO != null && maior == null)
                            maior = atividade.DataTerminoREALIZADO;

                        if (atividade.DataTerminoREALIZADO != null)
                        {
                            if (atividade.DataTerminoREALIZADO > maior)
                                maior = atividade.DataTerminoREALIZADO;

                        }

                        //itemGovernanca["DataConstituicao"] = Convert.ToDateTime(maior);
                        retorno = maior;
                    }
                    else
                    {
                        retorno = null;
                    }
                }
                                 
           // }
           // catch (Exception e)
           // {
                return retorno;             

           // }
                   
         }*/
    }
}
