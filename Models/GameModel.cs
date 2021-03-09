using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    public bool Started { get; set; }

    public int TotalCards { get { return Cities + Refuelings + Aggravations; } }
    public int RemainingCards { get { return TotalCards - playerCardDrawed.Count; } }

    public int PlayerCards { get{
        switch (Players)
        {
            case 2: return 8;
            case 3: return 9;
            case 4: return 8;
            default: throw new System.ArgumentOutOfRangeException("Players must be between 2 and 4");
        }
    } }

    public IEnumerable<Stack> Stacks
    {
        get
        {
            for (int i = 0; i < Aggravations; i++)
            {
                yield return new Stack{
                    MaxCards = ((TotalCards - PlayerCards) / Aggravations ) + ((i < TotalCards % Aggravations) ? 1 : 0)
                };
            }
        }
    }

    private IList<PlayerCardType> playerCardDrawed = new List<PlayerCardType>();

    public void DrawCard(PlayerCardType type)
    {
        playerCardDrawed.Add(type);
    }

    public class Stack
    {
        public int MaxCards { get; set; }
    }
}