
using FluentValidation;
using ControlCash.Application.DTOs;

namespace ControlCash.Application.Validators;

public class GastoCreateValidator : AbstractValidator<GastoCreateDTO>
{
    public GastoCreateValidator()
    {
        RuleFor(x => x.IdCategoria).GreaterThan(0).WithMessage("La categoría es obligatoria.");
        RuleFor(x => x.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor a 0.");
        RuleFor(x => x.Descripcion).MaximumLength(200).WithMessage("La descripción no debe exceder los 200 caracteres.");
    }
}