using SmartExpenseTracker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartExpenseTracker.Services
{
    // Simple in-memory implementation suitable for demo and development.
    public class ExpenseService : IExpenseService
    {
        private readonly List<Expense> _items = new List<Expense>();
        private int _nextId = 1;

        public ExpenseService()
        {
            // seed with some data
            _items.Add(new Expense { Id = _nextId++, Description = "Coffee", Amount = 3.5m, Category = "Food", Date = System.DateTime.Now.AddDays(-1) });
            _items.Add(new Expense { Id = _nextId++, Description = "Bus fare", Amount = 2.25m, Category = "Transport", Date = System.DateTime.Now.AddDays(-2) });
            _items.Add(new Expense { Id = _nextId++, Description = "Movie", Amount = 12.0m, Category = "Entertainment", Date = System.DateTime.Now.AddDays(-3) });
        }

        public Task AddExpenseAsync(Expense expense)
        {
            expense.Id = _nextId++;
            _items.Add(expense);
            return Task.CompletedTask;
        }

        public Task<(IEnumerable<Expense> Items, int TotalCount)> GetPageAsync(int startIndex, int count)
        {
            var page = _items.Skip(startIndex).Take(count).ToList();
            return Task.FromResult((Items: (IEnumerable<Expense>)page, TotalCount: _items.Count));
        }

        public Task<decimal> GetTotalAmountAsync()
        {
            return Task.FromResult(_items.Sum(x => x.Amount));
        }
    }
}
