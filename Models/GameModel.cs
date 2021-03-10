using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MathNet.Numerics;

public class GameModel
{
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

    public Stack[] Stacks = new Stack[0];

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
                MaxCards = maxCard,
                StackNumber = i
            };
        }).ToArray();

    }

    public void DrawCard(PlayerCardType type)
    {
        foreach (var stack in Stacks)
        {
            if (stack.TryAddDrawedCard(type))
            {
                break;
            }
        }
    }

    public class Stack
    {
        public int StackNumber { get; set; }
        public int MaxCards { get; set; }
        public int RemainingCards { get { return MaxCards - drawnedCards.Count; } }
        private IList<PlayerCardType> drawnedCards = new List<PlayerCardType>();

        public double ChanceToDrawAggravation(int level){
            if(drawnedCards.Any(c => c == PlayerCardType.Aggravation)){
                return 0;
            }
            return 1;
            //return Combinatorics.Combinations(1,1)*Combinatorics.Combinations(RemainingCards-1, level-1)/Combinatorics.Combinations(RemainingCards, level);
        }

        public bool TryAddDrawedCard(PlayerCardType type)
        {
            if (drawnedCards.Count >= MaxCards)
            {
                return false;
            }
            drawnedCards.Add(type);
            return true;
        }
    }
}