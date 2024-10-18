# ZEA.Applications.Workflows

This library provides a flexible and structured workflow system for managing complex processes in a step-by-step manner.
The workflow is designed to handle both successful operations and error cases, making it easier to manage sequential
tasks with proper error handling.

## Key Concepts

### 1. Workflow Class

The Workflow<TRequest, TContext, TResponse, TError> class is the base class for implementing workflows. It processes
a request and returns either a response or an error at the end of the workflow.

### 2. Workflow Steps

Each step in the workflow is implemented as a WorkflowStep<TContext, TError>. A step processes the current context
and either:

- Proceeds to the next step by returning an updated context, or
- Returns an error that stops the workflow.

## How to Use

### Step 1: Define a Workflow Request

Define a workflow request class that extends WorkflowRequest<TResponse, TError>. This request represents the starting
point of the workflow.

```csharp
public sealed class ProcessOrderRequest : WorkflowRequest<OrderResponseDto, OrderError>
{
    public string OrderId { get; }

    public ProcessOrderRequest(string orderId)
    {
        OrderId = orderId;
    }
}
```

### Step 2: Create a Workflow Step

Each step in the workflow inherits from ```WorkflowStep<TContext, TError>```. Steps perform specific actions in the
process
and either return an updated context or an error.

```csharp
public sealed class ValidateOrderStep : WorkflowStep<OrderContext, OrderError>
{
    public override async Task<Either<OrderContext, OrderError>> ExecuteAsync(
        OrderContext context, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(context.OrderId))
        {
            return Failure(new OrderError("Invalid order ID"));
        }

        // Simulate validation
        await Task.Delay(100);
        return Success(context);
    }
}
```

### Step 3: Implement the Workflow

The workflow handler orchestrates the steps by chaining them using `ThenAsync` from `EitherExtensions`. The handler
executes the steps in sequence, passing the updated context from one step to the next.

```csharp
public sealed class ProcessOrderWorkflow : Workflow<ProcessOrderRequest, OrderContext, OrderResponseDto, OrderError>
{
    private readonly ValidateOrderStep validateOrderStep;
    private readonly CheckInventoryStep checkInventoryStep;
    private readonly ProcessPaymentStep processPaymentStep;
    private readonly ShipOrderStep shipOrderStep;

    public ProcessOrderWorkflow(
        ValidateOrderStep validateOrderStep,
        CheckInventoryStep checkInventoryStep,
        ProcessPaymentStep processPaymentStep,
        ShipOrderStep shipOrderStep)
    {
        this.validateOrderStep = validateOrderStep;
        this.checkInventoryStep = checkInventoryStep;
        this.processPaymentStep = processPaymentStep;
        this.shipOrderStep = shipOrderStep;
    }

    public override async Task<Either<OrderResponseDto, OrderError>> Handle(
        ProcessOrderRequest request, 
        CancellationToken cancellationToken)
    {
        var context = new OrderContext(request.OrderId);

        var result = await validateOrderStep.ExecuteAsync(context, cancellationToken)
            .ThenAsync(checkInventoryStep, cancellationToken)
            .ThenAsync(processPaymentStep, cancellationToken)
            .ThenAsync(shipOrderStep, cancellationToken);
        
        return result.Match(
            c => Success(c.Response),
            Failure
        );
    }
}
```