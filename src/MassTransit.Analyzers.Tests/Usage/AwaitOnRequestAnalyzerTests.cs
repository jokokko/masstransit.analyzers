using MassTransit.Analyzers.Tests.Infrastructure;
using MassTransit.Analyzers.Usage;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace MassTransit.Analyzers.Tests.Usage
{
    public sealed class AwaitOnRequestAnalyzerTests
    {
        private readonly DiagnosticAnalyzer analyzer = new AwaitOnRequestAnalyzer();

        [Theory]
		[InlineData(@"await context.RespondAsync<SomeMessage>(context.Message, m => {});")]        
        [InlineData(@"await client.Request(context.Message);")]
        [InlineData(@"await context.Request<SomeMessage, SomeMessage>(null, new Uri(""loopback://none""), context.Message);")]
        [InlineData(@"var t = context.RespondAsync<SomeMessage>(context.Message, m => {});")]
        [InlineData(@"var t = client.Request(context.Message);")]
        [InlineData(@"var t = context.Request<SomeMessage, SomeMessage>(null, new Uri(""loopback://none""), context.Message);")]
        [InlineData(@"var r = client.Request(context.Message).Result;")]
		[InlineData(@"client.Request(context.Message).Wait();")]
        [InlineData(@"await context.Publish(context.Message);")]        
        [InlineData(@"await context.ScheduleSend(TimeSpan.FromTicks(0), context.Message);")]
        [InlineData(@"await context.Send(new Uri(""loopback://none""), context.Message);")]
        [InlineData(@"await context.Forward(new Uri(""loopback://none""));")]
        [InlineData(@"await context.Redeliver(TimeSpan.MaxValue);")]
        [InlineData(@"await context.ScheduleRecurringSend(new Uri(""loopback://none""), null, context.Message);")]
        [InlineData(@"await bus.Publish(context.Message);")]
		public async void AwaitOrAssignmentShouldNotTriggerAnalyzer(string call)
        {
            var diagnostics = await TestHelper.GetDiagnosticsAsync(analyzer,
@"using System;
using MassTransit;
using System.Threading.Tasks;

	public class TestClass : IConsumer<SomeMessage>
	{
		private readonly IRequestClient<SomeMessage, SomeMessage> client;
		private readonly IBusControl bus;

		public TestClass(IRequestClient<SomeMessage, SomeMessage> client, IBusControl bus)
		{
			this.client = client;
			this.bus = bus;
		}			
	
		public async Task Consume(ConsumeContext<SomeMessage> context)
		{
			" + call +@"		  
		}
}

public class SomeMessage {
}
");

	        Assert.Empty(diagnostics);
		}

	    [Theory]
	    [InlineData(@"context.RespondAsync<SomeMessage>(context.Message, m => {});")]
	    [InlineData(@"client.Request(context.Message);")]
	    [InlineData(@"context.Request<SomeMessage, SomeMessage>(null, new Uri(""loopback://none""), context.Message);")]
	    [InlineData(@"context.Publish(context.Message);")]	    
	    [InlineData(@"context.ScheduleSend(TimeSpan.FromTicks(0), context.Message);")]
	    [InlineData(@"context.Send(new Uri(""loopback://none""), context.Message);")]
	    [InlineData(@"context.Forward(new Uri(""loopback://none""));")]
	    [InlineData(@"context.Redeliver(TimeSpan.MaxValue);")]
	    [InlineData(@"context.ScheduleRecurringSend(new Uri(""loopback://none""), null, context.Message);")]
	    [InlineData(@"bus.Publish(context.Message);")]
		public async void NoAwaitOrAssignmentShouldTriggerAnalyzer(string call)
	    {
		    var diagnostics = await TestHelper.GetDiagnosticsAsync(analyzer,
				@"using System;
using MassTransit;
using System.Threading.Tasks;

	public class TestClass : IConsumer<SomeMessage>
	{
		private readonly IRequestClient<SomeMessage, SomeMessage> client;
		private readonly IBusControl bus;

		public TestClass(IRequestClient<SomeMessage, SomeMessage> client, IBusControl bus)
		{
			this.client = client;
			this.bus = bus;
		}
	
		public async Task Consume(ConsumeContext<SomeMessage> context)
		{
			" + call + @"		  
		}
}

public class SomeMessage {
}
");

			Assert.NotEmpty(diagnostics);

		    var d = diagnostics[0];

		    Assert.Equal(DiagnosticSeverity.Error, d.Severity);
		    Assert.Equal(17, d.Location.GetLineSpan().StartLinePosition.Line);			
	    }
	}
}
