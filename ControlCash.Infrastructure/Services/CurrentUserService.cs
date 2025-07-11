﻿using System.Security.Claims;
using ControlCash.Domain.Interfaces.Services;

namespace ControlCash.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public string ObtenerEmailDesdeClaims(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value ?? throw new Exception("No se pudo obtener el email del usuario.");
    }
    public int ObtenerId(ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idClaim, out var id) ? id : throw new Exception("ID de usuario inválido.");
    }

}