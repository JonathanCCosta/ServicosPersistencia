using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public class FluxodeCaixaService
    {
        public Double SetItemFluxodeCaixa(List<SPListItem> lstItemFluxo)
        {
            double valor = 0;
          //  try
          //  {
                foreach (var item in lstItemFluxo)
                {
                    valor = valor + Convert.ToDouble(item["Valor"]);
                }
          //  }
          //  catch (Exception)
          //  {

                return valor;
          //  }
            return valor;
        }
    }
}
