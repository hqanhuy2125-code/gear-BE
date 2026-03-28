using FluentValidation;
using GamingGearBackend.DTOs;
using System.Text.RegularExpressions;

namespace GamingGearBackend.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            
            // Password: tối thiểu 8 ký tự, có chữ hoa, chữ thường, số
            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ hoa.")
                .Matches("[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ thường.")
                .Matches("[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số.");
        }
    }
}
