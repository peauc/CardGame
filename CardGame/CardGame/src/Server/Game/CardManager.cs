namespace CardGame.Server.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CardGame.Protocol;

    /// <summary>
    /// The card manager.
    /// </summary>
    public partial class CardManager
    {
        public CardManager()
        {
            this.Cards = new List<Card>();
            this.CurrentTrump = Contract.Types.Type.Undefined;
            this.ScoreByValueByScaleDictionary = new Dictionary<ScoreScale, Dictionary<Card.Types.Value, int>>
                                                     {
                                                         {
                                                             ScoreScale.Standard,
                                                             new Dictionary<Card.Types.Value, int>
                                                                 {
                                                                     { Card.Types.Value.Ace, 11 },
                                                                     { Card.Types.Value.King, 4 },
                                                                     { Card.Types.Value.Queen, 3 },
                                                                     { Card.Types.Value.Jack, 2 },
                                                                     { Card.Types.Value.Ten, 10 },
                                                                     { Card.Types.Value.Nine, 0 },
                                                                     { Card.Types.Value.Eight, 0 },
                                                                     { Card.Types.Value.Seven, 0 }
                                                                 }
                                                         },
                                                         {
                                                             ScoreScale.Trump,
                                                             new Dictionary<Card.Types.Value, int>
                                                                 {
                                                                     { Card.Types.Value.Ace, 11 },
                                                                     { Card.Types.Value.King, 4 },
                                                                     { Card.Types.Value.Queen, 3 },
                                                                     { Card.Types.Value.Jack, 20 },
                                                                     { Card.Types.Value.Ten, 10 },
                                                                     { Card.Types.Value.Nine, 14 },
                                                                     { Card.Types.Value.Eight, 0 },
                                                                     { Card.Types.Value.Seven, 0 }
                                                                 }
                                                         },
                                                         {
                                                             ScoreScale.AllTrumps,
                                                             new Dictionary<Card.Types.Value, int>
                                                                 {
                                                                     { Card.Types.Value.Ace, 6 },
                                                                     { Card.Types.Value.King, 3 },
                                                                     { Card.Types.Value.Queen, 1 },
                                                                     { Card.Types.Value.Jack, 14 },
                                                                     { Card.Types.Value.Ten, 5 },
                                                                     { Card.Types.Value.Nine, 9 },
                                                                     { Card.Types.Value.Eight, 0 },
                                                                     { Card.Types.Value.Seven, 0 }
                                                                 }
                                                         },
                                                         {
                                                             ScoreScale.NoTrumps,
                                                             new Dictionary<Card.Types.Value, int>
                                                                 {
                                                                     { Card.Types.Value.Ace, 19 },
                                                                     { Card.Types.Value.King, 4 },
                                                                     { Card.Types.Value.Queen, 3 },
                                                                     { Card.Types.Value.Jack, 2 },
                                                                     { Card.Types.Value.Ten, 10 },
                                                                     { Card.Types.Value.Nine, 0 },
                                                                     { Card.Types.Value.Eight, 0 },
                                                                     { Card.Types.Value.Seven, 0 }
                                                                 }
                                                         }
                                                     };

            var types = new List<Card.Types.Type>
            {
                Card.Types.Type.Diamonds,
                Card.Types.Type.Hearts,
                Card.Types.Type.Clubs,
                Card.Types.Type.Spades
            };
            var values = new List<Card.Types.Value>
            {
                Card.Types.Value.Ace,
                Card.Types.Value.King,
                Card.Types.Value.Queen,
                Card.Types.Value.Jack,
                Card.Types.Value.Ten,
                Card.Types.Value.Nine,
                Card.Types.Value.Eight,
                Card.Types.Value.Seven
            };

            foreach (var type in types)
            {
                foreach (var value in values)
                {
                    this.Cards.Add(new Card { Type = type, Value = value });
                }
            }
        }

        public Contract.Types.Type CurrentTrump { get; set; }

        private List<Card> Cards { get; set; }

        private Dictionary<ScoreScale, Dictionary<Card.Types.Value, int>> ScoreByValueByScaleDictionary { get; }

        public void Mix()
        {
            var newCards = new List<Card>();
            var rnd = new Random();

            while (this.Cards.Any())
            {
                var randomNum = rnd.Next(0, this.Cards.Count);
                newCards.Add(this.Cards[randomNum]);
                this.Cards.RemoveAt(randomNum);
            }

            this.Cards = new List<Card>(newCards);
        }

        public void DistributeToAll(List<Player> players)
        {
            var numberCardsToDistribute = new List<int> { 3, 2, 3 };
            var cardsToDistribute = new List<Card>();

            cardsToDistribute.AddRange(this.Cards);
            for (var n = 0; n < 3; n++)
            {
                foreach (var player in players)
                {
                    for (var j = 0; j < numberCardsToDistribute[n]; j++)
                    {
                        player.Hand.Card.Add(cardsToDistribute[0]);
                        cardsToDistribute.RemoveAt(0);
                    }
                }
            }
        }

        public int GetCardScore(Card card)
        {
            if (this.CurrentTrump == Contract.Types.Type.Undefined)
            {
                return -1;
            }

            ScoreScale trumpType;
            if (card.Type.ToString() == this.CurrentTrump.ToString())
            {
                trumpType = ScoreScale.Trump;
            }
            else
            {
                switch (this.CurrentTrump)
                {
                    case Contract.Types.Type.Diamonds:
                    case Contract.Types.Type.Hearts:
                    case Contract.Types.Type.Clubs:
                    case Contract.Types.Type.Spades:
                        trumpType = ScoreScale.Standard;
                        break;

                    case Contract.Types.Type.Aa:
                        trumpType = ScoreScale.AllTrumps;
                        break;

                    default:
                        trumpType = ScoreScale.NoTrumps;
                        break;
                }
            }

            return this.ScoreByValueByScaleDictionary[trumpType][card.Value];
        }

        public int CompareCards(Card card1, Card card2)
        {
            if (card1.Type.ToString() == this.CurrentTrump.ToString()
                && card2.Type.ToString() != this.CurrentTrump.ToString())
            {
                return 1;
            }

            if (card2.Type.ToString() == this.CurrentTrump.ToString()
                && card1.Type.ToString() != this.CurrentTrump.ToString())
            {
                return -1;
            }

            return this.GetCardScore(card1) - this.GetCardScore(card2);
        }

        public Player GetCurrentTurnWinner(IDictionary<Player, Card> cards)
        {
            if (!cards.Any())
            {
                return null;
            }

            Player currentWinner = null;
            foreach (var card in cards)
            {
                if (currentWinner == null)
                {
                    currentWinner = card.Key;
                }
                else if (this.CompareCards(cards[currentWinner], card.Value) < 0)
                {
                    currentWinner = card.Key;
                }
            }

            return currentWinner;
        }

        public bool IsTrumpQueen(Card card)
        {
            return card.Type.ToString() == this.CurrentTrump.ToString() && card.Value == Card.Types.Value.Queen;
        }

        public bool IsTrumpKing(Card card)
        {
            return card.Type.ToString() == this.CurrentTrump.ToString() && card.Value == Card.Types.Value.King;
        }
    }
}