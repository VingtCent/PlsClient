using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MathNet.Numerics;

namespace PlsClient.Models
{
    public class GameModel
    {
        private const int NumberOfAggravationPerStack = 1;
        internal const int NumberOfDrawnedCardPerTurn = 2;

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

        public bool Started { get; set; }

        public int TotalCards { get { return Cities + Refuelings + Aggravations; } }
        public int RemainingCards { get { return NumberOfCardPerStack.Sum(); } }

        public int PlayerCards { get; set; } = 8;

        public int[] NumberOfCardPerStack { get; set; } = new int[0];

        public int CurrentStack { get; set; } = 0;

        public IList<PlayerCardType> DrawnedCards { get; set; } = new List<PlayerCardType>();

        public bool IsAggravationDrawned { get; set; }

        public int NumberOfCardToDraw
        {
            get { return NumberOfDrawnedCardPerTurn - DrawnedCards.Count % NumberOfDrawnedCardPerTurn; }
        }

        public void Start()
        {
            Started = true;
            int div = (TotalCards - PlayerCards) / Aggravations;
            int mod = (TotalCards - PlayerCards) % Aggravations;
            NumberOfCardPerStack = Enumerable.Range(1, Aggravations).Select(i =>
            {
                var maxCard = div + ((mod == 0 || i >= mod) ? 0 : 1);
                return maxCard;
            }).ToArray();
        }

        public void DrawCard(PlayerCardType type)
        {
            DrawnedCards.Add(type);
            if (type == PlayerCardType.Aggravation)
            {
                IsAggravationDrawned = true;
            }

            NumberOfCardPerStack[CurrentStack]--;
            if (NumberOfCardPerStack[CurrentStack] == 0)
            {
                CurrentStack++;
                IsAggravationDrawned = false;
            }
        }

        public double ChanceToDrawAggravation()
        {
            if (IsAggravationDrawned)
            {
                return 0;
            }
            // https://www.dcode.fr/probabilites-tirage#0
            int m = NumberOfAggravationPerStack;
            int k = 1;
            int N = NumberOfCardPerStack[CurrentStack];
            int n = Math.Min(NumberOfCardPerStack[CurrentStack],  NumberOfCardToDraw);
            return Combinatorics.Combinations(m, k) * Combinatorics.Combinations(N - m, n - k) / Combinatorics.Combinations(N, n);
        }

        public void Back()
        {
            if (DrawnedCards.Count > 0)
            {
                if (DrawnedCards.Last() == PlayerCardType.Aggravation)
                {
                    IsAggravationDrawned = false;
                }

                DrawnedCards.RemoveAt(DrawnedCards.Count - 1);
                NumberOfCardPerStack[CurrentStack]++;

                
            }
        }
    }
}