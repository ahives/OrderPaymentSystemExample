namespace Services.Core
{
    using Events;

    public class OrderValidator :
        IOrderValidator
    {
        public bool Validate(ValidateOrder message) => true;
    }
}