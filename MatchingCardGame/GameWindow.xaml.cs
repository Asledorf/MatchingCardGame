using MatchingCardGame.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MatchingCardGame
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        //ctor
        public GameWindow()
        {
            InitializeComponent();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        //members & props
        private Random random = new Random();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private List<Card> cards = new List<Card>();
        private List<string> symbols = new List<string>()
        {
            "A","A","B","B","C","C",
            "D","D","E","E","F","F"
        };
        private Card card1 = null;
        private Card card2 = null;

        //methods
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            foreach (var c in cards)
            {
                if (c.State != Card.eState.MATCHED)
                {
                    c.State = Card.eState.IDLE;
                }
            }
            card1 = null;
            card2 = null;
        }
        public void RegisterCard(Card card)
        {
            card.State = Card.eState.IDLE;
            int r = random.Next(symbols.Count);
            card.Symbol = symbols[r];
            symbols.RemoveAt(r);
            cards.Add(card);
        }
        public void SelectCard(Card card)
        {
            if (card1 == null)
            {
                card1 = card;
            }
            else
            {
                card2 = card;
                if (card1.Symbol == card2.Symbol)
                {
                    card1.State = Card.eState.MATCHED;
                    card2.State = Card.eState.MATCHED;

                    card1 = null;
                    card2 = null;
                }
                else
                {
                    foreach (var c in cards)
                    {
                        if (card.State == Card.eState.IDLE)
                        {
                            card.State = Card.eState.INACTIVE;
                        }
                    }
                    dispatcherTimer.Start();
                }
            }
        }
    }
}
