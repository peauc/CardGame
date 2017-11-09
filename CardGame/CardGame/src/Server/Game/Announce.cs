namespace CardGame.Server.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CardGame.Protocol;

    public class Announce
    {
        public Announce(Player player, AnnounceType type, Card card)
        {
            this.Player = player;
            this.Type = type;
            this.Reward = this.Type.Score();

            if (this.Type == AnnounceType.Carre)
            {
                switch (card.Value)
                {
                    case Card.Types.Value.Jack:
                        this.Reward += 100;
                        break;

                    case Card.Types.Value.Nine:
                        this.Reward += 50;
                        break;
                }

                foreach (Card.Types.Type t in Enum.GetValues(typeof(Card.Types.Type)))
                {
                    this.CardsToValidate.Add(new AnnounceCard(new Card
                    {
                        Type = t,
                        Value = card.Value
                    }));
                }
            }
            else
            {
                Card.Types.Value[] values = (Card.Types.Value[])Enum.GetValues(typeof(Card.Types.Value));

                for (int i = (int)card.Value + this.Type.NumberOfCards() - 1; i >= (int)card.Value; i--)
                {
                    this.CardsToValidate.Add(new AnnounceCard(new Card
                    {
                        Type = card.Type,
                        Value = values[i]
                    }));
                }
            }
        }

        public Player Player { get; }

        public AnnounceType Type { get; }

        public List<AnnounceCard> CardsToValidate { get; set; }

        public int Reward { get; set; }

        public bool Validate(Card card)
        {
            foreach (AnnounceCard cardToValidate in this.CardsToValidate)
            {
                if (cardToValidate.Card.Value == card.Value && cardToValidate.Card.Type == card.Type)
                {
                    cardToValidate.Validated = true;
                    return true;
                }
                else if (this.Type.OrderMatters())
                {
                    return false;
                }
            }

            return false;
        }

        public bool IsComplete()
        {
            return this.CardsToValidate.All(card => card.Validated);
        }

        public int CompareTo(Announce other)
        {
            if (other.Type != this.Type)
            {
                return (int)other.Type - (int)this.Type;
            }
            else
            {
                return this.CardsToValidate[0].CompareTo(other.CardsToValidate[0]);
            }
        }

        public class AnnounceCard
        {
            public AnnounceCard(Card card)
            {
                this.Card = card;
                this.Validated = false;
            }

            public bool Validated { get; set; }

            public Card Card { get; }

            public int CompareTo(AnnounceCard other)
            {
                return this.Card.Value.CompareTo(other.Card.Value);
            }
        }
    }
}