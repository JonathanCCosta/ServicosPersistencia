using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public class ComplexoService
    {
        public SPFieldLookupValue GetComplexo(Complexo complexo)
        {
            return new SPFieldLookupValue(complexo.ID,complexo.Title);
 
        }
        public string GetLogo(Complexo complexo)
        {

       var teste =complexo.Logo.Split('#');
       string nomedoLogo = teste[1].ToString().Split('/').LastOrDefault().Remove(teste[1].ToString().Split('/').LastOrDefault().Length - 1);
       teste[2] = teste[2].ToString().Replace(teste[2].ToString(), "http://shpt12:160/secretariagovernanca/LogoSPE/" + nomedoLogo);

      
      var format =  string.Format("#{0}#{1}#{2}", teste[0], teste[1], teste[2]);
  
            return Convert.ToString(format);
        }
    }
}
