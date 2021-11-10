namespace PlsClient.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GameModel
    {
        private const int NumberOfAggravationPerStack = 1;
        internal const int NumberOfDrawnedCardPerTurn = 2;

        [Required] [Range(1, 48)] public int Cities { get; set; } = 48;

        [Required] [Range(0, 48)] public int Objectives { get; set; } = 0;

        [Required] [Range(0, int.MaxValue)] public int Refuelings { get; set; } = 5;

        [Required] [Range(1, int.MaxValue)] public int Aggravations { get; set; } = 5;

        [Required] [Range(2, 4)] public int Players { get; set; } = 4;

        public bool IsStarted { get; set; }

        public int TotalPlayerCards
        {
            get { return Cities + Refuelings + Aggravations - Objectives - PlayerCards; }
        }

        public int PlayerCards
        {
            get { return Players * 2; }
        }

        public IList<PlayerCardType> DrawnCards { get; set; } = new List<PlayerCardType>();

        public int NumberOfCardToDraw
        {
            get { return NumberOfDrawnedCardPerTurn - DrawnCards.Count % NumberOfDrawnedCardPerTurn; }
        }

        public IList<PlayerCardHeaps> PlayerCardHeaps { get; set; } = new List<PlayerCardHeaps>();

        public void Start()
        {
            IsStarted = true;
            var div = TotalPlayerCards / Aggravations;
            var mod = TotalPlayerCards % Aggravations;

            for (var i = 0; i < Aggravations; i++)
            {
                PlayerCardHeaps.Add(new PlayerCardHeaps
                {
                    InitialNumberOfCards = div + (i < mod ? 1 : 0)
                });
            }
        }

        public void Draw(PlayerCardType card)
        {
            DrawnCards.Add(card);
            foreach (var heap in PlayerCardHeaps)
            {
                if (heap.RemainingCards > 0)
                {
                    heap.DrawnCards++;
                    break;
                }
            }
        }

        public double ChanceToDrawAggravation()
        {
            throw new NotImplementedException();

            //if (IsAggravationDrawn)
            //{
            //    return 0;
            //}
            //// https://www.dcode.fr/probabilites-tirage#0
            //int m = NumberOfAggravationPerStack;
            //int k = 1;
            //int N = NumberOfCardPerStack[CurrentStack];
            //int n = Math.Min(NumberOfCardPerStack[CurrentStack],  NumberOfCardToDraw);
            //return Combinatorics.Combinations(m, k) * Combinatorics.Combinations(N - m, n - k) / Combinatorics.Combinations(N, n);
        }

        public void Back()
        {
            //if (DrawnCards.Count > 0)
            //{
            //    if (DrawnCards.Last() == PlayerCardType.Aggravation)
            //    {
            //        IsAggravationDrawn = false;
            //    }

            //    DrawnCards.RemoveAt(DrawnCards.Count - 1);
            //    NumberOfCardPerStack[CurrentStack]++;


            //}
        }
    }

    public class PlayerCardHeaps
    {
        public int InitialNumberOfCards { get; set; }

        public int RemainingCards
        {
            get { return InitialNumberOfCards - DrawnCards; }
        }

        public int DrawnCards { get; set; }
    }
}