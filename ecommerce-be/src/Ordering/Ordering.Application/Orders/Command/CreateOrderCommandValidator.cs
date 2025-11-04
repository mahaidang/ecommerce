using FluentValidation;
using Ordering.Application.Common;
using Ordering.Application.Orders.Command;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator(IOrderingDbContext db)
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.DiscountTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ShippingFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Items).NotEmpty();

        RuleForEach(x => x.Items).SetValidator(new CreateOrderItemDtoValidator());
    }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UnitPrice).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
