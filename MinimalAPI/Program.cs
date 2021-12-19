using System.Dynamic;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/statement", () =>
{
    Invoice invoice = new()
    {
        Customer = "BigCo",
        Performances = new List<Performance>()
        {
            new() {PlayId = "hamlet",Audience = 55},
            new() {PlayId = "as-like",Audience = 35},
            new() {PlayId = "othello",Audience = 40}

        }
    };

    List<Play> plays = new List<Play>()
    {
        new() { PlayId = "hamlet", Name = "Hamlet", Type = "tragedy"},
        new() { PlayId = "as-like", Name = "As You Like It", Type = "comedy"},
        new() { PlayId = "othello", Name = "Othello", Type = "tragedy"}
    };

    return statement(invoice, plays);
});

app.Run();

static string statement (Invoice invoice, List<Play> plays)
{
    dynamic statementData = new ExpandoObject();
    statementData.customer = invoice.Customer;

    return renderPlainText(statementData, invoice, plays);
    
}

static string renderPlainText(dynamic statementData, Invoice invoice, List<Play> plays)
{
    int amoutFor(Performance aPerformance)
    {
        int result = 0;
        switch (playFor(aPerformance).Type)
        {
            case "tragedy":
                result = 40000;
                if (aPerformance.Audience > 30)
                {
                    result += 1000 * (aPerformance.Audience - 20);
                }
                break;

            case "comedy":
                result = 30000;
                if (aPerformance.Audience > 20)
                {
                    result += 10000 + 500 * (aPerformance.Audience - 20);
                }
                break;

            default:
                throw new Exception($"Unknown type {playFor(aPerformance).Type}");
        }

        return result;
    }
    Play playFor(Performance aPerformance)
    {
        return plays.First(p => p.PlayId == aPerformance.PlayId);
    }
    decimal volumeCreditsFor(Performance aPerformance)
    {
        decimal result = Math.Max(aPerformance.Audience - 30, 0);
        if (playFor(aPerformance).Type == "comedy")
            result += Math.Floor(aPerformance.Audience / 5m);
        return result;
    }
    string usd(decimal aNumber)
        => string.Format("{0:C}", aNumber / 100);
    decimal totalVolumeCredits()
    {
        decimal result = 0;
        foreach (Performance perf in invoice.Performances)
        {
            result += volumeCreditsFor(perf);
        }
        return result;
    }
    decimal totalAmount()
    {
        int result = 0;
        foreach (Performance perf in invoice.Performances)
        {
            result += amoutFor(perf);
        }
        return result;
    }


    string result = $"Statement for {statementData.customer}\n";
    foreach (Performance perf in invoice.Performances)
    {
        result += $"\t{playFor(perf).Name}: {usd(amoutFor(perf))} ({perf.Audience} seats)\n";
    }

    result += $"Amount owed is {usd(totalAmount())}\n";
    result += $"You earn {totalVolumeCredits()} credits\n";
    return result;
}



