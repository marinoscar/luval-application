using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.OpenAI.Models
{
    public class BaseChatObject
    {
        public BaseChatObject()
        {
            Id = Guid.NewGuid().ToString();
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
        }

        public string Id { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public string UpdatedByUserId { get; set; }
    }
}
