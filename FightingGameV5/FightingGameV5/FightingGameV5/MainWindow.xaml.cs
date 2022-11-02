using FightingGameV5.ButtonFunctionality;
using FightingGameV5.Items;
using PlayerCsharp;
using EnemyCsharp;
using ObstacleCsharp;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FightingGameV5
{
    // <summary>
    // Interaction logic for MainWindow.xaml
    // </summary>
    // Note: These Comments arent just for coders, theyre also for people who want to find out how this game works
    public partial class MainWindow : Window 
    {
        SolidColorBrush blue = new(Color.FromRgb(0, 255, 255)); // Special Blue Attack: Do NOT move
        SolidColorBrush orange = new(Color.FromRgb(255, 128, 0)); // Special Orange Attack: DO move
        Player player = new();
        Enemy enemy = new(); // self-explanatory
        ButtonFunctionalityClass buttonFunctionality = new(); //does things when button is pressed
        DispatcherTimer TimeControl = new();
        int TimeSurvived = 0; //control enemy round time
        Obstacle[] obstacle = { new Obstacle(), new Obstacle(), new Obstacle(), new Obstacle(), new Obstacle(), new Obstacle(), new Obstacle(), new Obstacle() }; // List of all the Obstacles
        public BitmapImage RedMode = new(new Uri(@"Sprites\RedHeart.png", UriKind.Relative));
        public BitmapImage BlueMode = new(new Uri(@"Sprites\BlueHeart.png", UriKind.Relative)); // Color of the Soul
        public Item items = new Item();
        double speed; // Controls Obstacle Speed
        string modes = "Normal"; // Difficulty selection with Normal as failsafe default
        bool PlayerTurn = false; // Is it the Players turn?
        bool ActMenuOpen = false; // Is the Act Menu open?
        bool itemTabOpen = false; // Is the Item Tab open?
        bool Continue = false; // Did the Player already do something?
        bool isMoving = false; //Is the Soul sprite moving
        bool GameEnd = false; //Is the Game over?
        bool dealtDamage = false; //Did the Player deal Damage?
        bool Gamestart = true; //Did a new game start?
        bool OnlyOnce = true; //Is this the first game start of this session?
        public MainWindow()
        {
            InitializeComponent(); //Starts Window
        }
        public void KeyboardMovement(Key e) //Controls Movement
        {
            double leftMargin = Soul.Margin.Left;
            double topMargin = Soul.Margin.Top;
            double rightMargin = Soul.Margin.Right;
            double bottomMargin = Soul.Margin.Bottom;
            //Note: IsMoving isnt set to true here so that falling due to being blue is more fair in No Hit Mode
            if (e == Key.W) //Example: Moving up on W press
            {
                topMargin -= 2; // Reduce Margin from Top
                bottomMargin += 2; // increase Margin from Bottom
                if (topMargin < 10) // If its at the edge of the Box...
                {
                    topMargin = 10; // ...Margin from Top is the Width of the Border of the Box, ...
                    bottomMargin = FightBox.ActualHeight - 10 - Soul.ActualHeight; // ...Margin from the Bottom is the Height of the Box minus Width of the Border of the Box minus the height of the player Soul Sprite...
                    isMoving = false; //... and its not moving, so isMoving is false.
                }
            }
            if (e == Key.S)
            {
                topMargin += 2;
                bottomMargin -= 2;
                if (bottomMargin < 10)
                {
                    bottomMargin = 10;
                    topMargin = FightBox.ActualHeight - 10 - Soul.ActualHeight;
                    isMoving = false;
                }
            }
            if (e == Key.A)
            {
                leftMargin -= 2;
                rightMargin += 2;
                if (leftMargin < 10)
                {
                    leftMargin = 10;
                    rightMargin = FightBox.ActualWidth - 10 - Soul.ActualWidth;
                    isMoving = false;
                }
            }
            if (e == Key.D)
            {
                leftMargin += 2;
                rightMargin -= 2;
                if (rightMargin < 10)
                {
                    rightMargin = 10;
                    leftMargin = FightBox.ActualWidth - 10 - Soul.ActualWidth;
                    isMoving = false;
                }
            }
            Soul.Margin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin);
        }
        public void GameControl(object Object, EventArgs e)
        {
            isMoving = false; //If no button is pressed, theres nothing to move, so isMoving is set to false at the start.
            if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up)) //Is a button pressed? (example for up)
            {
                isMoving = true; //Yes, hes moving
                KeyboardMovement(Key.W); //go up to KeyboardMovement with the Key being W
            }
            else if (Soul.Source == BlueMode)
            {
                KeyboardMovement(Key.S); // pulls the Player down. Doesnt count as movement.
            }
            if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
            {
                isMoving = true;
                KeyboardMovement(Key.A);
            }
            if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
            {
                isMoving = true;
                KeyboardMovement(Key.S);
            }
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
            {
                isMoving = true;
                KeyboardMovement(Key.D);
            }
            obstaclePositionUpdate(); //Player updated, now going down to obstaclePositionUpdate
        }
        public void GameLogic()
        {
            GameEnd = false; // Game didnt end yet
            int i = 0; //this integer will be useful later
            TimeSurvived = 0; //TimeSurvived set to 0 due to the enemy's turn just beginning
            Random random = new(); // random chance
            int FiftyFifty = random.Next(0, 100); // Basically coin flip
            if (FiftyFifty >= 50)// Random Chance to be blue.
            {
                Soul.Source = BlueMode;
            }
            else
            {
                Soul.Source = RedMode;
            }
            Soul.Visibility = Visibility.Visible; // Soul is set to Visible
            if (Gamestart) // Did a new game start?
            {
                string currentDir = Directory.GetCurrentDirectory(); //get current directory
                Uri EasyMode = new(currentDir + @"\EasyMode.mp3", UriKind.RelativeOrAbsolute); // Music files
                Uri NormalMode = new(currentDir + @"\NormalMode.mp3", UriKind.RelativeOrAbsolute);
                Uri HardMode = new(currentDir + @"\HardMode.mp3", UriKind.RelativeOrAbsolute);
                Uri NoHitMode = new(currentDir + @"\NoHitMode.mp3", UriKind.RelativeOrAbsolute);
                switch (modes) // Difficulty
                {
                    case "Normal":
                        player.hp = 50; //set players HP
                        mePlayer.Source = NormalMode; // set music
                        speed = 1.2; //set obstacle speed
                        break;
                    case "Easy":
                        player.hp = 100;
                        mePlayer.Source = EasyMode;
                        speed = 0.8;
                        break;
                    case "Hard":
                        player.hp = 20;
                        mePlayer.Source = HardMode;
                        speed = 1.5;
                        break;
                    case "No Hit":
                        player.hp = 1;
                        mePlayer.Source = NoHitMode;
                        speed = 2.0;
                        break;
                    default:
                        break;
                }
                mePlayer.Play(); // play Music
                mePlayer.MediaEnded += new RoutedEventHandler(Repeat); //Repeat at the end
                PlayerName.Content = NameOfthePlayer.Text; // Show the name of the Player
                MainMenu.Visibility = Visibility.Hidden; // Hide the main menu
                player.maxhp = player.hp; //Since HP may be over or under the previously set max, the Max HP is now equal to the Amount of HP you have
                player.TP = 0; // All TP is removed
                TPBar.Height = player.TP * 2; //TPBar is gone
                TPContent.Content = player.TP + "% TP"; // GUI now says "0% TP"
                HPContent.Content = player.hp + "/" + player.maxhp + " HP"; //GUI now says "X/X HP"
                Bar.Width = player.maxhp * 2; //Width of the Red Bar that appears when you lose HP
                HPBar.Width = player.hp * 2; //Width of the yellow Bar that disappears when you lose HP
                Soul.Visibility = Visibility.Visible; //The Soul must be Visible, better safe than sorry...
                if (OnlyOnce) //Is this the first game start of this session?
                {
                    TimeControl.Tick += new EventHandler(GameControl); // Does GameControl...
                    TimeControl.Interval = new TimeSpan(25000);        // ...every 25000 nanoseconds
                    OnlyOnce = false; //This was done once, so its no longer needed
                }
                enemy = new Enemy(); //New Enemy is created
                foreach (Button button in ItemGrid.Children) //Adding Items: Added 05.10.2022
                {
                    Item item = new Item(); //create Item
                    item.generateNewItem(); //generate Item
                    button.Content = "* " + item.getName(); // set Content of button to the Name of the Item
                }
                Gamestart = false; //Game is started, GameStart is now false
                foreach (Rectangle rectangle in AttackContainer.Children)
                {
                    obstacle[i].speed = speed; // Set the i-th obstacle's Speed stat to the Speed double.
                    if (i == 0)
                    {
                        rectangle.Fill = blue; // make it blue!
                    }
                    else if (i == 4)
                    {
                        rectangle.Fill = orange; // make it orange
                    }
                    i++;
                }
            }
            i = 0; 
            foreach (Rectangle rectangle in AttackContainer.Children)
            {
                double TopMargin = rectangle.Margin.Top; // TopMargin stays the same
                double LeftMargin = (FightBox.ActualWidth - 20) / 5 * (i + 1); // LeftMargin changes so that the obstacles are evenly spaced
                double RightMargin = FightBox.ActualWidth - LeftMargin - rectangle.Width; // RightMargin changes so that its in its correct position
                double BottomMargin = rectangle.Margin.Bottom; // BottomMargin stays the same
                rectangle.Margin = new Thickness(LeftMargin, TopMargin, RightMargin, BottomMargin); // new Margin is set
                rectangle.Visibility = Visibility.Visible; // Invisible obstacles wouldnt be fair, so this makes them visible 
                i++;
            }
            PlayerTurnText.Visibility = Visibility.Hidden; //Its not the Players turn, so no Player Turn Text is shown
            Soul.Margin = new Thickness(242, 95, 242, 95); // Positioning the Soul in the Middle
            TimeControl.Start(); // Start the Time
        }
        public void obstaclePositionUpdate()
        {
            int i = 0;
            foreach (Rectangle rectangle in AttackContainer.Children)
            {
                rectangle.Visibility = Visibility.Visible; //Just to make sure theyre visible
                double leftMargin = rectangle.Margin.Left;
                double topMargin = rectangle.Margin.Top;
                double rightMargin = rectangle.Margin.Right;
                double bottomMargin = rectangle.Margin.Bottom; // get the Margin from all sides
                if(enemy.spareprog >= 100) //Is the Enemy spareable?
                {
                    speed = 0.5; //Attacks slow down massivly.
                    obstacle[i].speed = speed;
                }
                if (player.hp > 0 && !GameEnd) //If the Game isnt over
                {
                    if ((leftMargin < Soul.Margin.Left + Soul.ActualWidth && leftMargin + rectangle.Width > Soul.Margin.Left && topMargin <= FightBox.ActualHeight - Soul.Margin.Bottom && obstacle[i].position == "Bottom") || (leftMargin < Soul.Margin.Left + Soul.ActualWidth && leftMargin + rectangle.Width > Soul.Margin.Left && bottomMargin <= FightBox.ActualHeight - Soul.Margin.Top && obstacle[i].position == "Top")) // Does the Soul collide with an obstacle?
                    {
                        if (player.BlockableAtks > 0) //Did the player defend?
                        {
                            player.BlockableAtks--; //Blockable Attacks went down by one
                        }
                        else if (rectangle.Fill == blue && !isMoving) // Is the player standing still for a blue attack?
                        {
                            player.hp -= 0; //no damage
                        }
                        else if (rectangle.Fill == orange && isMoving) // Is the player moving through an orange attack?
                        {
                            player.hp -= 0; //no damage
                        }
                        else
                        {
                            player.hp--; //deal damage
                            if (player.hp < 0)
                            {
                                player.hp = 0; //Cant have under 0 HP
                            }
                            HPBar.Width = 2 * player.hp; // shorten the bar
                            HPContent.Content = player.hp + "/" + player.maxhp + " HP"; // Change the tag 
                        }
                    }
                    if (obstacle[i].position == "Bottom") //Keep in Position
                    {
                        bottomMargin = 10;
                        topMargin = FightBox.ActualHeight - 10 - rectangle.ActualHeight;
                    }
                    else if (obstacle[i].position == "Top")
                    {
                        topMargin = 10;
                        bottomMargin = FightBox.ActualHeight - 10 - rectangle.ActualHeight;
                    }
                    if (obstacle[i].right) //Movement to the Right
                    {
                        if (rightMargin <= 10) //if it touches the Border...
                        {
                            obstacle[i] = new Obstacle(); //...new Obstacle with new Height is created...
                            obstacle[i].speed = speed;
                            leftMargin -= obstacle[i].speed;
                            rightMargin += obstacle[i].speed;
                            obstacle[i].right = false; //...which moves left.
                            rectangle.Width = obstacle[i].width;
                            rectangle.Height = obstacle[i].height; //sets height and width of Obstacle
                        }
                        else
                        {
                            leftMargin += obstacle[i].speed;
                            rightMargin -= obstacle[i].speed; //moves right
                        }
                    }
                    else //Movement to the Left is the same as to the Right, just with left and right swapped
                    {
                        if (leftMargin <= 10)
                        {
                            obstacle[i] = new Obstacle();
                            obstacle[i].speed = speed;
                            leftMargin += obstacle[i].speed;
                            rightMargin -= obstacle[i].speed;
                            obstacle[i].right = true;
                            rectangle.Width = obstacle[i].width;
                            rectangle.Height = obstacle[i].height;
                        }
                        else
                        {
                            leftMargin -= obstacle[i].speed;
                            rightMargin += obstacle[i].speed;
                        }
                    }
                }
                else if (player.hp <= 0 && !GameEnd) //If the player is dead...
                {
                    TimeControl.Stop();
                    mePlayer.Stop(); // Stop everything
                    Soul.Source = new BitmapImage(new Uri(@"Sprites\RedHeartShattered.png", UriKind.Relative)); //Heart is shattered
                    GameEnd = true; //Game is End
                    MainMenu.Visibility = Visibility.Visible; // Main Menu is Visible
                    Gamestart = true; //Gamestart is true
                    MessageBox.Show("Game Over", "You died"); // Game over is You
                }
                rectangle.Margin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin); //new Position
                i++;
            }
            if (!GameEnd) // if the Game isnt over.
            {
                TimeSurvived += 25000; //increase TimeSurvived by 15 Milliseconds
                if (mePlayer.Position == TimeSpan.MaxValue)// If mediaPlayer is finished
                {
                    Repeat(new Object(), new EventArgs()); // make it repeat
                }
            }
            if (TimeSurvived >= 100000000) //After one minute
            {
                TimeControl.Stop(); // Stop the Turn
                if (enemy.spareprog < 100) // If the Enemy isnt sparable...
                {
                    WindowsPresentationFoundation.Source = new BitmapImage(new Uri(@"Sprites\WPF_normalFace.png", UriKind.Relative)); // Give him the normal sprite
                }
                foreach (Rectangle rectangle in AttackContainer.Children) 
                {
                    rectangle.Visibility = Visibility.Hidden; // Hide Obstacles
                }
                Soul.Visibility = Visibility.Hidden; // Hide the Soul
                PlayerTurnText.Visibility = Visibility.Visible; // Show the Player Turn Text
                PlayerTurn = true; // It is the Players turn now.
            }
        }
        public void FightButton(object sender, RoutedEventArgs e) // Fighting
        {
            if (PlayerTurn && !GameEnd && !itemTabOpen && !ActMenuOpen) // Only works if its the Players turn, the Game isnt over, and no Menus are open
            {
                int hpBefore = enemy.hp; //Saves the HP of the Enemy, before the Damage is dealt
                enemy.hp = buttonFunctionality.Fight(player.atk, enemy.def, enemy.hp); // Reduces enemy hp
                if (enemy.hp <= 0) // If Enemy is dead
                {
                    WindowsPresentationFoundation.Source = new BitmapImage(new Uri(@"Sprites\WPF_DeathFace.png", UriKind.Relative)); //Show his death face
                    PlayerTurnText.Visibility = Visibility.Visible; // Keep the Turn Text visible
                    PlayerTurnText.Text = "YOU WON: You gained 32 EXP and 76 G"; // Make it display victory
                    mePlayer.Stop(); //Stop the Music
                }
                else //Enemy has 1 or more HP
                {
                    WindowsPresentationFoundation.Source = new BitmapImage(new Uri(@"Sprites\WPF_hitFace.png", UriKind.Relative)); // Show his hurt face
                    PlayerTurnText.Visibility = Visibility.Visible; // Keep the Turn Text visible
                    PlayerTurnText.Text = "* You dealt " + (hpBefore - enemy.hp) + " Damage"; //Show how much Damage was dealt
                    DialogueBox.Visibility = Visibility.Visible; // Show Enemy Dialouge
                    DialogueText.Text = "Oh, dangit, that hurt!"; // Set Enemy Dialouge
                    PlayerTurn = false; // It isnt Players Turn
                    Continue = true;  // Press Z to continue
                    dealtDamage = true; // You dealt Damage
                }
            }
        }
        public void DefendButton(object sender, RoutedEventArgs e) // Defending
        {
            if (PlayerTurn && !GameEnd && !itemTabOpen && !ActMenuOpen) // Only works if its the Players turn, the Game isnt over, and no Menus are open
            {
                if (!player.isDef) //Was the player defending last Turn?
                {
                    player.isDef = true; //For next turn
                    player.BlockableAtks = 25; //Free Hits.
                }
                player.TP = buttonFunctionality.defendTP(player.TP, player.def); // Increase TP
                TPBar.Height = player.TP * 2;
                TPContent.Content = Convert.ToString(player.TP) + "% TP"; //Show TP Increase
                PlayerTurn = false; // it isnt Players Turn
                PlayerTurnText.Visibility = Visibility.Visible; // Keep the Turn Text visible
                PlayerTurnText.Text = "* You defended, and gained some TP in the process."; // Show that you defended
                DialogueBox.Visibility = Visibility.Visible; // Show Enemy Dialouge
                if (enemy.spareprog < 100) // Is the Enemy ready to be spared?
                {
                    DialogueText.Text = "Hiding behind a defend wont make you win!"; // Set aggressive Enemy Dialouge
                }
                else
                {
                    DialogueText.Text = "...friend?"; // Set peaceful Enemy Dialouge
                }
                Continue = true; // Press Z to continue
            }
        }
        public void ItemButton(object sender, RoutedEventArgs e) // Using Items
        {
            if (PlayerTurn && !GameEnd && !ActMenuOpen) // Only works if its the Players turn, the Game isnt over, and no other Menus are open
            {
                if (!itemTabOpen) // Item Tab is Closed
                {
                    PlayerTurnText.Visibility = Visibility.Hidden; // Hide Turn text
                    ItemGrid.Visibility = Visibility.Visible; // Show Items
                    itemTabOpen = true; //Now its open
                }
                else if (itemTabOpen) // Item Tab is Open
                {
                    PlayerTurnText.Visibility = Visibility.Visible; // Show Turn Text
                    ItemGrid.Visibility = Visibility.Hidden; // Hide Items
                    itemTabOpen = false; //Now its closed
                }
                else // Item Tab is undefined
                {
                    PlayerTurnText.Visibility = Visibility.Hidden;
                    ItemGrid.Visibility = Visibility.Visible;
                    itemTabOpen = true; //just opening it
                }
            }
        }
        public void healing(object sender, RoutedEventArgs e) // Regenerating HP
        {
            Button button = sender as Button; // sender is a Button
            string HealingItem = Convert.ToString(button.Content); //The Item that heals
            HealingItem = HealingItem.Substring(2); // All we have to do is remove the "* "
            int hpBefore = player.hp; // Players HP before healing
            player.hp += items.getHeal(HealingItem); // Getting HP
            if (player.hp > player.maxhp) // If it would heal you over max...
            {
                player.hp = player.maxhp; // ...it heals you to max
            }
            HPBar.Width = player.hp * 2;
            HPContent.Content = Convert.ToString(player.hp) + "/" + player.maxhp + " HP"; // Show on the GUI that you healed
            button.Visibility = Visibility.Collapsed; // The Item is used up now
            ItemGrid.Visibility = Visibility.Hidden; // hide the Grid
            PlayerTurn = false; // Its no longer Players Turn
            itemTabOpen = false; // The Item Tab is now closed
            PlayerTurnText.Visibility = Visibility.Visible; // The Turn Text is visible
            PlayerTurnText.Text = "* You consumed the " + HealingItem + ". " + (player.hp - hpBefore) + " HP restored."; // Shows how much you healed
            DialogueBox.Visibility = Visibility.Visible; // Show Enemy Dialouge
            if (enemy.spareprog < 100)
            {
                DialogueText.Text = "You are gonna run out of items sometime!"; // Set aggresive Enemy Dialouge
            }
            else
            {
                DialogueText.Text = "May i have some?"; // Set peaceful Enemy Dialouge
            }
            Continue = true; // Press Z to continue
        }
        public void ActButton(object sender, RoutedEventArgs e) // Menu for Doing stuff
        {
            if (PlayerTurn && !GameEnd && !itemTabOpen) // Only works if its the Players turn, the Game isnt over, and no other Menus are open
            {
                if (!ActMenuOpen) // same as Item Tab, but with the Act Menu
                {
                    PlayerTurnText.Visibility = Visibility.Hidden;
                    ActGrid.Visibility = Visibility.Visible;
                    ActMenuOpen = true;
                }
                else if (ActMenuOpen)
                {
                    PlayerTurnText.Visibility = Visibility.Visible;
                    ActGrid.Visibility = Visibility.Hidden;
                    ActMenuOpen = false;
                }
                else
                {
                    PlayerTurnText.Visibility = Visibility.Hidden;
                    ActGrid.Visibility = Visibility.Visible;
                    ActMenuOpen = true;
                }
            }
        }
        public void Acting(object sender, RoutedEventArgs e) // Doing Stuff
        {
            Button button = sender as Button;
            string action = Convert.ToString(button.Content);
            action = action.Substring(2); // Same as Healing
            switch (action) // What Action is done
            {
                case "Check": // Checking Enemy Stats
                    {
                        ActGrid.Visibility = Visibility.Hidden;
                        PlayerTurnText.Visibility = Visibility.Visible;
                        PlayerTurnText.Text = "* Windows Presentation Foundation - " + enemy.hp + " HP, " + enemy.atk + " ATK, " + enemy.def + " DEF"; // Shows Enemy Stats
                        DialogueBox.Visibility = Visibility.Visible; // Show Enemy Dialouge
                        if (enemy.spareprog < 100)
                        {
                            DialogueText.Text = "Hey, checking is basically useless!"; // Set aggressive Dialogue
                        }
                        else
                        {
                            DialogueText.Text = "...friend?"; // Set peaceful dialouge
                        }
                        PlayerTurn = false;
                        ActMenuOpen = false; // ends Players turn
                        Continue = true; // Press Z to continue
                        break;
                    }
                case "Work harder":
                    {
                        enemy.spareprog += 10; // Increases Progress to Sparing the Enemy
                        ActGrid.Visibility = Visibility.Hidden;
                        PlayerTurnText.Visibility = Visibility.Visible;
                        PlayerTurnText.Text = "You worked harder... You understand now more of WPF";
                        DialogueBox.Visibility = Visibility.Visible; // Same as above
                        if(enemy.spareprog < 100)
                        {
                            DialogueText.Text = "Good attempt. Try harder next time!";
                        } else
                        {
                            DialogueText.Text = "Good Job... friend?";
                        }
                        PlayerTurn = false;
                        ActMenuOpen = false;
                        Continue = true; // same as any other Act
                        break;
                    }
                case "Tension Heal (30%)":
                    {
                        if (player.TP >= 30) // Only possible with enough TP
                        {
                            player.TP -= 30;
                            TPBar.Height = player.TP * 2;
                            TPContent.Content = player.TP + "% TP"; // Reduces TP
                            player.hp += items.getHeal("Tension Heal (30%)"); //Regenerates between 5 to 10 HP
                            if (player.hp > player.maxhp)
                            {
                                player.hp = player.maxhp; //Cant Overheal
                            }
                            HPBar.Width = player.hp * 2;
                            HPContent.Content = player.hp + "/" + player.maxhp + " HP";
                            ActGrid.Visibility = Visibility.Hidden;
                            PlayerTurnText.Visibility = Visibility.Visible;
                            PlayerTurnText.Text = "* You healed yourself using your Tension Points"; // Shows it on the GUI
                            DialogueBox.Visibility = Visibility.Visible;
                            DialogueText.Text = "Huh, seems im good at dealing damage"; // Shows Enemy Dialouge 
                            PlayerTurn = false;
                            ActMenuOpen = false;
                            Continue = true; // same as any other Act
                        }
                        break;
                    }
                case "Special Attack (50%)":
                    {
                        if (player.TP >= 50) // Only possible with enough TP
                        {
                            player.TP -= 50;
                            TPBar.Height = player.TP * 2;
                            TPContent.Content = player.TP + "% TP"; // Reduces TP
                            int hpBefore = enemy.hp;
                            enemy.hp = buttonFunctionality.specatk(player.TP, player.atk, enemy.def, enemy.hp);
                            if (enemy.hp <= 0)
                            {
                                WindowsPresentationFoundation.Source = new BitmapImage(new Uri(@"Sprites\WPF_DeathFace.png", UriKind.Relative));
                                ActGrid.Visibility = Visibility.Hidden;
                                PlayerTurnText.Text = "YOU WON: You gained 32 EXP and 76 G";
                                mePlayer.Stop();
                            }
                            else
                            {
                                WindowsPresentationFoundation.Source = new BitmapImage(new Uri(@"Sprites\WPF_hitFace.png", UriKind.Relative));
                                ActGrid.Visibility = Visibility.Hidden;
                                PlayerTurnText.Visibility = Visibility.Visible;
                                PlayerTurnText.Text = "* You dealt " + (hpBefore - enemy.hp) + " Damage";
                                DialogueBox.Visibility = Visibility.Visible;
                                DialogueText.Text = "Oh, dangit, that hurt!";
                                PlayerTurn = false;
                                Continue = true;
                                dealtDamage = true; // Basically Fight Button, but more damage
                            }
                        }
                        break;
                    }
            }
        }
        public void SpareButton(object sender, RoutedEventArgs e) // Showing Mercy
        {
            if (PlayerTurn && !GameEnd && !ActMenuOpen && !itemTabOpen) // Only works if its the Players turn, the Game isnt over, and no Menus are open
            {
                if (enemy.spareprog == 100)
                {
                    GameEnd = true;
                    PlayerTurnText.Text = "* YOU WON: You gained 0 EXP and 23 G";
                    mePlayer.Stop(); // Peaceful Victory
                }
                else
                {
                    PlayerTurnText.Text = "* You feel like you're gonna have to google everything.";
                    PlayerTurnText.Visibility = Visibility.Hidden;
                    GameLogic(); // Continues instantly
                }
            }
        }
        public void keyboardContinue(object sender, KeyboardEventArgs e)
        {
            if (Continue) // Has the Player finished his Turn?
            {
                if (Keyboard.IsKeyDown(Key.Z)) //Is the pressed Key Z?
                {
                    if (dealtDamage) // Was Damage dealt?
                    {
                        dealtDamage = false;
                        WindowsPresentationFoundation.Source = new BitmapImage(new Uri(@"Sprites\WPF_aggresiveFace.png", UriKind.Relative)); // Enemy is Very Angry
                    }
                    else if (enemy.spareprog == 100) // is Enemy sparable?
                    {
                        WindowsPresentationFoundation.Source = new BitmapImage(new Uri(@"Sprites\WPF_HappyFace.png", UriKind.Relative)); // Enemy is Happy
                    }
                    else
                    {
                        WindowsPresentationFoundation.Source = new BitmapImage(new Uri(@"Sprites\WPF_normalFace.png", UriKind.Relative)); // Enemy is Angry
                    }
                    PlayerTurnText.Text = "* You feel like you're gonna have to google everything.";
                    PlayerTurnText.Visibility = Visibility.Hidden;
                    DialogueBox.Visibility = Visibility.Hidden; // Hide everything
                    Continue = false; // Already continue'd
                    GameLogic(); // Enemy Turn begins
                }
            }
        }
        public void ModeButton(object sender, RoutedEventArgs e) // sets Difficulty based on button content
        {
            Button button = sender as Button;
            modes = (string)button.Content;
            GameLogic();
        }
        private void Repeat(object sender, EventArgs e) // repeats the Music
        {
            string currentDir = Directory.GetCurrentDirectory(); // The following is each piece of Music you can hear in the Game
            Uri EasyMode = new(currentDir + @"\EasyMode.mp3", UriKind.RelativeOrAbsolute);
            Uri NormalMode = new(currentDir + @"\NormalMode.mp3", UriKind.RelativeOrAbsolute);
            Uri HardMode = new(currentDir + @"\HardMode.mp3", UriKind.RelativeOrAbsolute);
            Uri NoHitMode = new(currentDir + @"\NoHitMode.mp3", UriKind.RelativeOrAbsolute);
            switch (modes) // Set source of Music by difficulty
            {
                case "Normal":
                    mePlayer.Source = NormalMode;
                    break;
                case "Easy":
                    mePlayer.Source = EasyMode;
                    break;
                case "Hard":
                    mePlayer.Source = HardMode;
                    break;
                case "No Hit":
                    mePlayer.Source = NoHitMode;
                    break;
                default:
                    break;
            }
            mePlayer.Play(); // Play the music.
        }
        public void Help(object sender, RoutedEventArgs e) //Gives help
        {
            MessageBox.Show(
                "Select Difficulty to start the Game.\n" +
                "WASD/Arrow keys to move the Heart.\n" +
                "If the Heart is blue, its getting pulled down.\n" +
                "If the Heart is Red, it is free to move anywhere.\n" +
                "Stand still through blue.\n" +
                "Move through Orange.\n" +
                "Avoid White entirely.\n" +
                "If your HP reaches 0, you lose.\n" +
                "Good Luck!", 
                "Help");
        }
    }
}