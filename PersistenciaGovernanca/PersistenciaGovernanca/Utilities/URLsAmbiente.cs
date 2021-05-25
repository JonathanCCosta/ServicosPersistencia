using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Utilities
{
    public static class URLsAmbiente
    {
        public static string DEV = "http://shpt12:160/secretariagovernanca";
        public static string HOM = "http://dp-furnasnetd/secretariagovernanca/";
        public static string PROD = "http://dp-furnasnetp/secretariagovernanca/";

        public static string HOM_SISSPE = "http://www.sisspehml.cepel.br/dnnet/sp/gep/gestaospe";
        public static string PROD_SISSPE = "http://www.sisspe.cepel.br/dnnet/sp/gep/gestaospe";
    }
}
