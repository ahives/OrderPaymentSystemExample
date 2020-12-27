namespace Services.Core
{
    using System;
    using Events;

    public class OrderValidator :
        IOrderValidator
    {
        public bool Validate(ValidateOrder data)
        {
            if (data.OrderId == Guid.Empty || data.RestaurantId == Guid.Empty || data.CustomerId == Guid.Empty)
                return false;

            for (int i = 0; i < data.Items.Length; i++)
            {
                if (data.Items[i].Id == Guid.Empty)
                    return false;
            }

            return true;
        }
    }
}