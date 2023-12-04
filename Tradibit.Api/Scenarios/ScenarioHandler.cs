using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.DataAccess;
using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Dashboard;
using Tradibit.Shared.DTO.Primitives;
using Tradibit.Shared.DTO.Scenarios;
using Tradibit.SharedUI.Interfaces;

namespace Tradibit.Api.Scenarios;

public class ScenarioHandler : 
    IRequestHandler<GetCurrentUserDashboardRequest, UserDashboard>,
    IRequestHandler<GetAvailableStrategiesRequest, List<IdName>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly TradibitDb _db;

    public ScenarioHandler(ICurrentUserProvider currentUserProvider, TradibitDb db)
    {
        _currentUserProvider = currentUserProvider;
        _db = db;
    }
    
    public async Task<UserDashboard> Handle(GetCurrentUserDashboardRequest request, CancellationToken cancellationToken)
    {
        var funds = await _db.UserFunds
            .Where(x => x.UserId == _currentUserProvider.CurrentUser.Id)
            .OrderBy(x => x.DateTime)
            .Select(x => new TimeValue
            {
                Value = x.Value, 
                DateTime = x.DateTime
            })
            .ToListAsync(cancellationToken);

        var first = funds.FirstOrDefault();
        var stat = new ProfitLossStat(first?.DateTime, first?.Value, funds.LastOrDefault()?.Value);
        
        //var scenarios = _db.ScenarioHistories
        return new UserDashboard
        {
            UserStat = stat,
            UserFunds = funds,
            Scenarios = new List<ScenarioDto>()
        };
    }

    public async Task<List<IdName>> Handle(GetAvailableStrategiesRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.CurrentUserId;
        return await _db.Strategies.Where(s => s.IsPublic && s.OwnerUserId == userId)
            .Select(x => new IdName(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }
}