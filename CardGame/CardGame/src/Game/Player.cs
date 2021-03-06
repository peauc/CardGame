﻿namespace Server.Game
{
    using System;
    using System.Linq;

    using CardGame.Protocol;

    using DotNetty.Transport.Channels;

    /// <summary>
    /// The player.
    /// </summary>
    public partial class Player
    {
        public Player(IChannelHandlerContext ctx, string name)
        {
            this.Ctx = ctx;
            this.Name = name;
            this.Hand = new Hand();
            this.Team = null;
            this.SetupForNewRound();
        }

        public IChannelHandlerContext Ctx { get; }

        public string Name { get; set; }

        public BeloteState Belote { get; set; }

        public BeloteState Rebelote { get; set; }

        public Hand Hand { get; private set; }

        public Team Team { get; set; }

        public void SetupForNewRound()
        {
            this.ClearHand();
            this.Belote = BeloteState.Undeclared;
            this.Rebelote = BeloteState.Undeclared;
        }

        public bool RemoveFromHand(Card card)
        {
            return this.Hand.Card.Remove(card);
        }

        public void ClearHand()
        {
            this.Hand.Card.Clear();
        }

        public bool SendMessage(Message message)
        {
            try
            {
                this.Ctx.WriteAndFlushAsync(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool SendHand()
        {
            try
            {
                return this.SendMessage(new Message { Type = Message.Types.Type.Hand, Hand = this.Hand });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Prompt(string toPrompt)
        {
            try
            {
                return this.SendMessage(
                    new Message
                    {
                        Type = Message.Types.Type.Prompt,
                        Prompt = new Prompt { ToDisplay = { toPrompt } }
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool Prompt(Prompt prompt)
        {
            try
            {
                return this.SendMessage(
                    new Message
                    {
                        Type = Message.Types.Type.Prompt,
                        Prompt = prompt
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool Reply(uint number, string message)
        {
            try
            {
                return this.SendMessage(
                    new Message
                    {
                        Type = Message.Types.Type.Reply,
                        Reply = new Reply { Number = number, Message = message }
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool Reply(Reply reply)
        {
            try
            {
                return this.SendMessage(
                    new Message
                    {
                        Type = Message.Types.Type.Reply,
                        Reply = reply
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool HasCard(Card card)
        {
            foreach (Card card1 in this.Hand.Card)
            {
                if (card.Value == card1.Value && card.Type == card1.Type)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasStartCard(Card.Types.Type startCardType)
        {
            return this.Hand.Card.Any(card => card.Type == startCardType);
        }

        public bool HasTrumpCard(Contract.Types.Type trumpType)
        {
            return this.Hand.Card.Any(card => card.Type.ToString() == trumpType.ToString());
        }

        public bool HasHigherTrumpCard(CardManager cm, Card other)
        {
            return this.Hand.Card.Any(card => cm.CurrentTrump.ToString() == card.Type.ToString() && cm.CompareCards(card, other) > 0);
        }
    }
}