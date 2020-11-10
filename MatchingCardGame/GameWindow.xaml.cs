using MatchingCardGame.Controls;
using System;
using System.IO;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Markup;
using System.ComponentModel;
//using System.Xaml;

namespace MatchingCardGame
{
	/// <summary>
	/// Interaction logic for GameWindow.xaml
	/// </summary>
	public partial class GameWindow : Window, INotifyPropertyChanged
	{
		//interface: INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		//ctor
		public GameWindow()
		{
			InitializeComponent();
			dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
			dispatcherTimer.Tick += DispatcherTimer_Tick;
			GameMenu.Visibility = Visibility.Hidden;
			DataContext = this;
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

		public int moveCount = 0;  //++ on card2 click
		public int MoveCount
		{
			get { return moveCount; }
			set
			{
				if (value != moveCount)
				{
					moveCount = value;
					NotifyPropertyChanged("MoveCount");
				}
			}
		}
		public int maxTries = 1;   //set on run OR by user
		public int MaxTries
		{
			get { return maxTries; }
			set
			{
				if (value != maxTries)
				{
					maxTries = value;
					NotifyPropertyChanged("MaxTries");
				}
			}
		}
		public int matchCount = 0; //++ on success
		public int MatchCount
		{
			get { return matchCount; }
			set
			{
				if (value != matchCount)
				{
					matchCount = value;
					NotifyPropertyChanged("MatchCount");
				}
			}
		}
		public int failCount = 0;  //++ on fail
		public int FailCount
		{
			get { return failCount; }
			set
			{
				if (value != failCount)
				{
					failCount = value;
					NotifyPropertyChanged("FailCount");
				}
			}
		}

		public string Stats() 
		{
			return 
			"\n Moves Made:     " + moveCount +
			"\n Max Tries:      " + maxTries + 
			"\n Matches Made:   " + matchCount + 
			"\n Matches Failed: " + failCount + 
			"\n "+
			"\n "
			;
		}

		//timer methods
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
		//card methods
		public void RegisterCard(Card card)
		{
			card.State = Card.eState.IDLE;
			int r = random.Next(symbols.Count);
			card.Symbol = symbols[r];
			symbols.RemoveAt(r);
			cards.Add(card);
		}
		//gameplay methods
		public void SelectCard(Card card)
		{

			if (card1 == null)
			{
				//card clicked is the first card
				card1 = card;
				wav_Select.Play();
			}
			else
			{
				MoveCount++;
				//card clicked is the second card
				card2 = card;

				if (card1.Symbol == card2.Symbol)
				{
					//successful match
					MatchCount++;
					//sounds
					wav_YES.Play();
					//cards
					card1.State = Card.eState.MATCHED;
					card2.State = Card.eState.MATCHED;

					card1 = null;
					card2 = null;

					//check win conditions
					allowNewGame = Win();
				}
				else
				{
					//failed match
					FailCount++;
					//sounds
					wav_NO.Play();
					//cards
					foreach (var c in cards)
					{
						if (card.State == Card.eState.IDLE)
						{
							card.State = Card.eState.INACTIVE;
						}
					}
					dispatcherTimer.Start();
					allowNewGame = Loss();
				}
			}
		}
		private bool Win()
		{
			if (cards.Count/2 == matchCount)
			{
				MessageBox.Show("YOU WON");
				AllowNewGame();
				return true;
			}
			else
			{
				return false;
			}
		}
		private bool Loss()
		{
			if (moveCount == maxTries)
			{
				MessageBox.Show("YOU LOST");
				AllowNewGame();
				return true;
			}
			else
			{
				return false;
			}

		}
		//ui methods
		bool allowNewGame = false;
		public void AllowNewGame()
		{
			GameMenu.Visibility = Visibility.Visible;
		}
		private void Start_Click(object sender, RoutedEventArgs e)
		{

			if (allowNewGame)
			{
				//card.State = Card.eState.IDLE;
				//int r = random.Next(symbols.Count);
				//card.Symbol = symbols[r];
				//symbols.RemoveAt(r);
				//cards.Add(card);
				//int[] rows = new int[] { };
				//int[] cols = new int[] { };
				foreach (Card item in PlayingTable.Children)
                {
					item.State = Card.eState.IDLE;
                }
				
				card1 = null;
				card2 = null;
				MatchCount = 0;
				MoveCount = 0;
				FailCount = 0;
				NewGameMenu();
			}
		}
		private void NewGameMenu()
		{
			if (MaxTriesInput.Text != "")
			{
				MaxTries = int.Parse(MaxTriesInput.Text);
                if (MaxTries > 25)
                {
					MaxTries = 0;
                }
			}
			else
			{
				maxTries = 0;
			}
			GameMenu.Visibility = Visibility.Hidden;
		}
		//number validation
		private void NumberValidation(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		//music methods
		SoundPlayer wav_Select = new SoundPlayer(@"C:\Users\Asledorf Morvant\source\repos\MatchingCardGame\MatchingCardGame\Resources\Select.wav");
		SoundPlayer wav_NO = new SoundPlayer(@"C:\Users\Asledorf Morvant\source\repos\MatchingCardGame\MatchingCardGame\Resources\NO.wav");
		SoundPlayer wav_YES = new SoundPlayer(@"C:\Users\Asledorf Morvant\source\repos\MatchingCardGame\MatchingCardGame\Resources\YES.wav");
	}
}
