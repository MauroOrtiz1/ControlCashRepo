using ControlCash.Application.DTOs;
using FluentValidation;

namespace ControlCash.Application.Validators;

public class CategoriaCreateDtoValidator : AbstractValidator<CategoriaCreateDto>
{
    public CategoriaCreateDtoValidator()
    {
        RuleFor(x => x.NombreCategoria)
            .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
    }
}

public class CategoriaUpdateDtoValidator : AbstractValidator<CategoriaUpdateDto>
{
    public CategoriaUpdateDtoValidator()
    {
        RuleFor(x => x.NombreCategoria)
            .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
    }
}
