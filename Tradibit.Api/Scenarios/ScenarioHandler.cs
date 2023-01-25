using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.Api.Controllers;
using Tradibit.DataAccess;
using Tradibit.Shared.Entities;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Dashboard;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.Scenarios;
using Tradibit.SharedUI.Interfaces;

namespace Tradibit.Api.Scenarios;

public class ScenarioHandler : 
    IRequestHandler<GetCurrentUserDashboardRequest, UserDashboard>,
    IRequestHandler<GetAvailableStrategiesRequest, List<IdName>>,
    IRequestHandler<AddStrategyToUserRequest>,
    IRequestHandler<RemoveStrategyFromUserRequest>
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
        var userId = _currentUserProvider.CurrentUserId;
        var publicStrategies = await _db.Strategies.Where(s => s.IsPublic && s.Users.All(u => u.UserId != userId))
            .Select(x => new IdName(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        var addedDisabledStrategies = await _db.StrategyUsers.Where(x => x.UserId == userId && !x.IsActive)
            .Select(x => new IdName(x.StrategyId, x.Strategy.Name))
            .ToListAsync(cancellationToken);

        return publicStrategies.Concat(addedDisabledStrategies).ToList();
    }
    
    public async Task<Unit> Handle(AddStrategyToUserRequest request, CancellationToken cancellationToken)
    {
        var strategyUser = await _db.StrategyUsers.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.StrategyId == request.StrategyId, cancellationToken);
        if (strategyUser == null)
            await _db.StrategyUsers.AddAsync(new StrategyUser { UserId = request.UserId, StrategyId = request.StrategyId }, cancellationToken);
        else
        {
            strategyUser.IsActive = true;
            _db.Update(strategyUser);   
        }
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;    
    }

    public async Task<Unit> Handle(RemoveStrategyFromUserRequest request, CancellationToken cancellationToken)
    {
        var strategyUser = await _db.StrategyUsers.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.StrategyId == request.StrategyId, cancellationToken);
        strategyUser!.IsActive = false;
        await _db.Save(strategyUser, cancellationToken);
        return Unit.Value;
    }
}