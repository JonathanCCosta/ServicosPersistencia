using FURNAS.GestaoSPE.PersistenciaGovernanca.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Service
{
    public class EscritorioSPEService
    {
        public string SetEscritorio(EscritorioSPE escritorioSPE)
        {
            
            string endereco = string.Empty;
          //  try
         //   {

                if (escritorioSPE != null)
                {

                    return endereco = string.Format("{0}, {1} - {2}, {3}, {4}, {5}", escritorioSPE.TipoLogradouro, escritorioSPE.Numeral, escritorioSPE.Bairro, escritorioSPE.Cidade, escritorioSPE.Estado, escritorioSPE.CEP);

                     
                }
           // }
          //  catch (Exception e)
          //  {
                escritorioSPE = null;

          //  }

            //itemGovernanca["Endereco"] 
            return endereco;

        }

          }
}
