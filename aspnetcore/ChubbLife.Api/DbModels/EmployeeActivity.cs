using CFW.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChubbLife.Api.DbModels
{
    public class EmployeeActivity : IEntity<Guid>
    {
        public Guid Id { set; get; }

        public Guid EmployeeId { set; get; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { set; get; } = new Employee();

        public DateTime ActivityDate { set; get; }

        public int CompletedCourses { set; get; }
    }
}
