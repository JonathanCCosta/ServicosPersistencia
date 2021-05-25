using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using FURNAS.GestaoSPE.PersistenciaGovernanca.TimerJob;
using Microsoft.SharePoint.Administration;

namespace FURNAS.GestaoSPE.PersistenciaGovernanca.Features.GovernancaImportacaoDiario
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("189570c6-ba8c-4c64-a511-a6d3a8b9ec4e")]
    public class PrimeiraImportacaoEventReceiver : SPFeatureReceiver
    {
        const string JobName = "Governanca Importacao Timer Job Diario HOMOLOG";
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    //SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;

                    SPSite site = properties.Feature.Parent as SPSite;

                   
                            DeleteExistingJob(JobName, site);
                            CreateJob(site);
                       

                  
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool CreateJob(SPSite site)
        {
            bool jobCreated = false;
            try
            {
                TimerJob.GovernancaImportacaoDiario job = new TimerJob.GovernancaImportacaoDiario(JobName, site.WebApplication);
                SPHourlySchedule schedule = new SPHourlySchedule();


                schedule.BeginMinute = 0; 
                schedule.EndMinute = 59;
                
                job.Schedule = schedule;
                job.Update();

                job.Update();
            }
            catch (Exception)
            {
                return jobCreated;
            }
            return jobCreated;
        }
        public bool DeleteExistingJob(string jobName, SPSite site)
        {
            bool jobDeleted = false;
            try
            {

                foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
                {
                    if (job.Name == JobName)
                    {
                        job.Delete();
                        jobDeleted = true;
                    }
                }
                
            }
            catch (Exception)
            {
                return jobDeleted;
            }
            return jobDeleted;
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {

            lock (this)
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        SPSite site = properties.Feature.Parent as SPSite;
                       

                        foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
                        {
                            if (job.Name == JobName)
                            {
                                DeleteExistingJob(JobName, site);
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }  
    }
}
