using System;
using System.Collections.Generic;
using DotNetty.Transport.Channels;
using CardGame.Protocol;

namespace Client.Parsing
{
    public class Parser
    {
        //private BufferedReader _in = new BufferedReader(new InputStreamReader(System.in));
        private IChannel _channel { get; }
        private String _string;
        private Dictionary<String, int> _map = new Dictionary<string, int>();
        static private Dictionary<Card.Types.Type, String> _color = new Dictionary<CardGame.Protocol.Card.Types.Type, string>();
        static private Dictionary<CardGame.Protocol.Card.Types.Value, String> _number = new Dictionary<CardGame.Protocol.Card.Types.Value, string>();
        static private Dictionary<CardGame.Protocol.Announce.Types.Type, String> _announces = new Dictionary<CardGame.Protocol.Announce.Types.Type, string>();
        static private Dictionary<CardGame.Protocol.Contract.Types.Type, String> _colorContract = new Dictionary<CardGame.Protocol.Contract.Types.Type, string>();


        public Parser(IChannel channel)
        {
            Console.WriteLine("Constructor");
            _channel = channel;
            _map.Add("NAME", 0);
            _map.Add("HAND", 1);
            _map.Add("QUIT", 2);
            _map.Add("CONTRACT", 3);
            _map.Add("PASS", 4);
            _map.Add("COINCHE", 5);
            _map.Add("SURCOINCHE", 6);
            _map.Add("PLAY", 7);
            _map.Add("LAST", 8);
            _map.Add("ANNOUNCE", 9);
            _map.Add("BELOTE", 10);
            _map.Add("REBELOTE", 11);
            _map.Add("HELP", 12);
            _colorContract.Add(CardGame.Protocol.Contract.Types.Type.Diamonds, "DIAMONDS");
            _colorContract.Add(CardGame.Protocol.Contract.Types.Type.Hearts, "HEARTS");
            _colorContract.Add(CardGame.Protocol.Contract.Types.Type.Clubs, "CLUBS");
            _colorContract.Add(CardGame.Protocol.Contract.Types.Type.Spades, "SPADES");
            _colorContract.Add(CardGame.Protocol.Contract.Types.Type.Aa, "AA");
            _colorContract.Add(CardGame.Protocol.Contract.Types.Type.Na, "NA");
            _color.Add(CardGame.Protocol.Card.Types.Type.Hearts, "HEARTS");
            _color.Add(CardGame.Protocol.Card.Types.Type.Diamonds, "DIAMONDS");
            _color.Add(CardGame.Protocol.Card.Types.Type.Clubs, "CLUBS");
            _color.Add(CardGame.Protocol.Card.Types.Type.Spades, "SPADES");
            _number.Add(CardGame.Protocol.Card.Types.Value.Ace, "ACE");
            _number.Add(CardGame.Protocol.Card.Types.Value.King, "KING");
            _number.Add(CardGame.Protocol.Card.Types.Value.Queen, "QUEEN");
            _number.Add(CardGame.Protocol.Card.Types.Value.Jack, "JACK");
            _number.Add(CardGame.Protocol.Card.Types.Value.Ten, "TEN");
            _number.Add(CardGame.Protocol.Card.Types.Value.Nine, "NINE");
            _number.Add(CardGame.Protocol.Card.Types.Value.Eight, "EIGHT");
            _number.Add(CardGame.Protocol.Card.Types.Value.Seven, "SEVEN");
            _announces.Add(CardGame.Protocol.Announce.Types.Type.Carre, "CARRE");
            _announces.Add(CardGame.Protocol.Announce.Types.Type.Cent, "CENT");
            _announces.Add(CardGame.Protocol.Announce.Types.Type.Cinquante, "CINQUENTE");
            _announces.Add(CardGame.Protocol.Announce.Types.Type.Tierce, "TIERCE");
        }

