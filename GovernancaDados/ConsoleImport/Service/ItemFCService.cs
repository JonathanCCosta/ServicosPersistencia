using ConsoleImportacao.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public class ItemFCService
    {
         public Double SetFluxodeCaixa(List<ItemFC> itemFC)
        {


            return itemFC.Sum(x =>  Math.Abs(Convert.ToDouble(x.Valor)));
           
            
        }
    }
   
}
