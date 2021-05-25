using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using FURNAS.GestaoSPE.PersistenciaGovernanca.TimerJob;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.TimerJob
{
    public class GovernancaImportacaoDiario : SPJobDefinition  
    {
        public GovernancaImportacaoDiario():base()  
        {   
        }

        public GovernancaImportacaoDiario(string jobname, SPService service, SPServer server, SPJobLockType targettype)
            : base(jobname, service, server, targettype)
        {
            this.Title = "Governanca Importacao Timer Job Diario HOMOLOG";  
        }


        public GovernancaImportacaoDiario(string jobName, SPWebApplication webapp)
            : base(jobName, webapp, null, SPJobLockType.ContentDatabase)  
        {
            this.Title = "Governanca Importacao Timer Job Diario HOMOLOG";  
        }

        public override void Execute(Guid targetInstanceId)
        {

            new JobDiario().Executar();
        }
    }
}
