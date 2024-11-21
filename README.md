# Expense Tracker

A comprehensive expense tracking application built with ASP.NET Core MVC and SQL Server, featuring AI-powered financial insights and interactive dashboards.

## Features

### Dashboard
- Real-time financial overview of the last 7 days
- Visual representations of income and expenses using doughnut and spline charts
- Quick summary showing total income, expenses, and current balance
- Recent transactions list
- Category-wise expense breakdown

### Transaction Management
- Add, edit, and delete transactions
- Categorize transactions
- Add notes to transactions
- Date tracking for each transaction
- Category-based filtering

### Category Management
- Create and manage custom categories
- Assign icons to categories
- Categorize as income or expense
- Edit and delete categories

### AI-Powered Financial Reports
- Automated financial analysis using OpenAI GPT
- Personalized financial advice
- Spending pattern analysis
- Custom recommendations for better financial management

## Technology Stack

- **Backend**: ASP.NET Core MVC
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Bootstrap, JavaScript
- **AI Integration**: OpenAI GPT-3.5 API
- **Charts**: Syncfusion (or your chosen charting library)

## Setup Instructions

1. **Prerequisites**
   - Visual Studio 2022 or later
   - .NET 6.0 or later
   - SQL Server

## Database Setup and Migrations Guide

### 1. Install Required Packages
First, ensure you have these NuGet packages installed in your project:
```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
```

### 2. Configure Connection String
In `appsettings.json`, add your database connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ExpenseTracker;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Create Database Models
Ensure your models are properly configured. Key models include:

```csharp
public class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Required]
    public string Title { get; set; }
    public string Icon { get; set; }
    public string Type { get; set; }
}

public class Transaction
{
    [Key]
    public int TransactionId { get; set; }
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int Amount { get; set; }
    public string Note { get; set; }
    public DateTime Date { get; set; }
}
```

### 4. Create Initial Migration
Open Package Manager Console and run:
```powershell
# Create initial migration
Add-Migration InitialCreate

# Apply migration to database
Update-Database
```

### 5. Common Migration Commands
```powershell
# Create a new migration
Add-Migration MigrationName

# Remove last migration (if not applied to database)
Remove-Migration

# Apply migrations to database
Update-Database

# Generate SQL script (useful for production deployments)
Script-Migration
```

### 6. Troubleshooting Migrations

If you encounter issues:

1. **Migration not applying**:
   ```powershell
   Update-Database -Verbose
   ```

2. **Reset database**:
   ```powershell
   # Drop the database
   Drop-Database

   # Remove all migrations
   Remove-Migration -Force

   # Start fresh
   Add-Migration InitialCreate
   Update-Database
   ```

3. **Check migration status**:
   ```powershell
   Get-Migration
   ```


Then create and apply a new migration:
```powershell
Add-Migration SeedData
Update-Database
```

[Rest of the README remains the same...]

3. **Configuration**
   - Update `appsettings.json` with your database connection string
   - Add your OpenAI API key in configuration:
     ```json
     {
       "ApiSettings": {
         "AI_API_KEY": "your-api-key-here"
       }
     }
     ```

4. **Run the Application**
   - Build and run the application in Visual Studio
   - Navigate to `https://localhost:[port]`

## Project Structure

```
Expense_Tracker/
├── Controllers/
│   ├── DashboardController.cs
│   ├── CategoryController.cs
│   ├── HomeController.cs
│   ├── TransactionController.cs
│   └── AIReportsController.cs
├── Models/
│   ├── Category.cs
│   ├── ApplicationDbContext.cs
│   ├── ErrorViewModel.cs
│   └── Transaction.cs
├── Views/
│   ├── AIReport/index.cshtml
│   ├── Dashboard/index.cshtml
│   ├── Category/index.cshtml
│   └── Transaction/index.cshtml

```

## Key Features Implementation

### Dashboard
The dashboard provides a comprehensive overview of financial data including:
- 7-day financial summary
- Income vs Expense visualization
- Category-wise expense breakdown
- Recent transactions

### AI-Powered Analysis
The application integrates with OpenAI's GPT-3.5 to provide:
- Automated financial advice
- Spending pattern analysis
- Personalized recommendations

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
