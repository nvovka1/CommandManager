using MassTransit;
using Microsoft.Extensions.Logging;
using Nvovka.CommandManager.Contract.Messages;
using Xunit;
using Xunit.Abstractions;

namespace Nvovka.CommandManager.Tests.Consumers;


/// <summary>
/// /[Collection(nameof(TestsFixture))]
/// </summary>
public class CreateCommandConsumerTests: IClassFixture<TestServerFixture>
{
    private readonly TestServerFixture _testsFixture;

    public CreateCommandConsumerTests(ITestOutputHelper testOutputHelper, TestServerFixture testsFixture)
    {
        _testsFixture = testsFixture;
       // _testsFixture.Configure(testOutputHelper);
    }

    ////private readonly CustomWebApplicationFactory _factory;

    ////public CreateCommandConsumerTests(CustomWebApplicationFactory factory)
    ////{
    ////    _factory = factory;
    ////}

    [Fact]
    public async Task Consume_ShouldReceive_Message()
    {
        // Arrenge
        string testName = "Test";

        // Act
        await _testsFixture.TestHarness.InputQueueSendEndpoint.Send(new CreateCommandMessage(testName, "Tsts"));

        // var consumedMessage = _testsFixture.CreateCommandConsumer.Consumed
        //.Select<ICreateCommandMessage>(x => x.Context.Message.Name == testName)
        //.FirstOrDefault();

        var consumedMessage = _testsFixture.TestHarness.Consumed
       .Select<ICreateCommandMessage>(x => x.Context.Message.Name == testName)
       .FirstOrDefault();

        // Assert
        Assert.NotNull(consumedMessage);
    }
}
