using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleImportacao.Service
{
    public class EmpreendimentoService
    {


        public Empreendimento VerificarFaseAnterior(Empreendimento emprendimentoAnterior, Empreendimento emprendimentoNovo, out string fase)
        {

             fase= string.Empty;

             if (emprendimentoAnterior == null)
                 return emprendimentoNovo;

            if (emprendimentoAnterior.FaseEmpreendimento == emprendimentoNovo.FaseEmpreendimento) // Se todos empreendimentos na mesma fase = fase dos empreendimentos
                return emprendimentoAnterior;

            if (emprendimentoNovo.FaseEmpreendimento == "outros" && emprendimentoNovo.FaseEmpreendimento != "Outros")// Se empreendimentos como "outros" e demais empreendimentos na mesma fase = fase dos empreendimentos diferente de "outros"
                return emprendimentoNovo;

            if (emprendimentoNovo.FaseEmpreendimento == "Operação Parcial" || emprendimentoNovo.FaseEmpreendimento == "Implantacao")//Se empreendimentos em operação e empreendimentos em operação parcial = Operação Parcial//Se empreendimentos em implantação e empreendimentos em operação parcial = Operação Parcial
            {
                fase = "Operação Parcial";
                return emprendimentoNovo;
            }
             
            if (emprendimentoNovo.Criado > emprendimentoAnterior.Criado)
                return emprendimentoNovo;
            else
                return emprendimentoAnterior;
        }


    }
}
