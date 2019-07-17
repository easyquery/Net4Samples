using Microsoft.AspNet.Identity.EntityFramework;

namespace EqAspNet4Demo.Models
{
    public class Report
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ModelId { get; set; }

        public string QueryJson { get; set; }

        public string OwnerId { get; set; }

        public IdentityUser Owner { get; set; }
    }
}