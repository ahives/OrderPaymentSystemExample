namespace Services.Core
{
    using Events;

    public interface IOrderValidator
    {
        bool Validate(ValidateOrder message);
    }
}