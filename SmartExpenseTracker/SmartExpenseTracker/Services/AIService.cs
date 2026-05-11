using OpenAI.Chat;
using SmartExpenseTracker.Models;
using System.Globalization;
using System.Text;

namespace SmartExpenseTracker.Services
{
    public class AIService : IAIService, IAICategorySuggestionService
    {
        private static readonly string[] SupportedCategories =
        {
            "Food",
            "Transport",
            "Entertainment",
            "Other"
        };

        private readonly IConfiguration _configuration;

        public AIService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetSuggestedCategory(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return "Other";
            }

            ChatCompletion completion = await CreateChatClient().CompleteChatAsync(
                new ChatMessage[]
                {
                    new SystemChatMessage(
                        "You categorize expenses for a personal finance app. " +
                        "Return exactly one category from this list: Food, Transport, Entertainment, Other. " +
                        "Do not include punctuation, explanation, or extra text."),
                    new UserChatMessage($"Expense description: {description.Trim()}")
                },
                new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 10
                });

            var suggestedCategory = completion.Content.FirstOrDefault()?.Text;
            return NormalizeCategory(suggestedCategory);
        }

        public async Task<string?> SuggestCategoryAsync(string description, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            ChatCompletion completion = await CreateChatClient().CompleteChatAsync(
                new ChatMessage[]
                {
                    new SystemChatMessage(
                        "You categorize expenses for a personal finance app. " +
                        "Return exactly one category from this list: Food, Transport, Entertainment, Other. " +
                        "Do not include punctuation, explanation, or extra text."),
                    new UserChatMessage($"Expense description: {description.Trim()}")
                },
                new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 10
                },
                cancellationToken);

            var suggestedCategory = completion.Content.FirstOrDefault()?.Text;
            return NormalizeCategory(suggestedCategory);
        }

        public async Task<string> GetSpendingInsights(List<Expense> expenses)
        {
            if (expenses.Count == 0)
            {
                return "No expenses are available to analyze.";
            }

            var expenseSummary = BuildExpenseSummary(expenses);

            ChatCompletion completion = await CreateChatClient().CompleteChatAsync(
                new ChatMessage[]
                {
                    new SystemChatMessage(
                        "You analyze personal expense data. " +
                        "Write concise, practical spending insights in 3 short bullet points."),
                    new UserChatMessage(expenseSummary)
                },
                new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 250
                });

            return completion.Content.FirstOrDefault()?.Text?.Trim()
                ?? "No spending insights were returned.";
        }

        private ChatClient CreateChatClient()
        {
            var apiKey = _configuration["OpenAI:ApiKey"]
                ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key is missing. Set OpenAI:ApiKey in configuration or the OPENAI_API_KEY environment variable.");
            }

            var model = _configuration["OpenAI:Model"];
            if (string.IsNullOrWhiteSpace(model))
            {
                model = "gpt-4.1-mini";
            }

            return new ChatClient(model, apiKey);
        }

        private static string NormalizeCategory(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return "Other";
            }

            var normalizedCategory = category.Trim().Trim('.', ',', ':', ';', '"', '\'');

            return SupportedCategories.FirstOrDefault(supportedCategory =>
                string.Equals(supportedCategory, normalizedCategory, StringComparison.OrdinalIgnoreCase))
                ?? "Other";
        }

        private static string BuildExpenseSummary(List<Expense> expenses)
        {
            var builder = new StringBuilder();
            builder.AppendLine(CultureInfo.InvariantCulture, $"Total expenses: {expenses.Sum(expense => expense.Amount):C}");
            builder.AppendLine("Spending by category:");

            foreach (var group in expenses.GroupBy(expense => expense.Category).OrderByDescending(group => group.Sum(expense => expense.Amount)))
            {
                builder.AppendLine(CultureInfo.InvariantCulture, $"- {group.Key}: {group.Sum(expense => expense.Amount):C}");
            }

            builder.AppendLine();
            builder.AppendLine("Recent expenses:");

            foreach (var expense in expenses.OrderByDescending(expense => expense.Date).Take(20))
            {
                builder.AppendLine(CultureInfo.InvariantCulture, $"- {expense.Date:yyyy-MM-dd}: {expense.Description} ({expense.Category}) {expense.Amount:C}");
            }

            return builder.ToString();
        }
    }
}
