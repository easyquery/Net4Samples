using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

using Microsoft.AspNet.Identity;

using Korzh.EasyQuery;
using Korzh.EasyQuery.Services;

using EqAspNet4Demo.Models;

namespace EqAspNet4Demo.Services
{
    public class ReportStore : IQueryStore
    {

        protected IPrincipal User;

        protected ApplicationDbContext DbContext;

        public ReportStore(ApplicationDbContext dbContext, IPrincipal user)
        {

            User = user;
            DbContext = dbContext;
        }

        public async Task<bool> AddQueryAsync(Query query)
        {
            if (string.IsNullOrEmpty(query.ID))
            {
                query.ID = Guid.NewGuid().ToString();
            }

            var report = new Report
            {
                Id = query.ID,
                Name = query.Name,
                Description = query.Description,
                ModelId = query.Model.ID,
                QueryJson = await query.SaveToJsonStringAsync(),
                OwnerId = User?.Identity.GetUserId()
            };


            if (report.OwnerId == null)
            {
                throw new ArgumentNullException(nameof(report.OwnerId));
            }

            DbContext.Reports.Add(report);
            await DbContext.SaveChangesAsync();
            return true;

        }

        public async Task<IEnumerable<QueryListItem>> GetAllQueriesAsync(string modelId)
        {
            var reports = await ApplyUserGuard(DbContext.Reports)
                            .Where(r => r.ModelId == modelId)
                            .ToListAsync();

            return reports.Select(r => new QueryListItem(r.Id, r.Name, r.Description));
        }



        public async Task<bool> LoadQueryAsync(Query query, string queryId)
        {
            var report = await ApplyUserGuard(DbContext.Reports).FirstOrDefaultAsync(r => r.Id == queryId);
            if (report != null)
            {
                await query.LoadFromJsonStringAsync(report.QueryJson);
                query.ID = report.Id;

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveQueryAsync(string modelId, string queryId)
        {
            var report = await ApplyUserGuard(DbContext.Reports).FirstOrDefaultAsync(r => r.Id == queryId);
            if (report != null)
            {
                DbContext.Reports.Remove(report);
                await DbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> SaveQueryAsync(Query query, bool createIfNotExist = true)
        {
            var report = await ApplyUserGuard(DbContext.Reports).FirstOrDefaultAsync(r => r.Id == query.ID);
            if (report != null)
            {
                report.Name = query.Name;
                report.Description = query.Description;
                report.ModelId = query.Model.ID;
                report.QueryJson = await query.SaveToJsonStringAsync();

                await DbContext.SaveChangesAsync();

                return true;
            }
            else if (createIfNotExist)
            {
                return await AddQueryAsync(query);
            }

            return false;
        }

        private IQueryable<Report> ApplyUserGuard(IQueryable<Report> filter)
        {
            var userId = User?.Identity.GetUserId();
            return filter.Where(r => r.OwnerId == userId);
        }

    }
}