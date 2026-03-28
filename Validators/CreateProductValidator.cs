using FluentValidation;
using GamingGearBackend.Models;

namespace GamingGearBackend.Validators
{
    public class CreateProductValidator : AbstractValidator<Product>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(200);
            RuleFor(p => p.Price).GreaterThan(0).LessThan(100000000);
            RuleFor(p => p.Stock).GreaterThanOrEqualTo(0).LessThanOrEqualTo(10000);
            RuleFor(p => p.Category).NotEmpty();
        }
    }
}
