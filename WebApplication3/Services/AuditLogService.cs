
using WebApplication3.Model;


namespace WebApplication3.Services
{
    public class AuditLogService
    {
        private readonly AuthDbContext _dbContext;

        public AuditLogService(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void LogAudit(string userId, string action, string details)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                Timestamp = DateTime.Now,
                Details = details
            };

            _dbContext.AuditLogs.Add(auditLog);
            _dbContext.SaveChanges();
        }
    }
}

