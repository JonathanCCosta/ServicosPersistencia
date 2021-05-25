using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao
{
    public class Governanca
    {
        public SPFieldLookupValue Complexo { get; set; }
        public string ComplexoDescricao { get; set; }
        public int IDComplexoGov { get; set; }        
        public string Title { get; set; }
        public string CNPJ { get; set; }
        public string Endereco { get; set; }
        public DateTime? DataConstituicao { get; set; }
        public string ParticipacoesSocios { get; set; }
        public string ConselhoAdministracao { get; set; }
        public string Estagio { get; set; }
        public string Logo { get; set; }
        public Double Investimento { get; set; }
        public DateTime? DataEntradaOperacaoPlena { get; set; }
        public string DescricaoEmpreendimento { get; set; }
        public string FaseEmpreendimento { get; set; }
        public string Diretoria { get; set; }
        public string Site { get; set; }
        public DocumentosBalancoPatrimonia DocumentoBP { get; set; }
       
     
        public List<SPListItem> GetListGovernancaSPE()
        {
            using (SPSite site = new SPSite("http://shpt12:160/secretariagovernanca/"))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists["SPE"];

                   
                   
                   return  list.GetItems().Cast<SPListItem>().ToList();
                }
            }
        }

       

        public SPWeb GetListGovernanca()
        {
            using (SPSite site = new SPSite("http://shpt12:160/secretariagovernanca/"))
            {
                using (SPWeb web = site.OpenWeb())
                {


                    return web;
                }
            }
        }

        public SPWeb ListGovernanca()
        {
            using (SPSite site = new SPSite("http://shpt12:160/secretariagovernanca/"))
            {
                using (SPWeb web = site.OpenWeb())
                {


                    return web;
                }
            }
        }

    }
}
