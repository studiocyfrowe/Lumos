using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hades2Monitor.Models
{
    public class UserSessionInfo
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public string Domain { get; set; }

        public override string ToString()
            => string.IsNullOrEmpty(Domain)
                ? UserName
                : $"{Domain}\\{UserName}";
    }
}
