//using FluentValidation;


//namespace Basket.Application.Features.Baskets.Commands.UpsertItem;

//public sealed class UpsertItemValidator : AbstractValidator<UpsertItemCommand>
//{
//    public UpsertItemValidator()
//    {
//        RuleFor(x => x.UserId).NotEmpty();
//        RuleFor(x => x.ProductId).NotEmpty();
//        RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
//        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
//        RuleFor(x => x.UnitPrice).GreaterThan(0);
//        RuleFor(x => x.Quantity).GreaterThan(0);
//        RuleFor(x => x.Currency).Length(3);
//    }
//}