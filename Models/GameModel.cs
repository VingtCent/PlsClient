using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MathNet.Numerics;

public class GameModel
{
    private const int NumberOfAggravationPerStack = 1;
    internal const int NumberOfDrawnedCard = 2;

    [Required]
    [Range(1, int.MaxValue)]
    public int Cities { get; set; } = 48;

    [Required]
    [Range(0, int.MaxValue)]
    public int Refuelings { get; set; } = 5;

    [Required]
    [Range(1, int.MaxValue)]
    public int Aggravations { get; set; } = 5;

    [Required]
    [Range(2, 4)]
    public int Players { get; set; } = 4;
    public int Level { get; set; } = 2;

    public bool Started { get; set; }

    public int TotalCards { get { return Cities + Refuelings + Aggravations; } }
    public int RemainingCards { get { return Stacks.Sum(s => s.RemainingCards); } }

    public int PlayerCards { get; set; } = 8;

    public Stack[] Stacks { get; set; } = new Stack[0];

    public void Start()
    {
        Started = true;
        int div = (TotalCards - PlayerCards) / Aggravations;
        int mod = (TotalCards - PlayerCards) % Aggravations;
        Stacks = Enumerable.Range(1, Aggravations).Select(i =>
        {
            var maxCard = div + ((mod == 0 || i >= mod) ? 0 : 1);
            return new Stack
            {
                InitialNumberOfAggravation = NumberOfAggravationPerStack,
                MaxCards = maxCard,
                StackNumber = i                
            };
        }).ToArray();

        SetDrawnedCard();
    }

    private void SetDrawnedCard()
    {
        var cardToDraw = NumberOfDrawnedCard;
        foreach (var stack in Stacks)
        {
            var cardDrawable = Math.Min(stack.RemainingCards, cardToDraw);
            stack.NumberOfDrawnedCard = cardDrawable;
            cardToDraw -= cardDrawable;
        }
    }

    public void DrawCard(PlayerCardType type)
    {
        if (type == PlayerCardType.Aggravation)
        {
            Aggravations++;
        }

        foreach (var stack in Stacks)
        {
            if (stack.RemainingCards > 0)
            {
                stack.DrawCard(type);
                break;
            }
        }

        SetDrawnedCard();
    }

    public class Stack
    {
        public int InitialNumberOfAggravation { get; set; }
        public ICollection<PlayerCardType> DrawnedCards { get; set; } = new List<PlayerCardType>();

        public int NumberOfDrawnedCard { get; set; }
        public int StackNumber { get; set; }
        public int MaxCards { get; set; }
        public int RemainingCards { get { return MaxCards - DrawnedCards.Count; } }

        public double ChanceToDrawAggravation()
        {            
            // https://www.dcode.fr/probabilites-tirage#0
            int m = InitialNumberOfAggravation - DrawnedCards.Count(c => c == PlayerCardType.Aggravation);
            int k = 1;
            int N = RemainingCards;
            int n = NumberOfDrawnedCard;
            return Combinatorics.Combinations(m, k) * Combinatorics.Combinations(N - m, n - k) / Combinatorics.Combinations(N, n);
        }

        public void DrawCard(PlayerCardType type)
        {
            if (RemainingCards <= 0)
            {
                throw new IndexOutOfRangeException("No remaining card");
            }
            DrawnedCards.Add(type);
        }
    }
}