using ConsoleImportacao.Service;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Utilities
{
    public static class LogHelper
    {
        

        public static void GravarLog(SPWeb web, Exception e)
        {
             
            SPList oList = web.Lists["log"];
            SPListItem ListItem = oList.Items.Add();

            string excecao = "StackTrace nula";

            ListItem["Title"] ="Programa Principal";
          
            if (e.InnerException != null && e.InnerException.Message != null)
                ListItem["excecao"] = e.InnerException.Message;

            if (e.InnerException != null && e.InnerException.StackTrace != null)
            {
                ListItem["comentario"] = e.InnerException.StackTrace;
                excecao = e.InnerException.StackTrace;
            }

            ListItem["url"] = web.Url;
            ListItem.Update();
            Email.SendEmail(web, string.Format("Verificar na lista de Log o erro com ID {0} com a seguinte Excecao: <br/>{1}", ListItem.ID.ToString(), excecao));
        }
    }
}
