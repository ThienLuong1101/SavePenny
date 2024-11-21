using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class AIReportsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AIReportsController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        
        _apiKey = configuration["ApiSettings:AI_API_Key"];

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    // GET: AIReports/Index
    public async Task<IActionResult> Index()
    {
        var transactions = await _context.Transactions.Include(t => t.Category).ToListAsync();
        return View(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateReport()
    {
        try
        {
            var transactions = await _context.Transactions.Include(t => t.Category).ToListAsync();
            if (transactions == null || !transactions.Any())
            {
                ViewData["Message"] = "No transactions available for analysis.";
                return View("Index", transactions);
            }

            string financialAdvice = await GetFinancialAdviceWithTransactions(transactions);
            ViewData["Message"] = financialAdvice;
            return View("Index", transactions);
        }
        catch (Exception ex)
        {
            ViewData["Message"] = $"Error generating report: {ex.Message}";
            return View("Index", new List<Transaction>());
        }
    }

    private async Task<string> GetFinancialAdviceWithTransactions(List<Transaction> transactions)
    {
        try
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "OpenAI API Key is not configured. Please add it to your application settings.";
            }

            if (transactions == null || !transactions.Any())
            {
                return "No transaction data available for analysis.";
            }

            var transactionsSummary = transactions
                .GroupBy(t => t.Category.Title)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount),
                    TransactionDetails = g.Select(t => new { t.Date, t.Amount })
                }).ToList();

            var formattedTransactions = string.Join("\n", transactionsSummary.Select(group =>
                $"Category: {group.Category}\n" +
                $"Total: {group.TotalAmount:C0}\n" +
                "Transactions:\n" +
                string.Join("\n", group.TransactionDetails.Select(t =>
                    $"  - Date: {t.Date:dd-MMM-yyyy}, Amount: {t.Amount:C0}")
                )
            ));

            var jsonContent = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful financial advisor." },
                    new { role = "user", content = $@"
Please analyze the following transactions and provide a summary and financial advice:

{formattedTransactions}

Please provide a concise summary of the spending behavior and specific recommendations for better financial management." }
                },
                max_tokens = 300
            };

            var response = await _httpClient.PostAsync(
                "v1/chat/completions",
                new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json")
            );

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Error from OpenAI API: {response.StatusCode} - {responseContent}";
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return data?.choices?[0]?.message?.content?.ToString()?.Trim()
                ?? "No valid response received from AI.";
        }
        catch (Exception ex)
        {
            return $"Error generating financial advice: {ex.Message}";
        }
    }
}