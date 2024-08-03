using CFW.EFCore;
using ChubbLife.Api.Features.Employees.Models;
using CFW.OData.Attributes;
using CFW.OData.Handlers;
using ChubbLife.Api.DbModels;

namespace ChubbLife.Api.Features.Employees.Handlers
{
    [ODataRouting(Name = "employees")]
    public class EmployeesDelete : BaseDeleteODataHandler<EmployeeModel, Employee, Guid>
    {
        public EmployeesDelete(AppDbContext db) : base(db)
        {
        }
    }
}
