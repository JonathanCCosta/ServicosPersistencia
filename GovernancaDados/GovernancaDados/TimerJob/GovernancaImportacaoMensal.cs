using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using FURNAS.GestaoSPE.GovernancaDados.TimerJob;

namespace FURNAS.GestaoSPE.GovernancaDados.TimerJob
{
    public class GovernancaImportacaoMensal : SPJobDefinition  
    {
                public GovernancaImportacaoMensal():base()  
        {   
        }  
  
        public GovernancaImportacaoMensal (string jobname, SPService service, SPServer server, SPJobLockType targettype)
            : base(jobname, service, server, targettype)
        {
            this.Title = "Governanca Importacao TimerJob Mensal H";  
        }  
  
  
        public GovernancaImportacaoMensal(string jobName, SPWebApplication webapp) : base(jobName, webapp, null, SPJobLockType.ContentDatabase)  
        {
            this.Title = "Governanca Importacao TimerJob Mensal H";  
        }

        public override void Execute(Guid targetInstanceId)
        {
          new JobMensal().Executar();
        }
    }
}
