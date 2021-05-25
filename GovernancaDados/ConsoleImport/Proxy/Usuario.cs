using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleImportacao.Proxy
{
    public class Usuario
    {
        public string Dominio { get; set; }

        
        public string Login { get; set; }
        public string Senha { get; set; }
    }
}
