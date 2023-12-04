using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tradibit.DataAccess;
using Tradibit.Shared;
using Tradibit.Shared.DTO.Auth;
using Tradibit.Shared.Entities;
using Tradibit.SharedUI.DTO.Users;

namespace Tradibit.Api.Services;

public class UserHandler :
    IRequestHandler<GetUserByIdRequest, User?>,
    IRequestHandler<RegisterUserRequest, string>
{
    private readonly IMediator _mediator;
    private readonly TradibitDb _db;
    private readonly IMapper _mapper;
    private readonly AuthConfig _authConfig;

    public UserHandler(IMediator mediator, TradibitDb db, IMapper mapper, IOptions<AuthConfig> authConfig)
    {
        _mediator = mediator;
        _db = db;
        _mapper = mapper;
        _authConfig = authConfig.Value;
    }

    public async Task<User?> Handle(GetUserByIdRequest request, CancellationToken cancellationToken) =>
        await _db.Users
            .Include(x => x.UserState)
            .Include(x => x.UserSettings)
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

    public async Task<string> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(request);
        await _db.Save(user, cancellationToken);
        return GetToken(user);
    }

    private string GetToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authConfig.JwtSecret!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            expires: DateTime.Now.AddSeconds(_authConfig.TokenExpireSeconds),
            signingCredentials: credentials,
            claims: user.ToClaims());

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}