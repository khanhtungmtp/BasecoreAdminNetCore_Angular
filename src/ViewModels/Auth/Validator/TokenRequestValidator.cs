using FluentValidation;

namespace ViewModels.Auth.Validator;

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("AccessToken value is required");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken value is required");
    }
}