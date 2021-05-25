using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Utilities
{
    public static class LogHelper
    {
        

        public static void GravarLog(SPWeb web, Exception e, string mensagem)
        {
            try
            {
                web.AllowUnsafeUpdates = true;
                SPList oList = web.Lists["log"];
                SPListItem ListItem = oList.Items.Add();

                string excecao = "StackTrace nula - v1.4";

                ListItem["Title"] = mensagem;

                if (e != null)
                {
                    if (e.Message != null)
                        ListItem["excecao"] = e.Message;

                    if (e.StackTrace != null)
                    {
                        ListItem["comentario"] = e.StackTrace;
                        excecao = e.StackTrace;
                    }
                }

                ListItem["url"] = web.Url;
                ListItem.Update();

                /*StringDictionary headers = new StringDictionary();
                headers.Add("To", "jorgeh@furnas.com.br");
                headers.Add("From", "aplicacoesweb@furnas.com.br");
                headers.Add("Subject", string.Format("Test Email Job, do dia e hora {0}", DateTime.Now));
                headers.Add("content-type", "text/html");

                Boolean emailSent = true;

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    emailSent = SPUtility.SendEmail(web, headers, excecao, true);
                });*/
                web.AllowUnsafeUpdates = false;
            }
            catch { }
         //  bool retorno = Email.SendEmail(web, string.Format("Verificar na lista de Log o erro com ID {0} com a seguinte Excecao: <br/>{1}","Inicio", excecao));
        }
    }
}
