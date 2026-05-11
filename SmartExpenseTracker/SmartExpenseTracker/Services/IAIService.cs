using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Services
{
    public interface IAIService
    {
        Task<string> GetSuggestedCategory(string description);
        Task<string> GetSpendingInsights(List<Expense> expenses);
    }
}
