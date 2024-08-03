using CFW.Core.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChubbLife.Api.DbModels
{
    public class Contract : IEntity<Guid>
    {
        public Guid Id { set; get; }

        [DisplayName("SỐ HĐ")]
        public string? ContractNumber { get; set; }

        [DisplayName("NGÀY PHÁT HÀNH")]
        public DateTime IssueDate { get; set; }

        [DisplayName("NGÀY XÁC NHẬN HĐ")]
        public DateTime? ConfirmationDate { get; set; }

        [DisplayName("MÃ KH")]
        public string? CustomerId { get; set; }

        [DisplayName("BÊN MUA BH")]
        public string? Buyer { get; set; }

        [DisplayName("NGƯỜI ĐƯỢC BH")]
        public string? InsuredPerson { get; set; }

        [DisplayName("PHÍ")]
        public string? Fee { get; set; }

        public decimal Amount { get; set; }

        [DisplayName("MAIL")]
        public string? Email { get; set; }

        [DisplayName("SĐT")]
        public string? PhoneNumber { get; set; }

        [DisplayName("TÊN TV")]
        public string? EmployeeName { get; set; }

        public Guid EmployeeId { set; get; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = new Employee();
    }
}
