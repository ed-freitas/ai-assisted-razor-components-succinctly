using SmartExpenseTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<(IEnumerable<Expense> Items, int TotalCount)> GetPageAsync(int startIndex, int count);
        Task AddExpenseAsync(Expense expense);
        Task<decimal> GetTotalAmountAsync();
    }
}
