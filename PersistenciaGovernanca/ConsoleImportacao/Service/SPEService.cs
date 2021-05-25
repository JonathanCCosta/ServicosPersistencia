using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public class SPEService
    {

        public void DeletarSPEs(SPWeb web)
        {
            SPList list = web.Lists["SPE"];
           SPListItemCollection lstSPE = list.GetItems();
           foreach (SPListItem  item in lstSPE)
           { 
               item.Delete();
           }
           list.Update();
        }

        
    }
}
