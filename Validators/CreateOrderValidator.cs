using FluentValidation;
using GamingGearBackend.DTOs;

namespace GamingGearBackend.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            // Rule 1: Must have at least 1 item
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Đơn hàng phải có ít nhất 1 sản phẩm.");

            // Rule 2: Each item quantity must be 1-100
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.Quantity)
                     .GreaterThan(0)
                     .LessThanOrEqualTo(100)
                     .WithMessage("Số lượng mỗi sản phẩm phải từ 1 đến 100.");
            });

            // Rule 3: Phone number - optional, only validate format if provided
            // Supports VN mobile prefixes: 03x, 05x, 07x, 08x, 09x
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^0[3-9]\d{8}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Số điện thoại không hợp lệ. Định dạng VN: 0[3-9]xxxxxxxx");
        }
    }
}
