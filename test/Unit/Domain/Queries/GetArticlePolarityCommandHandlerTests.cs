﻿using AJE.Domain.Ai;
using AJE.Domain.Entities;
using AJE.Domain.Enums;
using AJE.Domain.Queries;

namespace AJE.Test.Unit.Domain.Queries;

public class GetArticlePolarityQueryHandlerTests
{
    [Fact]
    public void Ok()
    {
        // arrange
        var mockContextCreator = new Mock<IContextCreator<Article>>();
        mockContextCreator.Setup(x => x.Create(It.IsAny<Article>())).Returns("context");

        var mockPolarity = new Mock<IPolarity>();
        mockPolarity.Setup(x => x.Create(It.IsAny<string>())).Returns("prompt");
        mockPolarity.Setup(x => x.Parse(It.IsAny<string>())).Returns(Polarity.Positive);

        var completionResponse = new CompletionResponse { };
        var mockAiModel = new Mock<IAiModel>();
        mockAiModel.Setup(x => x.CompletionAsync(It.IsAny<CompletionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(completionResponse);

        var articleId = Guid.NewGuid();
        var command = new GetArticlePolarityQuery { Article = new Article { Id = articleId } };
        var handler = new GetArticlePolarityQueryHandler(mockContextCreator.Object, mockPolarity.Object, mockAiModel.Object, new Mock<IAiLogger>().Object);

        // act
        var result = handler.Handle(command, CancellationToken.None).Result;

        // assert
        Assert.NotNull(result);
        Assert.Equal(articleId, result.Id);
        Assert.Equal(Polarity.Positive, result.Polarity);
    }
}