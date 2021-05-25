using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FURNAS.GestaoSPE.GovernancaDados.Utilities
{
    public static class Email
    {
        public static bool SendEmail(
     SPWeb spWeb, 
  string body
  )
        {
            var messageHeaders = new StringDictionary();

            messageHeaders.Add("from", "jorgeh@furmas.com.br");
            messageHeaders.Add("to", "jorgeh@furmas.com.br");           
            messageHeaders.Add("subject", "Job Governancia SPE Diario");
            messageHeaders.Add("cc", "jorweb.cefet@gmail.com");
          
            string mimeType = "text/plain";
            
            messageHeaders.Add("content-type", mimeType);
            return  SPUtility.SendEmail(
                   spWeb,
                   messageHeaders,
                   body);
             
          
          
        }
    }
}
