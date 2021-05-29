using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Checker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Random random;
        private static object syncObj = new object();
        DispatcherTimer Timer;

        bool userEatCheked = false;
        bool game = false;
        bool isUserTurn = true;
        bool PCSteps = false;
        Border[][] allBorder;
        List<Ellipse> redCheckeredStones = new List<Ellipse>();
        List<Ellipse> whiteCheckeredStones = new List<Ellipse>();
        int[][] takingASeat;
        int redElementCount = 0;
        int whiteElementCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            InitRandomNumber(40);
            Timer = new DispatcherTimer();
            Timer.Tick += GameTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();

            int Cols = MyGrid.ColumnDefinitions.Count;
            int Rows = MyGrid.RowDefinitions.Count;
            allBorder = new Border[Rows][];
            takingASeat = new int[Rows][];
            for (int i = 0; i < allBorder.Length; i++)
            {
                takingASeat[i] = new int[Cols];
                allBorder[i] = new Border[Cols];
            }
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (game == false)
            {
                ThemeGame();
                game = true;
            }
            if (PCSteps == true)
            {
                PcSteps();
                isUserTurn = true;
                PCSteps = false;
            }
        }
        public void PcSteps()
        {
            for (int i = whiteCheckeredStones.Count - 1; i >= 0; i--)
            {
                EatStones(whiteCheckeredStones[i]);
            }
            if (PCSteps == true)
            {
                int colClicedField;
                int rowClicedField;
                for (int i = whiteCheckeredStones.Count - 1; i > 0; i--)
                {
                    bool ChangeStones = false;
                    int clickedField = GenerateRandomNumber(0, 3);
                    if (clickedField == 1)
                    {
                        colClicedField = Grid.GetColumn(whiteCheckeredStones[i]) - 1;
                        rowClicedField = Grid.GetRow(whiteCheckeredStones[i]) + 1;
                    }
                    else
                    {
                        colClicedField = Grid.GetColumn(whiteCheckeredStones[i]) + 1;
                        rowClicedField = Grid.GetRow(whiteCheckeredStones[i]) + 1;
                    }
                    if (colClicedField >= MyGrid.ColumnDefinitions.Count||colClicedField<=0 
                        || rowClicedField >= MyGrid.RowDefinitions.Count||rowClicedField<=0)
                        continue;
                    if (colClicedField == Grid.GetColumn(whiteCheckeredStones[i]) + 1)
                    {
                        if (rowClicedField == Grid.GetRow(whiteCheckeredStones[i]) + 1
                            && takingASeat[Grid.GetRow(whiteCheckeredStones[i]) + 1][Grid.GetColumn(whiteCheckeredStones[i]) + 1] == 0)
                        {
                            int previousCol = Grid.GetColumn(whiteCheckeredStones[i]);
                            int previousRow = Grid.GetRow(whiteCheckeredStones[i]);
                            Grid.SetColumn(whiteCheckeredStones[i], colClicedField);
                            Grid.SetRow(whiteCheckeredStones[i], rowClicedField);
                            takingASeat[previousRow][previousCol] = 0;
                            takingASeat[rowClicedField][colClicedField] = 1;
                            if (Grid.GetRow(whiteCheckeredStones[i]) != previousRow)
                                ChangeStones = true;
                        }
                    }
                    if (colClicedField < 0 || rowClicedField == MyGrid.RowDefinitions.Count + 1)
                        continue;
                    if (colClicedField == Grid.GetColumn(whiteCheckeredStones[i]) - 1)
                    {
                        if (rowClicedField == Grid.GetRow(whiteCheckeredStones[i]) + 1
                            && takingASeat[Grid.GetRow(whiteCheckeredStones[i]) + 1][Grid.GetColumn(whiteCheckeredStones[i]) - 1] == 0)
                        {
                            int previousCol = Grid.GetColumn(whiteCheckeredStones[i]);
                            int previousRow = Grid.GetRow(whiteCheckeredStones[i]);
                            Grid.SetColumn(whiteCheckeredStones[i], colClicedField);
                            Grid.SetRow(whiteCheckeredStones[i], rowClicedField);
                            takingASeat[previousRow][previousCol] = 0;
                            takingASeat[rowClicedField][colClicedField] = 1;
                            if (Grid.GetRow(whiteCheckeredStones[i]) != previousRow)
                                ChangeStones = true;
                        }
                    }
                    if (ChangeStones == true)
                        break;
                }
            }
        }

        private void ThemeGame()
        {
            for (int i = 0; i < allBorder.Length; i++)
            {
                for (int j = 0; j < allBorder[i].Length; j++)
                {
                    allBorder[i][j] = new Border();
                    allBorder[i][j].Background = Brushes.Black;
                    allBorder[i][j].BorderThickness = new Thickness(1, 1, 1, 1);
                    MyGrid.Children.Add(allBorder[i][j]);
                    Grid.SetColumn(allBorder[i][j], j);
                    Grid.SetRow(allBorder[i][j], i);
                    takingASeat[i][j] = 0;
                }
            }
            for (int i = 0; i < allBorder.Length; i++)
            {
                for (int j = 0; j < allBorder[0].Length; j++)
                {
                    i++;
                    j++;
                    if (i % 2 != 0)
                    {
                        if (j % 2 != 0)
                        {
                            i--;
                            j--;
                            allBorder[i][j].Background = Brushes.Gray;
                            allBorder[i][j].BorderThickness = new Thickness(1, 1, 1, 1);
                            Grid.SetColumn(allBorder[i][j], j);
                            Grid.SetRow(allBorder[i][j], i);
                        }
                        else
                        {
                            i--;
                            j--;
                            allBorder[i][j].Background = Brushes.Black;
                            allBorder[i][j].MouseLeftButtonDown += ClickField;
                            Grid.SetColumn(allBorder[i][j], j);
                            Grid.SetRow(allBorder[i][j], i);
                            CreateEllipse(i, j);
                        }
                    }
                    else
                    {
                        if (j % 2 != 0)
                        {
                            i--;
                            j--;
                            allBorder[i][j].Background = Brushes.Black;
                            allBorder[i][j].MouseLeftButtonDown += ClickField;
                            Grid.SetColumn(allBorder[i][j], j);
                            Grid.SetRow(allBorder[i][j], i);
                            CreateEllipse(i, j);
                        }
                        else
                        {
                            i--;
                            j--;
                            allBorder[i][j].Background = Brushes.Gray;
                            Grid.SetColumn(allBorder[i][j], j);
                            Grid.SetRow(allBorder[i][j], i);
                        }
                    }
                }
            }
        }
        public void CreateEllipse(int i, int j)
        {
            if (i < 3)
            {
                whiteCheckeredStones.Add(new Ellipse());
                whiteCheckeredStones[whiteElementCount].Fill = Brushes.White;
                whiteCheckeredStones[whiteElementCount].Width = 45;
                whiteCheckeredStones[whiteElementCount].Height = 45;
                MyGrid.Children.Add(whiteCheckeredStones[whiteElementCount]);
                Grid.SetColumn(whiteCheckeredStones[whiteElementCount], j);
                Grid.SetRow(whiteCheckeredStones[whiteElementCount], i);
                whiteElementCount++;
                takingASeat[i][j] = 1;
            }
            else if (i > 4)
            {
                redCheckeredStones.Add(new Ellipse());
                redCheckeredStones[redElementCount].Width = 45;
                redCheckeredStones[redElementCount].Height = 45;
                redCheckeredStones[redElementCount].Fill = Brushes.Red;
                redCheckeredStones[redElementCount].MouseLeftButtonDown += ClickStones;
                MyGrid.Children.Add(redCheckeredStones[redElementCount]);
                Grid.SetColumn(redCheckeredStones[redElementCount], j);
                Grid.SetRow(redCheckeredStones[redElementCount], i);
                redElementCount++;
                takingASeat[i][j] = 2;
            }
        }
        private void ClickField(object sender, RoutedEventArgs e)
        {
            if (isUserTurn == true)
            {
                var clickedField = ((Border)sender);
                if (clickedField.Background == Brushes.Green)
                {
                    int colClicedField = Grid.GetColumn(clickedField);
                    int rowClicedField = Grid.GetRow(clickedField);
                    clickedField.Background = Brushes.Black;
                    for (int i = 0; i < redCheckeredStones.Count; i++)
                    {
                        if (redCheckeredStones[i].Fill.ToString() == Brushes.Blue.ToString())
                        {
                            int previousCol = Grid.GetColumn(redCheckeredStones[i]);
                            int previousRow = Grid.GetRow(redCheckeredStones[i]);
                            Grid.SetColumn(redCheckeredStones[i], colClicedField);
                            Grid.SetRow(redCheckeredStones[i], rowClicedField);
                            redCheckeredStones[i].Fill = Brushes.Red;
                            if (colClicedField > 2)
                            {
                                allBorder[rowClicedField][colClicedField - 2].Background = Brushes.Black;
                            }
                            else if (colClicedField < MyGrid.ColumnDefinitions.Count - 2)
                            {
                                allBorder[rowClicedField][colClicedField + 2].Background = Brushes.Black;
                            }
                            allBorder[rowClicedField][colClicedField].Background = Brushes.Black;
                            if (previousRow - 2 == rowClicedField && previousCol - 2 == colClicedField)
                            {
                                for (int j = 0; j < whiteCheckeredStones.Count; j++)
                                {
                                    int row = Grid.GetRow(whiteCheckeredStones[j]);
                                    int col = Grid.GetColumn(whiteCheckeredStones[j]);
                                    if (previousRow - 1 == row && previousCol - 1 == col)
                                    {
                                        whiteCheckeredStones[j].Fill = Brushes.Transparent;
                                        takingASeat[row][col] = 0;
                                        MyGrid.Children.Remove(whiteCheckeredStones[j]);
                                        whiteCheckeredStones.Remove(whiteCheckeredStones[j]);
                                    }
                                }
                            }
                            if (previousRow - 2 == rowClicedField && previousCol + 2 == colClicedField)
                            {
                                for (int j = 0; j < whiteCheckeredStones.Count; j++)
                                {
                                    int row = Grid.GetRow(whiteCheckeredStones[j]);
                                    int col = Grid.GetColumn(whiteCheckeredStones[j]);
                                    if (previousRow - 1 == row && previousCol + 1 == col)
                                    {
                                        whiteCheckeredStones[j].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(whiteCheckeredStones[j]);
                                        whiteCheckeredStones.Remove(whiteCheckeredStones[j]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                            }
                            takingASeat[previousRow][previousCol] = 0;
                            takingASeat[rowClicedField][colClicedField] = 2;
                            isUserTurn = false;
                            PCSteps = true;
                            for (int j = 0; j < allBorder.Length; j++)
                            {
                                for (int k = 0; k < allBorder[j].Length; k++)
                                {
                                    if (allBorder[j][k].Background == Brushes.Green)
                                        allBorder[j][k].Background = Brushes.Black;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ClickStones(object sender, RoutedEventArgs e)
        {
            if (isUserTurn == true)
            {
                for (int i = 0; i < takingASeat.Length; i++)
                {
                    for (int j = 0; j < takingASeat[i].Length; j++)
                    {
                        if (takingASeat[i][j] == 0 && allBorder[i][j].Background != Brushes.Gray)
                        {
                            allBorder[i][j].Background = Brushes.Black;
                        }
                    }
                }
                var clickedElipse = ((Ellipse)sender);
                clickedElipse.Fill = new SolidColorBrush(Colors.Blue);
                foreach (Ellipse elipse in redCheckeredStones)
                {
                    if (elipse != null && elipse.Equals(clickedElipse) == false)
                    {
                        elipse.Fill = new SolidColorBrush(Colors.Red);
                    }
                }
                int col = Grid.GetColumn(clickedElipse);
                int row = Grid.GetRow(clickedElipse);
                if (col < MyGrid.RowDefinitions.Count - 1 && takingASeat[row - 1][col + 1] != 2 && takingASeat[row - 1][col + 1] != 1)
                {
                    allBorder[row - 1][col + 1].Background = Brushes.Green;
                }
                if (col > 0 && takingASeat[row - 1][col - 1] != 2 && takingASeat[row - 1][col - 1] != 1)
                {
                    allBorder[row - 1][col - 1].Background = Brushes.Green;
                }
                EatStonesCheked(clickedElipse);
            }
        }
        public void EatStonesCheked(Ellipse ellipse)
        {
            int col = Grid.GetColumn(ellipse);
            int row = Grid.GetRow(ellipse);
            if (col <= MyGrid.ColumnDefinitions.Count - 2 && takingASeat[row - 1][col + 1] == 1)
            {
                if (takingASeat[row - 2][col + 2] == 0)
                {
                    allBorder[row - 2][col + 2].Background = Brushes.Green;
                    userEatCheked = true;
                }
            }
            if (col >= 2 && takingASeat[row - 1][col - 1] == 1)
            {
                if (takingASeat[row - 2][col - 2] == 0)
                {
                    allBorder[row - 2][col - 2].Background = Brushes.Green;
                    userEatCheked = true;
                }
            }

        }
        public void EatStones(Ellipse ellipse)
        {
            int col = Grid.GetColumn(ellipse);
            int row = Grid.GetRow(ellipse);
            if (col < MyGrid.ColumnDefinitions.Count - 2 && takingASeat[row + 1][col + 1] == 2)
            {
                if (col < MyGrid.ColumnDefinitions.Count - 2 && row < MyGrid.RowDefinitions.Count - 2 &&
                    takingASeat[row + 2][col + 2] == 0)
                {

                    for (int j = 0; j < redCheckeredStones.Count; j++)
                    {
                        int rowRedStones = Grid.GetRow(redCheckeredStones[j]);
                        int colRedStones = Grid.GetColumn(redCheckeredStones[j]);
                        if (row + 1 == rowRedStones && col + 1 == colRedStones)
                        {
                            Grid.SetColumn(ellipse, col + 2);
                            Grid.SetRow(ellipse, row + 2);
                            takingASeat[row][col] = 0;
                            takingASeat[row + 2][col + 2] = 1;
                            redCheckeredStones[j].Fill = Brushes.Transparent;
                            MyGrid.Children.Remove(redCheckeredStones[j]);
                            redCheckeredStones.Remove(redCheckeredStones[j]);
                            takingASeat[row + 1][col + 1] = 0;
                            PCSteps = false;
                            isUserTurn = true;
                        }
                    }
                }
            }
            else if (col > 2 && takingASeat[row + 1][col - 1] == 2)
            {
                if (row < MyGrid.RowDefinitions.Count - 2 && col > 1 &&
                    takingASeat[row + 2][col - 2] == 0)
                {

                    for (int j = 0; j < redCheckeredStones.Count; j++)
                    {
                        if (redCheckeredStones[j] != null)
                        {
                            int rowRedStones = Grid.GetRow(redCheckeredStones[j]);
                            int colRedStones = Grid.GetColumn(redCheckeredStones[j]);
                            if (row + 1 == rowRedStones && col - 1 == colRedStones)
                            {
                                Grid.SetColumn(ellipse, col - 2);
                                Grid.SetRow(ellipse, row + 2);
                                takingASeat[row][col] = 0;
                                takingASeat[row + 2][col - 2] = 1;
                                redCheckeredStones[j].Fill = Brushes.Transparent;
                                MyGrid.Children.Remove(redCheckeredStones[j]);
                                redCheckeredStones.Remove(redCheckeredStones[j]);
                                takingASeat[row + 1][col - 1] = 0;
                                PCSteps = false;
                                isUserTurn = true;
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private static void InitRandomNumber(int seed)
        {
            random = new Random(seed);
        }

        private static int GenerateRandomNumber(int min, int max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random(); // Or exception...
                return random.Next(min, max);
            }
        }
    }
}
