using System.Collections.Generic;
using MinimalAPI;
using Xunit;

namespace MinimalAPI.Test;

public class MinimalAPIShould
{
    [Fact]
    public void GetResultFromInvoice()
    {
        Invoice invoice = new()
        {
            Customer = "BigCo",
            Performances = new List<Performance>()
        {
            new() {PlayId = "hamlet",Audience = 55},
            new() {PlayId = "as-like",Audience = 35},
            new() {PlayId = "",Audience = 40}

        }
        };

        List<Play> plays = new List<Play>()
        {
            new() { PlayId = "hamlet", Name = "Hamlet", Type = "tragedy"},
            new() { PlayId = "as-like", Name = "As You Like It", Type = "comedy"},
            new() { PlayId = "othello", Name = "Othello", Type = "tragedy"}
        };
        
    }
}
