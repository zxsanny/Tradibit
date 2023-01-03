using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.Api.Controllers;
using Tradibit.DataAccess;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Dashboard;
using Tradibit.SharedUI.DTO.Primitives;
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
            .OrderBy(x => x.TimeValue.DateTime)
            .Select(x => x.TimeValue)
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
        await _db.Strategies.Where(s => s.IsPublic || s.Users.Where(us => us.UserId == request.UserId && !us.IsActive))
            
    }
}