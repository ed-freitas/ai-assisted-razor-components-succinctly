using System;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using Moq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using SmartExpenseTracker.Components.Pages;
using SmartExpenseTracker.Services;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Tests
{
    public class ExpenseEntryComponentTests : BunitContext
    {
        [Fact]
        public void SubmittingValidForm_CallsAddExpenseAndNavigates()
        {
            // Arrange
            var expenseServiceMock = new Mock<IExpenseService>();
            expenseServiceMock.Setup(s => s.AddExpenseAsync(It.IsAny<Expense>())).Returns(Task.CompletedTask).Verifiable();

            Services.AddSingleton<IExpenseService>(expenseServiceMock.Object);
            Services.AddSingleton(Mock.Of<IAICategorySuggestionService>());

            var navMan = Services.GetRequiredService<BunitNavigationManager>();

            var cut = Render<ExpenseEntry>();

            // Act - fill form
            cut.Find("#description").Change("Test item");
            cut.Find("#amount").Change("12.34");
            cut.Find("#date").Change(DateTime.Now.ToString("yyyy-MM-dd"));
            cut.Find("#category").Change("Food");

            // submit
            cut.Find("form").Submit();

            // Assert
            expenseServiceMock.Verify(s => s.AddExpenseAsync(It.Is<Expense>(e => e.Description == "Test item" && e.Amount == 12.34m && e.Category == "Food")), Times.Once);
            Assert.Contains("/expenses", navMan.Uri);
        }

        [Fact]
        public void ApplyingSuggestedCategory_UpdatesCategorySelect()
        {
            var expenseServiceMock = new Mock<IExpenseService>();
            var categoryServiceMock = new Mock<IAICategorySuggestionService>();
            categoryServiceMock
                .Setup(s => s.SuggestCategoryAsync(It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync("Transport");

            Services.AddSingleton<IExpenseService>(expenseServiceMock.Object);
            Services.AddSingleton(categoryServiceMock.Object);

            var cut = Render<ExpenseEntry>();

            cut.Find("#description").Change("Taxi ride");
            cut.Find("[data-testid='suggest-category']").Click();
            cut.WaitForAssertion(() => Assert.Contains("Suggested: Transport", cut.Markup));

            cut.Find("[data-testid='apply-category']").Click();

            categoryServiceMock.Verify(s => s.SuggestCategoryAsync("Taxi ride", It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.Equal("Transport", cut.Find("#category").GetAttribute("value"));
        }
    }
}
