﻿using AJE.Domain.Data;
using AJE.Domain.Entities;
using AJE.Domain.Queries;

namespace AJE.Test.Unit.Domain.Queries;

public class GetAiChatQueryHandlerTests
{
    [Fact]
    public void Ok()
    {
        // arrange
        var myId = Guid.NewGuid();
        var mockAiChatRepository = new Mock<IAiChatRepository>();
        mockAiChatRepository.Setup(x => x.GetAsync(It.Is<Guid>(g => g == myId))).ReturnsAsync(new AiChat
        {
            ChatId = myId,
            StartTimestamp = DateTime.UtcNow,
            Interactions = new EquatableList<AiChatInteractionEntry>
            {
                new() {
                    InteractionId = Guid.NewGuid(),
                    InteractionTimestamp = DateTime.UtcNow,
                    Input = "User",
                    Output = "Ai"
                }
            }
        });
        var handler = new GetAiChatQueryHandler(mockAiChatRepository.Object);

        // act
        var result = handler.Handle(new GetAiChatQuery { Id = myId }, CancellationToken.None).Result;

        // assert
        Assert.NotNull(result);
        Assert.Equal(myId, result.ChatId);
        Assert.Single(result.Interactions);
        Assert.Equal("User", result.Interactions[0].Input);
        Assert.Equal("Ai", result.Interactions[0].Output);
    }
}