using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Tests
{
    public class ExpenseValidationTests
    {
        private static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void ValidExpense_PassesValidation()
        {
            var expense = new Expense
            {
                Description = "Lunch",
                Amount = 9.99m,
                Date = DateTime.Now,
                Category = "Food"
            };

            var results = ValidateModel(expense);
            Assert.Empty(results);
        }

        [Fact]
        public void MissingDescription_FailsValidation()
        {
            var expense = new Expense
            {
                Description = string.Empty,
                Amount = 9.99m,
                Date = DateTime.Now,
                Category = "Food"
            };

            var results = ValidateModel(expense);
            Assert.Contains(results, r => r.MemberNames != null && r.MemberNames.Contains("Description"));
        }

        [Fact]
        public void InvalidAmount_FailsValidation()
        {
            var expense = new Expense
            {
                Description = "Stuff",
                Amount = 0m,
                Date = DateTime.Now,
                Category = "Other"
            };

            var results = ValidateModel(expense);
            Assert.Contains(results, r => r.MemberNames != null && r.MemberNames.Contains("Amount"));
        }

        [Fact]
        public void MissingCategory_FailsValidation()
        {
            var expense = new Expense
            {
                Description = "Coffee",
                Amount = 3.5m,
                Date = DateTime.Now,
                Category = string.Empty
            };

            var results = ValidateModel(expense);
            Assert.Contains(results, r => r.MemberNames != null && r.MemberNames.Contains("Category"));
        }
    }
}
