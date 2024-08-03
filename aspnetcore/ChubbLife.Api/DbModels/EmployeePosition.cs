using CFW.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChubbLife.Api.DbModels
{
    public class EmployeePosition : IEntity<Guid>
    {
        public Guid Id { set; get; }

        public Guid EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = default!;

        public DateTime? BeginDate { get; set; }

        public EmployeePositionRole PositionRole { get; set; }
    }
}
