using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MatchingCardGame.Controls
{
    /// <summary>
    /// Interaction logic for Card.xaml
    /// </summary>
    public partial class Card : UserControl, INotifyPropertyChanged
    {
    //interface: INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    //Base Class
        //ctor
        public Card()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += Card_Loaded;
        }

        private void Card_Loaded(object sender, RoutedEventArgs e)
        {
            Owner = (GameWindow)Window.GetWindow(this);
            Owner.RegisterCard(this);
        }

        public enum eState
        {
            INACTIVE,
            IDLE,
            FLIPPED,    //card is flipped 
            MATCHED     //card has been matched
        }

        public GameWindow Owner { get; set; }

        private eState state = eState.INACTIVE;
        public eState State 
        { 
            get { return state; }
            set 
            {
                if (value != state)
                {
                    state = value;
                    Interactable = (state == eState.IDLE);
                    Show = (state == eState.FLIPPED || state == eState.MATCHED);
                    NotifyPropertyChanged("State");
                }
            } 
        }

        private bool interactable = true;
        public bool Interactable 
        {
            get { return interactable; }
            set
            {
                if (value != interactable)
                {
                    interactable = value;
                    NotifyPropertyChanged("Interactable");
                }
            }
        }
        
        private string symbol;
        public string Symbol 
        {
            get { return symbol; }
            set
            {
                if (value != symbol)
                {
                    symbol = value;
                    NotifyPropertyChanged("Symbol");
                }
            }
        }

        public bool Show 
        {
            set 
            {
                if (value)
                {
                    lblSymbol.Visibility = Visibility.Visible;
                    btnCard.Visibility = Visibility.Hidden;
                }
                else
                {
                    lblSymbol.Visibility = Visibility.Hidden;
                    btnCard.Visibility = Visibility.Visible;
                }
            } 
        }

        //click event
        private void btnCard_Click(object sender, RoutedEventArgs e)
        {
            State = eState.FLIPPED;
            Owner.SelectCard(this);
        }
    }
}
