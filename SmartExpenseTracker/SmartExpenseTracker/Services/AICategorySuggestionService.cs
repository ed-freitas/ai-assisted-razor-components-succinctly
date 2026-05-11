using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartExpenseTracker.Services
{
    public class AICategorySuggestionService : IAICategorySuggestionService
    {
        private static readonly (string Category, string[] Keywords)[] CategoryRules =
        {
            ("Food", new[] { "coffee", "lunch", "dinner", "breakfast", "restaurant", "cafe", "grocery", "groceries", "snack", "meal" }),
            ("Transport", new[] { "bus", "train", "taxi", "uber", "lyft", "fare", "fuel", "gas", "parking", "metro", "subway" }),
            ("Entertainment", new[] { "movie", "cinema", "concert", "game", "theater", "show", "streaming", "music", "ticket" })
        };

        public Task<string?> SuggestCategoryAsync(string description, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return Task.FromResult<string?>(null);
            }

            var normalizedDescription = description.Trim().ToLowerInvariant();
            var category = CategoryRules
                .FirstOrDefault(rule => rule.Keywords.Any(normalizedDescription.Contains))
                .Category;

            return Task.FromResult<string?>(string.IsNullOrEmpty(category) ? "Other" : category);
        }
    }
}
