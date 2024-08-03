using CFW.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChubbLife.Api.DbModels
{
    [Table("Employees")]
    public class Employee : IEntity<Guid>, IAuditable
    {
        [Key]
        public Guid Id { set; get; }

        public string? EmployeeId { get; set; }

        public string? EmployeeName { get; set; }

        public DateTime? JoiningDate { get; set; }

        public string? Position { get; set; }

        public IEnumerable<EmployeePosition>? Positions { get; set; }

        public string? TaxId { get; set; }

        public string? PhoneNumber { get; set; }

        public string? IdentityNumber { get; set; }

        public string? Email { get; set; }

        public string? DirectManager { get; set; }

        public IEnumerable<DirectManagerHistory>? DirectManagers { get; set; }

        public string? Introducer { get; set; }

        public Guid? IntroducerId { set; get; }

        public DateTime CreatedAt { set; get; }

        public DateTime? UpdatedAt { set; get; }

        public string? CreatedBy { set; get; }

        public string? UpdatedBy { set; get; }
    }
}
