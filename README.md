# OrderPaymentSystemExample

## Architecture

I imagined the following human actors: customer, cook, kitchen staff, and courier, respectively.

- **Customer** - initiates the workflow by using a client app (e.g., DoorDash, etc.) to place a food order
- **Cook** - determines whether or not the order is valid, whether or not the kitchen can fulfill the order, prepares the food, and places the food on a shelf
for pick up
- **Kitchen Staff** - say something here
- **Courier** - arrives at the kitchen, picks up the order, and delivers said order back to the customer

### Customer

### Cook

### Kitchen Staff


### Courier

Couriers are dispatched when the order has been confirmed by the restaurant to have been valid.

![Courier State Machine Diagram](CourierStateMachine.png)

