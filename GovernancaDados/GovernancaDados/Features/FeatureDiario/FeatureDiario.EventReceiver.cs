using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using FURNAS.GestaoSPE.GovernancaDados.TimerJob;
using Microsoft.SharePoint.Administration;
namespace FURNAS.GestaoSPE.GovernancaDados.Features.FeatureDiario
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("ee718c38-914a-4dfa-a8be-eaecaf5ab764")]
    public class FeatureDiarioEventReceiver : SPFeatureReceiver
    {
        const string JobName = "Governanca Importacao TimerJob Diario H";
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
