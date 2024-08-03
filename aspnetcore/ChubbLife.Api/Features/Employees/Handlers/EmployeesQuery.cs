using CFW.EFCore;
using ChubbLife.Api.Features.Employees.Models;
using Microsoft.EntityFrameworkCore;
using CFW.OData.Attributes;
using CFW.OData.Handlers;
using ChubbLife.Api.DbModels;

namespace ChubbLife.Api.Features.Employees.Handlers
{
    [ODataRouting(Name = "employees")]
    public class EmployeesQuery : BaseQueryODataHandler<EmployeeModel, Guid>, IODataEntitySetsQueryHandler<EmployeeModel, Guid>
    {
        public EmployeesQuery(AppDbContext db) : base(db)
        {
        }

        public override IQueryable<EmployeeModel> Query => _db.Set<Employee>().Select(x => new EmployeeModel
        {
            Id = x.Id,
            CreatedAt = x.CreatedAt,
            CreatedBy = x.CreatedBy,
            DirectManager = x.DirectManager,
            DirectManagers = x.DirectManagers!.Select(d => new DirectManagerHistoryModel
            {
                BeginDate = d.BeginDate,
                EmployeeId = d.EmployeeId,
                DirectManager = _db.Set<Employee>()
                .Where(e => e.Id == d.DirectManagerId)
                .Select(e => e.EmployeeName).FirstOrDefault(),
                DirectManagerId = d.EmployeeId,
            }),
            Email = x.Email,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.EmployeeName,
            IdentityNumber = x.IdentityNumber,
            Introducer = x.Introducer,
            IntroducerId = x.IntroducerId,
            JoiningDate = x.JoiningDate,
            PhoneNumber = x.PhoneNumber,
            Position = x.Position,
            Positions = x.Positions!.Select(x => new EmployeePositionModel
            {
                BeginDate = x.BeginDate,
                EmployeeId = x.EmployeeId,
                Id = x.Id,
                PositionRole = x.PositionRole,

            }).ToList(),
            TaxId = x.TaxId,
            UpdatedAt = x.UpdatedAt,
            UpdatedBy = x.UpdatedBy,
        }).AsNoTracking();
    }
}
