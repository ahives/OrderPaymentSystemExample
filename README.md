# OrderPaymentSystemExample

## Architecture

I imagined the following human actors: customer, chef/cook, kitchen staff, and courier, respectively.

- **Customer** - initiates the workflow by using a client app (e.g., DoorDash, etc.) to place a food order
- **Chef/Cook** - determines whether or not the order is valid, whether or not the kitchen can fulfill the order, prepares the food, and places the food on a shelf
for pick up
- **Kitchen Staff** - say something here
- **Courier** - arrives at the kitchen, picks up the order, and delivers said order back to the customer

#### What is an Order?

An order has several meanings. To better understand, lets look at its meaning from different perspectives.
- **Customer/Courier** - from the customer or courier's perspective, an order is a single, atomic transaction between it and a restaurant.
- **Restaurant** - from the restaurant perspective, an order is a single, atomic transaction between it and a courier. To most efficiently prepare the order, however, the restaurant may require that multiple chefs or cooks prepare each item within the order in parallel.
- **Chef/Cook** - from the chef or cook perspective, an order is a set of singular requests to prepare food items in order to fulfill the order.

Essentially, an order may consist of one or more items that must be prepared individually. That said, each item being prepared impacts the final state of the
order. Each order item has its own state as well as it is being prepared.



![Order Item State Machine Diagram](OrderItemStateMachine.png)

**Figure 2**


### Customer

### Cook

Cooks are dispatched when the order has been confirmed by the restaurant to have been valid.



### Kitchen Staff


### Courier

Couriers are dispatched when the order has been confirmed by the restaurant to have been valid.

#### Workflow

When a courier is dispatched, he/she can either confirm or decline the restaurant's request. A courier is dispatched by the restaurant by finding a courier
that meets certain criteria based on locale and whether or not the courier is active. Once an appropriate courier is chosen, a request is sent to the chosen
courier so that can confirm or decline delivery of the order. If the courier confirms the request for delivery, they will subsequently head to the restaurant
location to pick up the order. If the courier declines the request, the restaurant must dispatch a new courier or cancel the order. An order can be canceled
by either the customer, restaurant, or courier at different points in the workflow. A customer or restaurant can cancel an order up until the point when the
order is picked up by a courier. A courier can only cancel an order after he/she has picked up said order in route to the customer. When a courier requests
that an order be canceled, the restaurant must make a decision on whether or not dispatch a new courier or discard the order.

<br>

#### State Machine

From the above workflow, we have identified the following states:

| State | Description |
|---|---|
| **Dispatched** | This state represents the system choosing a courier to deliver the order based on proximity to the customer and current availability. When a courier has been chosen it is said to have been dispatched (or requested). |
| **Confirmed** | This state represents the courier having confirmed the dispatch, agreeing to deliver the order to the customer. |
| **Picked Up** | This state represents the courier having verified that they have picked up the order in route to the customer. |
| **Delivered** | This is a final state representing the courier having delivered the order to the customer. |
| **Declined** | This is a final state representing the courier declining the dispatch. |
| **Canceled** | This is a final state representing the restaurant or customer canceling the order. From the courier's perspective, a canceled or can only be initiated by the customer before pickup or at any time by the restaurant. |
|  |  |

![Courier State Machine Diagram](CourierStateMachine.png)

**Figure 3**

MassTransit provides us a nice API to represent states and behaviors. Here is an code snippet taken from CourierStateMachine.cs.

```c#
...
During(Dispatched,
    When(CourierConfirmed)
        .Activity(x => x.OfType<CourierConfirmationActivity>())
        .TransitionTo(Confirmed),
...
```

<br>

###### Wanna know more about creating state machines in MassTransit?

Check the below links:
[MassTransit Automatonymous](https://masstransit-project.com/usage/sagas/automatonymous.html#automatonymous)

<br>

####  Orchestration/Choreography
The state machine diagram in Figure 3 represents a rendering of how states are transitioned by certain events. Figure 4 represents how the various tasks are
orchestrated so that state transitions can take place.

![Courier State Machine with Consumers Diagram](CourierStateMachineWithConsumers.png)

**Figure 4**




