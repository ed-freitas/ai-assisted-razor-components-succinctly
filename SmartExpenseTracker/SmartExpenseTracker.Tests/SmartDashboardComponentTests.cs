using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartExpenseTracker.Components.Pages;
using SmartExpenseTracker.Models;
using SmartExpenseTracker.Services;
using Xunit;

namespace SmartExpenseTracker.Tests
{
    public class SmartDashboardComponentTests : BunitContext
    {
        [Fact]
        public void Dashboard_DisplaysCategoryTotalsAndAIInsights()
        {
            JSInterop.Mode = JSRuntimeMode.Loose;

            var expenses = new List<Expense>
            {
                new Expense { Id = 1, Description = "Coffee", Amount = 5m, Category = "Food", Date = DateTime.Today },
                new Expense { Id = 2, Description = "Bus fare", Amount = 3m, Category = "Transport", Date = DateTime.Today },
                new Expense { Id = 3, Description = "Lunch", Amount = 15m, Category = "Food", Date = DateTime.Today }
            };

            var expenseServiceMock = new Mock<IExpenseService>();
            expenseServiceMock
                .Setup(service => service.GetPageAsync(0, int.MaxValue))
                .ReturnsAsync((expenses, expenses.Count));

            var aiServiceMock = new Mock<IAIService>();
            aiServiceMock
                .Setup(service => service.GetSpendingInsights(It.IsAny<List<Expense>>()))
                .ReturnsAsync("Food is your largest spending category.");

            Services.AddSingleton(expenseServiceMock.Object);
            Services.AddSingleton(aiServiceMock.Object);

            var cut = Render<SmartDashboard>();

            cut.WaitForAssertion(() => Assert.Contains("Food is your largest spending category.", cut.Markup));
            Assert.Contains("Expenses by Category", cut.Markup);
            Assert.Contains("Food", cut.Markup);
            Assert.Contains("Transport", cut.Markup);
            Assert.Contains(20m.ToString("C"), cut.Markup);
            Assert.Contains(3m.ToString("C"), cut.Markup);
        }
    }
}