        public Boolean ShouldParse()
        {
            try
            {
                return (false);
                //TODO
                // return (_in.ready());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return (false);
        }

        private void Read()
        {
            return;
            //TODO:
            //_string = _in.reradLine();
        }

        public void Parse()
        {
            try
            {
                Read();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            int i = -1;
            foreach (KeyValuePair<String, int> m in _map)
            {
                if (_string.Contains(m.Key))
                {
                    i = m.Value;
                }
            }
            _string = _string.Trim();
            switch (i)
            {
                case -1:
                    {
                        Console.WriteLine("Please input a valid command, type HELP to see the list");
                        break;
                    }
                case 0:
                    {
                        Name(_string);
                        break;
                    }
                case 1:
                    {
                        Hand();
                        break;
                    }
                case 2:
                    {
                        Environment.Exit(1);
                        break;
                    }
                case 3:
                    {
                        Contract(_string);
                        break;
                    }
                case 4:
                    {
                        Pass();
                        break;
                    }
                case 5:
                    {
                        CardGameFct();
                        break;
                    }
                case 6:
                    {
                        Surcoinche();
                        break;
                    }
                case 7:
                    {
                        Play(_string);
                        break;
                    }
                case 8:
                    {
                        Last();
                        break;
                    }
                case 9:
                    {
                        Announce(_string);
                        break;
                    }
                case 10:
                    {
                        Belote();
                        break;
                    }
                case 11:
                    {
                        Rebelote();
                        break;
                    }
                case 12:
                    {
                        PrintHelp();
                        break;
                    }
            }
        }

        private void Rebelote()
        {
            Message m = new Message()
            {
                Type = Message.Types.Type.Event,
                Event = new Event()
                {
                    Type = Event.Types.Type.Rebelote,
                }
            };
            _channel.WriteAndFlushAsync(m);
        }

        private void Belote()
        {
            Message m = new Message()
            {
                Type = Message.Types.Type.Event,
                Event = new Event()
                {
                    Type = Event.Types.Type.Belote,
                }
            };
            _channel.WriteAndFlushAsync(m);

        }

        private void Last()
        {

        }

        private void CardGameFct()
        {
            Message m = new Message()
            {
                Type = Message.Types.Type.Event,
                Event = new Event()
                {
                    Type = Event.Types.Type.Coinche,
                }
            };
            _channel.WriteAndFlushAsync(m);

        }

        private void Surcoinche()
        {
            Message m = new Message()
            {
                Type = Message.Types.Type.Event,
                Event = new Event()
                {
                    Type = Event.Types.Type.Surcoinche,
                }
            };
            _channel.WriteAndFlushAsync(m);
        }

        private void Pass()
        {
            Message m = new Message()
            {
                Type = Message.Types.Type.Event,
                Event = new Event()
                {
                    Type = Event.Types.Type.Pass
                }
            };
            _channel.WriteAndFlushAsync(m);
        }

        private void Contract(String str)
        {

            if (!Utils.HasArgument(str) || Utils.HasEnoughArgument(str, 2))
            {
                Console.Error.WriteLine("CONTRACT [score] [CLUBS-DIAMONDS-HEARTS-SPADES-AA-NA]");
                return;
            }
            str = Utils.GetArgument(str);

            int score = 0;
            Contract.Types.Type ContractType = CardGame.Protocol.Contract.Types.Type.Undefined;
            String[] input = str.Split(" ");
            try
            {
                if (!int.TryParse(input[0], out score))
                {
                    Console.WriteLine("Please input a valid number");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            foreach (KeyValuePair<Contract.Types.Type, String> e in _colorContract)
            {
                if (input[1] == e.Value)
                {

                    ContractType = e.Key;
                }
            }
            if (ContractType == CardGame.Protocol.Contract.Types.Type.Undefined)
            {
                Console.Error.WriteLine("Syntax error, type CONTRACT to have the correc syntax");
            }
            else
            {
                Message m = new Message()
                {
                    Type = Message.Types.Type.Event,
                    Event = new Event()
                    {
                        Type = Event.Types.Type.Contract,
                        Contract = new Contract
                        {
                            Type = ContractType,
                            Score = (uint)score,

                        }
                    }
                };
                _channel.WriteAndFlushAsync(m);
            }
        }

        private void Announce(String str)
        {
            Console.Out.WriteLine("Wait for an update to use ANNOUNCE !");
            if (str.Length == 0)
                return;
            str = Utils.GetArgument(str);
            String[] input = str.Split(" ");

            CardGame.Protocol.Card.Types.Type CardType = Card.Types.Type.Undefinedt;
            CardGame.Protocol.Announce.Types.Type AnnounceType = CardGame.Protocol.Announce.Types.Type.Undefined;
            CardGame.Protocol.Card.Types.Value CardValue = Card.Types.Value.Undefinedv;
            if (!Utils.HasArgument(str) || input.Length != 3)
            {
                Console.Error.WriteLine("ANNOUNCE [CARRE-CENT-CINQUANTE-TIERCE] [SEVEN-EIGHT-NINE-TEN-JACK-QUEEN-KING-ACE] [CLUBS-DIAMONDS-HEARTS-SPADES]");
                return;
            }
            foreach (KeyValuePair<CardGame.Protocol.Announce.Types.Type, String> e in _announces)
            {
                if (input[0] == e.Value)
                {
                    AnnounceType = e.Key;
                }
            }
            foreach (KeyValuePair<CardGame.Protocol.Card.Types.Value, String> e in _number)
            {
                if (input[1] == e.Value)
                {
                    CardValue = e.Key;
                }
            }
            foreach (KeyValuePair<CardGame.Protocol.Card.Types.Type, String> e in _color)
            {
                if (input[2] == e.Value)
                {
                    CardType = e.Key;
                }
            }

            if (AnnounceType == CardGame.Protocol.Announce.Types.Type.Undefined || CardType == Card.Types.Type.Undefinedt || CardType == Card.Types.Type.Undefinedt)
            {
                Console.Error.WriteLine("Syntax error, type ANNOUNCE to have the correc syntax");
            }
            else
            {
                if (CardValue != Card.Types.Value.Undefinedv)
                {
                    Message m = new Message()
                    {
                        Type = Message.Types.Type.Event,
                        Event = new Event()
                        {
                            Type = Event.Types.Type.Announce,
                            Announce = new CardGame.Protocol.Announce()
                            {
                                Type = AnnounceType,
                                Card = new Card()
                                {
                                    Value = CardValue,
                                    Type = CardType,
                                }
                            }
                        }
                    };
                    _channel.WriteAndFlushAsync(m);
                }
            }
        }



        private void Play(String str)
        {


            if (!Utils.HasArgument(str) || Utils.HasEnoughArgument(str, 2))
            {
                Console.Error.WriteLine("[SEVEN-EIGHT-NINE-TEN-JACK-QUEEN-KING-ACE] [CLUBS-DIAMONDS-HEARTS-SPADES]");
                return;
            }
            str = Utils.GetArgument(str);
            String[] input = str.Split(" ");
            CardGame.Protocol.Card.Types.Value v = Card.Types.Value.Undefinedv;
            CardGame.Protocol.Card.Types.Type t = Card.Types.Type.Undefinedt;
            foreach (KeyValuePair<CardGame.Protocol.Card.Types.Value, String> e in _number)
            {
                if (input[0] == e.Value)
                {

                    v = e.Key;
                }
            }
            foreach (KeyValuePair<CardGame.Protocol.Card.Types.Type, String> e in _color)
            {
                if (input[1] == e.Value)
                {
                    t = e.Key;
                }
            }
            if (t != Card.Types.Type.Undefinedt && v != Card.Types.Value.Undefinedv)
            {
                Message m = new Message()
                {
                    Type = Message.Types.Type.Event,
                    Event = new Event()
                    {
                        Type = Event.Types.Type.Play,
                        Card = new Card()
                        {
                            Type = t,
                            Value = v,
                        }
                    }
                };
                 _channel.WriteAndFlushAsync(m);
            }
            else
            {
                Console.Error.WriteLine("Syntax error, type PLAY to have the correct syntax");
            }
        }

        private void Name(String line)
        {
            String arguments;

            if (!Utils.HasArgument(line))
            {
                Console.Error.WriteLine("Name need an argument");
                return;
            }
            arguments = Utils.GetArgument(line);
            Message m = new Message()
            {
                Type = Message.Types.Type.Event,
                Event = new Event()
                {
                    Type = Event.Types.Type.Name,
                    Argument = {arguments}, 
                } 
            };
            _channel.WriteAndFlushAsync(m);
        }

        private void Hand()
        {
            Message m = new Message()
            {
                Type = Message.Types.Type.Event,
                Event = new Event()
                {
                    Type = Event.Types.Type.Hand,
                }
            };
            _channel.WriteAndFlushAsync(m);
        }

        private void PrintHelp()
        {
            Console.Out.WriteLine("NAME nickname\nHAND\nQUIT\nCONTRACT\nPASS\nCOINCHE\nSURCOINCHE\nPLAY\nLAST\nANNOUNCE\nBELOTE\nREBELOTE\nHELP");
        }
    }

}
