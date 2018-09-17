---
title: MassTransit1000
description: Await or capture a messaging task from MassTransit
category: Usage
severity: Error
---

## Cause

A MassTransit messaging operation that returns a `Task` is not awaited on, or assigned to a variable.

The analyzer inspects invocations of the following methods
```
MassTransit.ConsumeContext.Request,
MassTransit.ConsumeContext.RespondAsync,
MassTransit.ConsumeContextExtensions.Forward,
MassTransit.ConsumeContextSelfSchedulerExtensions.ScheduleSend,
MassTransit.IPublishEndpoint.Publish,
MassTransit.IRequestClient.Request,
MassTransit.MessageRequestClient.Request,
MassTransit.PublishEndpointRecurringSchedulerExtensions.ScheduleRecurringSend,
MassTransit.RedeliverExtensions.Redeliver,
MassTransit.RequestExtensions.Request,
MassTransit.RespondAsyncExecuteExtensions.RespondAsync,
MassTransit.SendEndpointExtensions.Send
```

## Reason for rule

Not (a)waiting on the task can result in the message(s) involved being lost.

## How to fix violations

Await on the task or capture it in a variable that is waited on.

## Examples

### Violates

```csharp
public async Task Consume(ConsumeContext<SomeMessage> context)
{
	context.Publish(new IReceivedAMessage());
}
```

### Does not violate

```csharp
public async Task Consume(ConsumeContext<SomeMessage> context)
{
	await context.Publish(new IReceivedAMessage());
}

```
