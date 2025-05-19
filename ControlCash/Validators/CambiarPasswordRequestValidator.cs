using FluentValidation;
using ControlCash.DTOs;

namespace ControlCash.Validators;

public class CambiarPasswordRequestValidator : AbstractValidator<CambiarPasswordRequest>
{
    public CambiarPasswordRequestValidator()
    {
        RuleFor(x => x.PasswordActual)
            .NotEmpty().WithMessage("La contraseña actual es obligatoria.");
        
        RuleFor(x => x.NuevaPassword)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("Debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("Debe contener al menos una letra minúscula.")
            .Matches(@"\d").WithMessage("Debe contener al menos un número.")
            .Matches(@"[\W_]").WithMessage("Debe contener al menos un carácter especial.");

    }
}