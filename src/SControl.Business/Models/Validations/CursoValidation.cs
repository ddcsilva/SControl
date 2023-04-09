using FluentValidation;

namespace SControl.Business.Models.Validations;

public class CursoValidation : AbstractValidator<Curso>
{
    public CursoValidation()
    {
        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .Length(2, 50).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

        RuleFor(c => c.DuracaoPorSemestre)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .GreaterThan(0).WithMessage("O campo {PropertyName} precisa ser maior que zero");

        RuleFor(c => c.Modalidade)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .Length(2, 30).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

        RuleFor(c => c.Ativo)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");
    }
}
