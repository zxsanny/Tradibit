using Tradibit.Common.Entities;

namespace Tradibit.Common.Interfaces;

public interface ICurrentUserProvider
{
    User CurrentUser { get; }
}