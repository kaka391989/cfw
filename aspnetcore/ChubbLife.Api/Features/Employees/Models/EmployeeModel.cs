using CFW.Core.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChubbLife.Api.Features.Employees.Models
{

    public class EmployeeModel : IEntity<Guid>
    {
        [Key]
        public Guid Id { set; get; }

        [DisplayName("Mã số Đại Diện Kinh Doanh")]
        public string? EmployeeId { get; set; }

        [DisplayName("Họ tên Đại Diện Kinh Doanh")]
        public string? EmployeeName { get; set; }

        [DisplayName("Ngày gia nhập")]
        public DateTime? JoiningDate { get; set; }

        [DisplayName("Vị trí")]
        public string? Position { get; set; }

        public IEnumerable<EmployeePositionModel>? Positions { get; set; }

        [DisplayName("MÃ SỐ THUẾ")]
        public string? TaxId { get; set; }

        [DisplayName("SDT")]
        public string? PhoneNumber { get; set; }

        [DisplayName("số CMND")]
        public string? IdentityNumber { get; set; }

        [DisplayName("GMAIL")]
        public string? Email { get; set; }

        [DisplayName("QL trực tiếp")]
        public string? DirectManager { get; set; }

        public IEnumerable<DirectManagerHistoryModel>? DirectManagers { get; set; }

        [DisplayName("Người Giới Thiệu")]
        public string? Introducer { get; set; }

        public Guid? IntroducerId { set; get; }

        public DateTime CreatedAt { set; get; }

        public DateTime? UpdatedAt { set; get; }

        public string? CreatedBy { set; get; }

        public string? UpdatedBy { set; get; }
    }
}
