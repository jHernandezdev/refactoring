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
    dynamic enrichPerformance(Performance aPerformance)
    {
        dynamic result = new ExpandoObject();
        result.PlayId = aPerformance.PlayId;
        result.Audience = aPerformance.Audience;
        result.play = playFor(aPerformance);
        result.amount = amoutFor(result);

        return result;
    }
    Play playFor(Performance aPerformance)
    {
        return plays.First(p => p.PlayId == aPerformance.PlayId);
    }
    int amoutFor(dynamic aPerformance)
    {
        int result = 0;
        switch (aPerformance.play.Type)
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
                throw new Exception($"Unknown type {aPerformance.play.Type}");
        }

        return result;
    }
    dynamic statementData = new ExpandoObject();
    statementData.customer = invoice.Customer;
    statementData.performances = invoice.Performances.Select(enrichPerformance);

    return renderPlainText(statementData, plays);
    
}

static string renderPlainText(dynamic statementData, List<Play> plays)
{
    
    decimal volumeCreditsFor(dynamic aPerformance)
    {
        decimal result = Math.Max(aPerformance.Audience - 30, 0);
        if (aPerformance.play.Type == "comedy")
            result += Math.Floor(aPerformance.Audience / 5m);
        return result;
    }
    string usd(decimal aNumber)
        => string.Format("{0:C}", aNumber / 100);
    decimal totalVolumeCredits()
    {
        decimal result = 0;
        foreach (dynamic perf in statementData.performances)
        {
            result += volumeCreditsFor(perf);
        }
        return result;
    }
    decimal totalAmount()
    {
        int result = 0;
        foreach (dynamic perf in statementData.performances)
        {
            result += perf.amount;
        }
        return result;
    }


    string result = $"Statement for {statementData.customer}\n";
    foreach (dynamic perf in statementData.performances)
    {
        result += $"\t{perf.play.Name}: {usd(perf.amount)} ({perf.Audience} seats)\n";
    }

    result += $"Amount owed is {usd(totalAmount())}\n";
    result += $"You earn {totalVolumeCredits()} credits\n";
    return result;
}



