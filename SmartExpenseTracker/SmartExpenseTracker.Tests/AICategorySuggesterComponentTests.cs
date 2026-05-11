using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartExpenseTracker.Components;
using SmartExpenseTracker.Services;
using Xunit;

namespace SmartExpenseTracker.Tests
{
    public class AICategorySuggesterComponentTests : BunitContext
    {
        [Fact]
        public void ApplyingSuggestedCategory_NotifiesParent()
        {
            var categoryServiceMock = new Mock<IAICategorySuggestionService>();
            categoryServiceMock
                .Setup(s => s.SuggestCategoryAsync("Taxi ride", It.IsAny<CancellationToken>()))
                .ReturnsAsync("Transport");

            Services.AddSingleton(categoryServiceMock.Object);

            var appliedCategory = string.Empty;
            var cut = Render<AICategorySuggester>(parameters => parameters
                .Add(p => p.ExpenseDescription, "Taxi ride")
                .Add(p => p.OnCategoryApplied, category => appliedCategory = category));

            cut.Find("[data-testid='suggest-category']").Click();
            cut.WaitForAssertion(() => Assert.Contains("Suggested: Transport", cut.Markup));

            cut.Find("[data-testid='apply-category']").Click();

            Assert.Equal("Transport", appliedCategory);
        }
    }
}
