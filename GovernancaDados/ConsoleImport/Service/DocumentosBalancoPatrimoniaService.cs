using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public class DocumentosBalancoPatrimoniaService
    {
        public void SetDocumentosBalancoPatrimonial(SPListItem itemBP)
        {
          
            //try
           // {
                if (itemBP != null)
                {
                    SPAttachmentCollection Attachments = itemBP.Attachments;
                    foreach (FileInfo attachment in Attachments)
                    {
                        FileStream fs = new FileStream(attachment.FullName, FileMode.Open, FileAccess.Read);

                        // Create a byte array of file stream length
                        byte[] ImageData = new byte[fs.Length];

                        //Read block of bytes from stream into the byte array
                        fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));

                        //Close the File Stream
                        fs.Close();

                        itemBP.Attachments.Add(attachment.Name, ImageData);
                    }
                }
            //}
           // catch (Exception)
           // {
                itemBP = null;

          //  }

        }
    }
}
