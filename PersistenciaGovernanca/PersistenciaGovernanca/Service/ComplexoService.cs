using FURNAS.GestaoSPE.PersistenciaGovernanca.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Service
{
    public class ComplexoService
    {
        public SPFieldLookupValue GetComplexo(Complexo complexo)
        {
            return new SPFieldLookupValue(complexo.ID,complexo.Title);
 
        }
        public string GetLogo(Complexo complexo)
        {
            string urlSite = Utilities.URLsAmbiente.HOM; //"http://dp-furnasnetd/secretariagovernanca/"; //"http://shpt12:160/secretariagovernanca/LogoSPE/";
            var teste = complexo.Logo.Split('#');
            string nomedoLogo = teste[1].ToString().Split('/').LastOrDefault().Remove(teste[1].ToString().Split('/').LastOrDefault().Length - 1);
            teste[2] = teste[2].ToString().Replace(teste[2].ToString(), urlSite + nomedoLogo);


            var format = string.Format("{0}#{1};#{2}", teste[0], teste[2], teste[2]);

            return Convert.ToString(format);
        }
    }
}
