using System.Threading;
using System.Threading.Tasks;

namespace SmartExpenseTracker.Services
{
    public interface IAICategorySuggestionService
    {
        Task<string?> SuggestCategoryAsync(string description, CancellationToken cancellationToken = default);
    }
}
