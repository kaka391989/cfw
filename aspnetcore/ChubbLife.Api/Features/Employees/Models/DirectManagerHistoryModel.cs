using CFW.Core.Models;
using ChubbLife.Api.DbModels;
using System.ComponentModel.DataAnnotations;

namespace ChubbLife.Api.Features.Employees.Models
{
    public class DirectManagerHistoryModel : IEntity<Guid>
    {
        [Key]
        public Guid Id { set; get; }

        public Guid EmployeeId { get; set; }

        public DateTime? BeginDate { get; set; }

        public Guid? DirectManagerId { get; set; }

        public string? DirectManager { set; get; }
    }

    public class EmployeePositionModel : IEntity<Guid>
    {
        [Key]
        public Guid Id { set; get; }

        public Guid EmployeeId { get; set; }

        public DateTime? BeginDate { get; set; }

        public EmployeePositionRole PositionRole { get; set; }
    }
}
