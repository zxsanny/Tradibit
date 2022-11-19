using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Queries;
using Tradibit.Common.Entities;

namespace Tradibit.Common.Interfaces.API;

public interface IScenariosApi
{
    Task<Response<List<PagedResponse<Scenario>>>> GetScenarios(GetScenariosQuery query);
}