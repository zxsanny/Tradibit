using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Dashboard;
using Tradibit.Common.Entities;
using Tradibit.Common.Interfaces;
using Tradibit.DataAccess;

namespace Tradibit.Api.Scenarios;

public class ScenarioHandler : 
    IRequestHandler<GetCurrentUserDashboard, UserDashboard>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly TradibitDb _db;

    public ScenarioHandler(ICurrentUserProvider currentUserProvider, TradibitDb db)
    {
        _currentUserProvider = currentUserProvider;
        _db = db;
    }
    
    public async Task<UserDashboard> Handle(GetCurrentUserDashboard request, CancellationToken cancellationToken)
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
}