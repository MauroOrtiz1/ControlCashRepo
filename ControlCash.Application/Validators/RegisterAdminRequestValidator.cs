using FluentValidation;
using ControlCash.Application.DTOs;

namespace ControlCash.Application.Validators;

public class RegisterAdminRequestValidator : AbstractValidator<RegisterAdminRequest>
{
    public RegisterAdminRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no debe superar los 100 caracteres.");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("Debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("Debe contener al menos una letra minúscula.")
            .Matches(@"\d").WithMessage("Debe contener al menos un número.")
            .Matches(@"[\W_]").WithMessage("Debe contener al menos un carácter especial.");

        RuleFor(x => x.Rol)
            .NotEmpty()
            .Must(r => r.ToLower() == "user" || r.ToLower() == "admin")
            .WithMessage("Rol inválido. Solo se permiten: user, admin.");
    }
}
