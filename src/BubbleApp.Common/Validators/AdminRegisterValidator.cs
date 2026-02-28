using BubbleApp.Common.ViewModels.Auth;
using FluentValidation;

namespace BubbleApp.Common.Validators;

public class AdminRegisterValidator : AbstractValidator<AdminRegisterRequest>
{
    public AdminRegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}