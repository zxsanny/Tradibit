using MediatR;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.DTO.Queries;
using Tradibit.Common.Entities;
using Tradibit.Common.Interfaces.API;

namespace Tradibit.Api.Controllers;

public class ScenariosController : TradibitBaseController, IScenariosApi
{
    public ScenariosController(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    public async Task<Response<List<PagedResponse<Scenario>>>> GetScenarios(GetScenariosQuery query) =>
        await Send(query);

    public async Task<Response> CreateScenario(ScenarioCreatedEvent command) =>
        await Send(command);
    
    public async Task<Response> TestScenarioHistory(StartHistoryTestScenarioEvent query) =>
        await Send(query);

    public async Task<Response> StartScenario(StartScenarioEvent query) =>
        await Send(query);
}