using Microsoft.AspNet.Identity;
using Quartz;
using Quartz.Impl;
using Swappy_V2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Swappy_V2.Modules.DealMatchingModule
{
    public class DealMatchingModule
    {
        private static DealMatchingModule instance;

        private DealMatchingModule() { }

        public static DealMatchingModule Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DealMatchingModule();
                }
                return instance;
            }
        }


        public bool Matches(string a, string b)
        {
            return SearchModule.GetFuzze(a, b) == 1.0f;
        }


        public void FindMatch(DealModel a)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<FindMatchJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .Build();
            job.JobDataMap.Add(new KeyValuePair<string, object>("Deal", a));
            scheduler.ScheduleJob(job, trigger);
        }
    }

    class FindMatchJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var deal = (DealModel)context.MergedJobDataMap.Get("Deal");
            DealsRepository dealsRepository = new DealsRepository();
            UsersRepository usersRepository = new UsersRepository();
            IMatchProvider provider = new SimpleMatchProvider();
            var matchedDeals = new List<DealModel>();
            var matchedOwners = provider.GetMatchedDealsOwners(deal, out matchedDeals);
            Debug.WriteLine("Matched deals for deal #" + deal.Id);
            foreach(var matchedDeal in matchedOwners)
            {
                Debug.WriteLine(matchedDeal.Id);
            }
            var user = usersRepository.Get(deal.AppUserId);
            SendDealToUser(matchedDeals, user);
            var dealList = new List<DealModel>() { deal };
            foreach(var u in matchedOwners)
            {
                SendDealToUser(dealList, u);
            }
        }
        public void SendDealToUser(List<DealModel> deals, AppUserModel user)
        {
            var message = new IdentityMessage();
            message.Destination = user.Email;
            message.Subject = "Интересные объявления";
            var url = HttpContext.Current.Request.Url.AbsoluteUri + "Deals/Info?dealId=";
            message.Body = String.Join("\n",
                        deals.Select(d => String.Format("<p><a href='{0}{1}'>{2}</a></p>", url, d.Id, d.Title)));
            EmailModule.SendAsync("noreply@swappy.ru", message);
        }
    }
}