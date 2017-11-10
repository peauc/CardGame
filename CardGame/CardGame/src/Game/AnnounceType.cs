namespace Server.Game
{
    using System;

    using CardGame.Protocol;

    public enum AnnounceType
    {
        Carre,
        Cent,
        Cinquante,
        Tierce
    }

    public static class AnnounceTypeExtensions
    {
        public static Card.Types.Value MinimumCardValue(this AnnounceType self)
        {
            switch (self)
            {
                case AnnounceType.Carre:
                    return Card.Types.Value.Nine;

                case AnnounceType.Cent:
                    return Card.Types.Value.Jack;

                case AnnounceType.Cinquante:
                    return Card.Types.Value.Ten;

                case AnnounceType.Tierce:
                    return Card.Types.Value.Nine;

                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }
        }

        public static int NumberOfCards(this AnnounceType self)
        {
            switch (self)
            {
                case AnnounceType.Carre:
                    return 4;

                case AnnounceType.Cent:
                    return 5;

                case AnnounceType.Cinquante:
                    return 4;

                case AnnounceType.Tierce:
                    return 3;

                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }
        }

        public static int Score(this AnnounceType self)
        {
            switch (self)
            {
                case AnnounceType.Carre:
                    return 100;

                case AnnounceType.Cent:
                    return 100;

                case AnnounceType.Cinquante:
                    return 50;

                case AnnounceType.Tierce:
                    return 20;

                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }
        }

        public static bool OrderMatters(this AnnounceType self)
        {
            switch (self)
            {
                case AnnounceType.Carre:
                    return false;

                case AnnounceType.Cent:
                case AnnounceType.Cinquante:
                case AnnounceType.Tierce:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }
        }
    }
}