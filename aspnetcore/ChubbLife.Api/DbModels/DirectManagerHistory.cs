
using CFW.Core.Models;

namespace ChubbLife.Api.DbModels
{
    public class DirectManagerHistory : IEntity<Guid>
    {
        public Guid Id { set; get; }

        public Guid EmployeeId { get; set; }

        public DateTime? BeginDate { get; set; }

        public Guid? DirectManagerId { get; set; }
    }
}
