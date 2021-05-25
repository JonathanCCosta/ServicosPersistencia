using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using FURNAS.GestaoSPE.GovernancaDados;
using Microsoft.SharePoint.Administration;
using FURNAS.GestaoSPE.GovernancaDados.TimerJob;

namespace FURNAS.GestaoSPE.GovernancaDados.Features.FeatureMensal
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("c0ca051b-1d3d-4156-bedd-5039428aef9e")]
    public class FeatureMensalEventReceiver : SPFeatureReceiver
    {
        const string JobName = "Governanca Importacao TimerJob Mensal H";
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
                GovernancaImportacaoMensal job = new TimerJob.GovernancaImportacaoMensal(JobName, site.WebApplication);
                //SPHourlySchedule schedule = new SPHourlySchedule();
                SPMonthlySchedule schedule = new SPMonthlySchedule();

                schedule.BeginDay = 1;
                schedule.BeginHour = 1;
                schedule.EndHour = 3;
                schedule.EndDay = 1;

                /*schedule.BeginMinute = 0;
                schedule.EndMinute = 59;*/

                job.Schedule = schedule;
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
