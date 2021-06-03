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
        List<Ellipse> queenRedStones = new List<Ellipse>();
        List<Ellipse> queenWhiteStones = new List<Ellipse>();
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
            for (int i = 0; i < queenWhiteStones.Count; i++)
            {
                PcQueenStepsMove1(queenWhiteStones[i]);
                PcQueenStepsMove2(queenWhiteStones[i]);
                PcQueenStepsMove3(queenWhiteStones[i]);
                PcQueenStepsMove4(queenWhiteStones[i]);
            }
            for (int i = 0; i < queenWhiteStones.Count; i++)
            {
                PcQueenSteps(queenWhiteStones[i]);
            }
            for (int i = whiteCheckeredStones.Count - 1; i >= 0; i--)
            {
                PCEatStones(whiteCheckeredStones[i]);
            }
            if (PCSteps == true)
            {
                int nextPosstibleColumn;
                int nextPosstibleRow;
                for (int i = whiteCheckeredStones.Count - 1; i >= 0; i--)
                {
                    bool ChangeStones = false;
                    int clickedField = GenerateRandomNumber(0, 3);
                    if (clickedField == 1)
                    {
                        nextPosstibleColumn = Grid.GetColumn(whiteCheckeredStones[i]) - 1;
                        nextPosstibleRow = Grid.GetRow(whiteCheckeredStones[i]) + 1;
                    }
                    else
                    {
                        nextPosstibleColumn = Grid.GetColumn(whiteCheckeredStones[i]) + 1;
                        nextPosstibleRow = Grid.GetRow(whiteCheckeredStones[i]) + 1;
                    }
                    if (nextPosstibleColumn >= MyGrid.ColumnDefinitions.Count || nextPosstibleColumn <= 0
                        || nextPosstibleRow >= MyGrid.RowDefinitions.Count || nextPosstibleRow <= 0)
                        continue;
                    if (takingASeat[nextPosstibleRow][nextPosstibleColumn] == 0)
                    {
                        int previousCol = Grid.GetColumn(whiteCheckeredStones[i]);
                        int previousRow = Grid.GetRow(whiteCheckeredStones[i]);
                        Grid.SetColumn(whiteCheckeredStones[i], nextPosstibleColumn);
                        Grid.SetRow(whiteCheckeredStones[i], nextPosstibleRow);
                        takingASeat[previousRow][previousCol] = 0;
                        takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                        if (Grid.GetRow(whiteCheckeredStones[i]) != previousRow)
                            ChangeStones = true;
                    }
                    if (Grid.GetRow(whiteCheckeredStones[i]) == MyGrid.RowDefinitions.Count - 1)
                    {
                        queenWhiteStones.Add(whiteCheckeredStones[i]);
                        whiteCheckeredStones.Remove(whiteCheckeredStones[i]);
                    }
                    if (ChangeStones == true)
                        break;
                }
            }
        }
        private void PcQueenSteps(Ellipse ellipse)
        {
            if (PCSteps == true)
            {
                int nextPosstibleRow = Grid.GetRow(ellipse);
                int nextPosstibleColumn = Grid.GetColumn(ellipse);
                int clickedField = GenerateRandomNumber(1, 4);
                int previousRow = nextPosstibleRow;
                int previousCol = nextPosstibleColumn;
                if (clickedField == 1)
                {
                    PcQueenStepsMove1(ellipse);
                }
                if (clickedField == 2)
                {
                    PcQueenStepsMove2(ellipse);
                }
                if (clickedField == 3)
                {
                    PcQueenStepsMove3(ellipse);
                }
                else if (clickedField == 4)
                {
                    PcQueenStepsMove4(ellipse);
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
                            if (rowClicedField == 0)
                            {
                                queenRedStones.Add(redCheckeredStones[i]);
                                redCheckeredStones.Remove(redCheckeredStones[i]);
                            }

                            for (int k = 0; k < queenRedStones.Count; k++)
                            {
                                queenRedStones[k].MouseLeftButtonDown -= ClickStones;
                                queenRedStones[k].MouseLeftButtonDown += ClickQuennRedStones;
                                queenRedStones[k].Fill = Brushes.DarkRed;
                            }
                        }
                        else if (queenRedStones.Count > 0)
                        {
                            for (int x = 0; x < queenRedStones.Count; x++)
                            {
                                if (queenRedStones[x].Fill.ToString() == Brushes.Blue.ToString())
                                {
                                    int previousCol = Grid.GetColumn(queenRedStones[x]);
                                    int previousRow = Grid.GetRow(queenRedStones[x]);
                                    Grid.SetColumn(queenRedStones[x], colClicedField);
                                    Grid.SetRow(queenRedStones[x], rowClicedField);
                                    redCheckeredStones[x].Fill = Brushes.Red;
                                    for (int k = 0; k < MyGrid.RowDefinitions.Count - 1; k++)
                                    {
                                        if (previousCol - k > 0 && previousRow - k > 0
                                            && previousRow - k == rowClicedField && previousCol - k == colClicedField)
                                        {
                                            for (int j = 0; j < whiteCheckeredStones.Count; j++)
                                            {
                                                int row = Grid.GetRow(whiteCheckeredStones[j]);
                                                int col = Grid.GetColumn(whiteCheckeredStones[j]);
                                                if (previousRow - 1 == row && previousCol - 1 == col)
                                                {
                                                    if (colClicedField < col && rowClicedField < row)
                                                    {
                                                        whiteCheckeredStones[j].Fill = Brushes.Transparent;
                                                        takingASeat[row][col] = 0;
                                                        MyGrid.Children.Remove(whiteCheckeredStones[j]);
                                                        whiteCheckeredStones.Remove(whiteCheckeredStones[j]);
                                                    }
                                                }
                                            }
                                        }
                                        if (previousRow - k > 0 && previousCol + k < MyGrid.ColumnDefinitions.Count &&
                                            previousRow - k == rowClicedField && previousCol + k == colClicedField)
                                        {
                                            for (int j = 0; j < whiteCheckeredStones.Count; j++)
                                            {
                                                int row = Grid.GetRow(whiteCheckeredStones[j]);
                                                int col = Grid.GetColumn(whiteCheckeredStones[j]);
                                                if (previousRow - 1 == row && previousCol + 1 == col)
                                                {
                                                    if (colClicedField > col && rowClicedField < row)
                                                    {
                                                        whiteCheckeredStones[j].Fill = Brushes.Transparent;
                                                        MyGrid.Children.Remove(whiteCheckeredStones[j]);
                                                        whiteCheckeredStones.Remove(whiteCheckeredStones[j]);
                                                        takingASeat[row][col] = 0;
                                                    }
                                                }
                                            }
                                        }
                                        if (previousRow + k < MyGrid.ColumnDefinitions.Count && previousCol + k < MyGrid.ColumnDefinitions.Count &&
                                            previousRow + k == rowClicedField && previousCol + k == colClicedField)
                                        {
                                            for (int j = 0; j < whiteCheckeredStones.Count; j++)
                                            {
                                                int row = Grid.GetRow(whiteCheckeredStones[j]);
                                                int col = Grid.GetColumn(whiteCheckeredStones[j]);
                                                if (previousRow + 1 == row && previousCol + 1 == col)
                                                {
                                                    if (colClicedField > col && rowClicedField > row)
                                                    {
                                                        whiteCheckeredStones[j].Fill = Brushes.Transparent;
                                                        MyGrid.Children.Remove(whiteCheckeredStones[j]);
                                                        whiteCheckeredStones.Remove(whiteCheckeredStones[j]);
                                                        takingASeat[row][col] = 0;
                                                    }
                                                }
                                            }
                                        }
                                        if (previousRow + k < MyGrid.RowDefinitions.Count && previousCol - k > 0 &&
                                            previousRow + k == rowClicedField && previousCol - k == colClicedField)
                                        {
                                            for (int j = 0; j < whiteCheckeredStones.Count; j++)
                                            {
                                                int row = Grid.GetRow(whiteCheckeredStones[j]);
                                                int col = Grid.GetColumn(whiteCheckeredStones[j]);
                                                if (previousRow + 1 == row && previousCol - 1 == col)
                                                {
                                                    if (rowClicedField > row && colClicedField < col)
                                                    {
                                                        whiteCheckeredStones[j].Fill = Brushes.Transparent;
                                                        MyGrid.Children.Remove(whiteCheckeredStones[j]);
                                                        whiteCheckeredStones.Remove(whiteCheckeredStones[j]);
                                                        takingASeat[row][col] = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    takingASeat[previousRow][previousCol] = 0;
                                    takingASeat[rowClicedField][colClicedField] = 2;
                                    isUserTurn = false;
                                    PCSteps = true;
                                    for (int j = 0; j < allBorder.Length; j++)
                                    {
                                        for (int l = 0; l < allBorder[j].Length; l++)
                                        {
                                            if (allBorder[j][l].Background == Brushes.Green)
                                                allBorder[j][l].Background = Brushes.Black;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ClickQuennRedStones(object sender, RoutedEventArgs e)
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
                    if (elipse.Equals(clickedElipse) == false)
                    {
                        elipse.Fill = new SolidColorBrush(Colors.Red);
                    }
                }
                foreach (Ellipse elipse in queenRedStones)
                {
                    if (elipse.Equals(clickedElipse) == false)
                    {
                        elipse.Fill = new SolidColorBrush(Colors.Red);
                    }
                }
                int col = Grid.GetColumn(clickedElipse);
                int row = Grid.GetRow(clickedElipse);
                for (int i = 0; i < MyGrid.RowDefinitions.Count; i++)
                {
                    for (int j = 0; j < MyGrid.ColumnDefinitions.Count; j++)
                    {
                        if (row > 0 && col < MyGrid.RowDefinitions.Count - 1 && takingASeat[row - 1][col + 1] != 2 && takingASeat[row - 1][col + 1] != 1)
                        {
                            allBorder[row - 1][col + 1].Background = Brushes.Green;
                            row--; col++;
                        }
                        else if (row > 1 && col < MyGrid.RowDefinitions.Count - 2 && takingASeat[row - 1][col + 1] != 2 && takingASeat[row - 1][col + 1] == 1)
                        {
                            if (takingASeat[row - 2][col + 2] == 0)
                            {
                                allBorder[row - 2][col + 2].Background = Brushes.Green;
                            }
                        }
                    }
                }
                row = Grid.GetRow(clickedElipse);
                col = Grid.GetColumn(clickedElipse);
                for (int i = 0; i < MyGrid.RowDefinitions.Count; i++)
                {
                    for (int j = 0; j < MyGrid.ColumnDefinitions.Count; j++)
                    {
                        if (row > 0 && col > 0 && takingASeat[row - 1][col - 1] != 2 && takingASeat[row - 1][col - 1] != 1)
                        {
                            allBorder[row - 1][col - 1].Background = Brushes.Green;
                            row--; col--;
                        }
                        else if (row > 1 && col > 1 && takingASeat[row - 1][col - 1] != 2 && takingASeat[row - 1][col - 1] == 1)
                        {
                            if (takingASeat[row - 2][col - 2] == 0)
                            {
                                allBorder[row - 2][col - 2].Background = Brushes.Green;
                            }
                        }
                    }
                }
                row = Grid.GetRow(clickedElipse);
                col = Grid.GetColumn(clickedElipse);
                for (int i = 0; i < MyGrid.RowDefinitions.Count; i++)
                {
                    for (int j = 0; j < MyGrid.ColumnDefinitions.Count; j++)
                    {
                        if (row < MyGrid.RowDefinitions.Count - 1 && col < MyGrid.RowDefinitions.Count - 1
                            && takingASeat[row + 1][col + 1] != 2 && takingASeat[row + 1][col + 1] != 1)
                        {
                            allBorder[row + 1][col + 1].Background = Brushes.Green;
                            row++; col++;
                        }
                        else if (row < MyGrid.RowDefinitions.Count - 2 && col < MyGrid.RowDefinitions.Count - 2
                            && takingASeat[row + 1][col + 1] != 2 && takingASeat[row + 1][col + 1] == 1)
                        {
                            if (takingASeat[row + 2][col + 2] == 0)
                            {
                                allBorder[row + 2][col + 2].Background = Brushes.Green;
                            }
                        }
                    }
                }
                row = Grid.GetRow(clickedElipse);
                col = Grid.GetColumn(clickedElipse);
                for (int i = 0; i < MyGrid.RowDefinitions.Count; i++)
                {
                    for (int j = 0; j < MyGrid.ColumnDefinitions.Count; j++)
                    {
                        if (row < MyGrid.RowDefinitions.Count - 1 && col > 0 && takingASeat[row + 1][col - 1] != 2 && takingASeat[row + 1][col - 1] != 1)
                        {
                            allBorder[row + 1][col - 1].Background = Brushes.Green;
                            row++; col--;
                        }
                        else if (row < MyGrid.RowDefinitions.Count - 2 && col > 1 && takingASeat[row + 1][col - 1] != 2 && takingASeat[row + 1][col - 1] == 1)
                        {
                            if (takingASeat[row + 2][col - 2] == 0)
                            {
                                allBorder[row + 2][col - 2].Background = Brushes.Green;
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
                    if (elipse.Equals(clickedElipse) == false)
                    {
                        elipse.Fill = new SolidColorBrush(Colors.Red);
                    }
                }
                foreach (Ellipse elipse in queenRedStones)
                {
                    if (elipse.Equals(clickedElipse) == false)
                    {
                        elipse.Fill = new SolidColorBrush(Colors.Red);
                    }
                }
                int col = Grid.GetColumn(clickedElipse);
                int row = Grid.GetRow(clickedElipse);
                if (row >= 0 && col < MyGrid.RowDefinitions.Count - 1 && takingASeat[row - 1][col + 1] != 2 && takingASeat[row - 1][col + 1] != 1)
                {
                    allBorder[row - 1][col + 1].Background = Brushes.Green;
                }
                if (row >= 0 && col > 0 && takingASeat[row - 1][col - 1] != 2 && takingASeat[row - 1][col - 1] != 1)
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
            if (col < MyGrid.ColumnDefinitions.Count - 2 && takingASeat[row - 1][col + 1] == 1)
            {
                if (row != 1 && takingASeat[row - 2][col + 2] == 0)
                {
                    allBorder[row - 2][col + 2].Background = Brushes.Green;
                    userEatCheked = true;
                }
            }
            if (row != 1 && col >= 1 && takingASeat[row - 1][col - 1] == 1)
            {
                if (takingASeat[row - 2][col - 2] == 0)
                {
                    allBorder[row - 2][col - 2].Background = Brushes.Green;
                    userEatCheked = true;
                }
            }

        }
        public void PCEatStones(Ellipse ellipse)
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
            if (Grid.GetRow(ellipse) == MyGrid.RowDefinitions.Count - 1)
            {
                queenWhiteStones.Add(ellipse);
                whiteCheckeredStones.Remove(ellipse);
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
        private void PcQueenStepsMove1(Ellipse ellipse)
        {
            if (PCSteps == true)
            {
                int nextPosstibleRow = Grid.GetRow(ellipse);
                int nextPosstibleColumn = Grid.GetColumn(ellipse);
                int previousRow = nextPosstibleRow;
                int previousCol = nextPosstibleColumn;
                for (int k = 0; k < MyGrid.RowDefinitions.Count; k++)
                {
                    for (int j = 0; j < MyGrid.ColumnDefinitions.Count; j++)
                    {
                        if (nextPosstibleRow > 0 && nextPosstibleColumn < MyGrid.RowDefinitions.Count - 1 && takingASeat[nextPosstibleRow - 1][nextPosstibleColumn + 1] != 2 && takingASeat[nextPosstibleRow - 1][nextPosstibleColumn + 1] != 1)
                        {
                            takingASeat[nextPosstibleRow][nextPosstibleColumn] = 0;
                            nextPosstibleColumn++;
                            nextPosstibleRow--;
                            Grid.SetColumn(ellipse, nextPosstibleColumn);
                            Grid.SetRow(ellipse, nextPosstibleRow);
                            takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                        }
                        else if (nextPosstibleRow > 1 && nextPosstibleColumn < MyGrid.RowDefinitions.Count - 2 && takingASeat[nextPosstibleRow - 1][nextPosstibleColumn + 1] == 2 && takingASeat[nextPosstibleRow - 1][nextPosstibleColumn + 1] != 1)
                        {
                            if (takingASeat[nextPosstibleRow - 2][nextPosstibleColumn + 2] == 0)
                            {
                                for (int i = 0; i < redCheckeredStones.Count; i++)
                                {
                                    int row = Grid.GetRow(redCheckeredStones[i]);
                                    int col = Grid.GetColumn(redCheckeredStones[i]);
                                    if (row == nextPosstibleRow - 1 && col == nextPosstibleColumn + 1)
                                    {
                                        redCheckeredStones[i].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(redCheckeredStones[i]);
                                        redCheckeredStones.Remove(redCheckeredStones[i]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                                for (int i = 0; i < queenRedStones.Count; i++)
                                {
                                    int row = Grid.GetRow(queenRedStones[i]);
                                    int col = Grid.GetColumn(queenRedStones[i]);
                                    if (row == nextPosstibleRow - 1 && col == nextPosstibleColumn + 1)
                                    {
                                        redCheckeredStones[i].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(queenRedStones[i]);
                                        queenRedStones.Remove(queenRedStones[i]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                                takingASeat[nextPosstibleRow][nextPosstibleColumn] = 0;
                                nextPosstibleRow -= 2;
                                nextPosstibleColumn += 2;
                                Grid.SetColumn(ellipse, nextPosstibleColumn);
                                Grid.SetRow(ellipse, nextPosstibleRow);
                                takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                                goto link1 ;
                            }
                        }
                    }
                }
                link1:
                if (previousRow != Grid.GetRow(ellipse) && previousCol != Grid.GetColumn(ellipse))
                {
                    isUserTurn = true;
                    PCSteps = false;
                }
            }
        }
        private void PcQueenStepsMove2(Ellipse ellipse)
        {
            if (PCSteps == true)
            {
                int nextPosstibleRow = Grid.GetRow(ellipse);
                int nextPosstibleColumn = Grid.GetColumn(ellipse);
                int previousRow = nextPosstibleRow;
                int previousCol = nextPosstibleColumn;
                for (int k = 0; k < MyGrid.RowDefinitions.Count; k++)
                {
                    for (int j = 0; j < MyGrid.ColumnDefinitions.Count; j++)
                    {
                        if (nextPosstibleRow < MyGrid.RowDefinitions.Count - 1 && nextPosstibleColumn > 0 && takingASeat[nextPosstibleRow + 1][nextPosstibleColumn - 1] != 2 && takingASeat[nextPosstibleRow + 1][nextPosstibleColumn - 1] != 1)
                        {
                            takingASeat[nextPosstibleRow][nextPosstibleColumn] = 0;
                            nextPosstibleColumn--;
                            nextPosstibleRow++;
                            Grid.SetColumn(ellipse, nextPosstibleColumn);
                            Grid.SetRow(ellipse, nextPosstibleRow);
                            takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                        }
                        else if (nextPosstibleRow < MyGrid.RowDefinitions.Count - 2 && nextPosstibleColumn > 1 && takingASeat[nextPosstibleRow + 1][nextPosstibleColumn - 1] == 2 && takingASeat[nextPosstibleRow + 1][nextPosstibleColumn - 1] != 1)
                        {
                            if (takingASeat[nextPosstibleRow + 2][nextPosstibleColumn - 2] == 0)
                            {
                                for (int i = 0; i < redCheckeredStones.Count; i++)
                                {
                                    int row = Grid.GetRow(redCheckeredStones[i]);
                                    int col = Grid.GetColumn(redCheckeredStones[i]);
                                    if (row == nextPosstibleRow + 1 && col == nextPosstibleColumn - 1)
                                    {
                                        redCheckeredStones[i].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(redCheckeredStones[i]);
                                        redCheckeredStones.Remove(redCheckeredStones[i]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                                for (int i = 0; i < queenRedStones.Count; i++)
                                {
                                    int row = Grid.GetRow(queenRedStones[i]);
                                    int col = Grid.GetColumn(queenRedStones[i]);
                                    if (row == nextPosstibleRow + 1 && col == nextPosstibleColumn - 1)
                                    {
                                        redCheckeredStones[i].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(queenRedStones[i]);
                                        queenRedStones.Remove(queenRedStones[i]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                                takingASeat[nextPosstibleRow][nextPosstibleColumn] = 0;
                                nextPosstibleRow += 2;
                                nextPosstibleColumn -= 2;
                                Grid.SetColumn(ellipse, nextPosstibleColumn);
                                Grid.SetRow(ellipse, nextPosstibleRow);
                                takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                                goto link1;
                            }
                        }
                    }
                }
                link1:
                if (previousRow != Grid.GetRow(ellipse) && previousCol != Grid.GetColumn(ellipse))
                {
                    isUserTurn = true;
                    PCSteps = false;
                }
            }
        }
        private void PcQueenStepsMove3(Ellipse ellipse)
        {
            if (PCSteps == true)
            {
                int nextPosstibleRow = Grid.GetRow(ellipse);
                int nextPosstibleColumn = Grid.GetColumn(ellipse);
                int previousRow = nextPosstibleRow;
                int previousCol = nextPosstibleColumn;
                for (int k = 0; k < MyGrid.RowDefinitions.Count; k++)
                {
                    for (int j = 0; j < MyGrid.ColumnDefinitions.Count; j++)
                    {
                        if (nextPosstibleRow > 0 && nextPosstibleColumn > 0 && takingASeat[nextPosstibleRow - 1][nextPosstibleColumn - 1] != 2 && takingASeat[nextPosstibleRow - 1][nextPosstibleColumn - 1] != 1)
                        {
                            takingASeat[nextPosstibleRow][nextPosstibleColumn] = 0;
                            nextPosstibleColumn--;
                            nextPosstibleRow--;
                            Grid.SetColumn(ellipse, nextPosstibleColumn);
                            Grid.SetRow(ellipse, nextPosstibleRow);
                            takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                        }
                        else if (nextPosstibleRow > 1 && nextPosstibleColumn > 1 && takingASeat[nextPosstibleRow - 1][nextPosstibleColumn - 1] == 2 && takingASeat[nextPosstibleRow - 1][nextPosstibleColumn - 1] != 1)
                        {
                            if (takingASeat[nextPosstibleRow - 2][nextPosstibleColumn - 2] == 0)
                            {
                                for (int i = 0; i < redCheckeredStones.Count; i++)
                                {
                                    int row = Grid.GetRow(redCheckeredStones[i]);
                                    int col = Grid.GetColumn(redCheckeredStones[i]);
                                    if (row == nextPosstibleRow - 1 && col == nextPosstibleColumn - 1)
                                    {
                                        redCheckeredStones[i].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(redCheckeredStones[i]);
                                        redCheckeredStones.Remove(redCheckeredStones[i]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                                for (int i = 0; i < queenRedStones.Count; i++)
                                {
                                    int row = Grid.GetRow(queenRedStones[i]);
                                    int col = Grid.GetColumn(queenRedStones[i]);
                                    if (row == nextPosstibleRow - 1 && col == nextPosstibleColumn - 1)
                                    {
                                        redCheckeredStones[i].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(queenRedStones[i]);
                                        queenRedStones.Remove(queenRedStones[i]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                                takingASeat[nextPosstibleRow][nextPosstibleColumn] = 0;
                                nextPosstibleRow -= 2;
                                nextPosstibleColumn -= 2;
                                Grid.SetColumn(ellipse, nextPosstibleColumn);
                                Grid.SetRow(ellipse, nextPosstibleRow);
                                takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                                goto link1;
                            }
                        }
                    }
                }
                link1:
                if (previousRow != Grid.GetRow(ellipse) && previousCol != Grid.GetColumn(ellipse))
                {
                    isUserTurn = true;
                    PCSteps = false;
                }
            }
        }
        private void PcQueenStepsMove4(Ellipse ellipse)
        {
            if (PCSteps == true)
            {
                int nextPosstibleRow = Grid.GetRow(ellipse);
                int nextPosstibleColumn = Grid.GetColumn(ellipse);
                int previousRow = nextPosstibleRow;
                int previousCol = nextPosstibleColumn;
                for (int k = 0; k < MyGrid.RowDefinitions.Count; k++)
                {
                    for (int j = 0; j < MyGrid.ColumnDefinitions.Count; j++)
                    {
                        if (nextPosstibleRow < MyGrid.RowDefinitions.Count - 1 && nextPosstibleColumn < MyGrid.RowDefinitions.Count - 1 && takingASeat[nextPosstibleRow + 1][nextPosstibleColumn + 1] != 2 && takingASeat[nextPosstibleRow + 1][nextPosstibleColumn + 1] != 1)
                        {
                            takingASeat[nextPosstibleRow][nextPosstibleColumn] = 0;
                            nextPosstibleColumn++;
                            nextPosstibleRow++;
                            Grid.SetColumn(ellipse, nextPosstibleColumn);
                            Grid.SetRow(ellipse, nextPosstibleRow);
                            takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                        }
                        else if (nextPosstibleRow < MyGrid.RowDefinitions.Count - 2 && nextPosstibleColumn < MyGrid.RowDefinitions.Count - 2 && takingASeat[nextPosstibleRow + 1][nextPosstibleColumn + 1] == 2 && takingASeat[nextPosstibleRow + 1][nextPosstibleColumn + 1] != 1)
                        {
                            if (takingASeat[nextPosstibleRow + 2][nextPosstibleColumn + 2] == 0)
                            {
                                for (int i = 0; i < redCheckeredStones.Count; i++)
                                {
                                    int row = Grid.GetRow(redCheckeredStones[i]);
                                    int col = Grid.GetColumn(redCheckeredStones[i]);
                                    if (row == nextPosstibleRow + 1 && col == nextPosstibleColumn + 1)
                                    {
                                        redCheckeredStones[i].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(redCheckeredStones[i]);
                                        redCheckeredStones.Remove(redCheckeredStones[i]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                                for (int i = 0; i < queenRedStones.Count; i++)
                                {
                                    int row = Grid.GetRow(queenRedStones[i]);
                                    int col = Grid.GetColumn(queenRedStones[i]);
                                    if (row == nextPosstibleRow + 1 && col == nextPosstibleColumn + 1)
                                    {
                                        redCheckeredStones[i].Fill = Brushes.Transparent;
                                        MyGrid.Children.Remove(queenRedStones[i]);
                                        queenRedStones.Remove(queenRedStones[i]);
                                        takingASeat[row][col] = 0;
                                    }
                                }
                                takingASeat[nextPosstibleRow][nextPosstibleColumn] = 0;
                                nextPosstibleRow += 2;
                                nextPosstibleColumn += 2;
                                Grid.SetColumn(ellipse, nextPosstibleColumn);
                                Grid.SetRow(ellipse, nextPosstibleRow);
                                takingASeat[nextPosstibleRow][nextPosstibleColumn] = 1;
                                goto link1;
                            }
                        }
                    }
                }
                link1:
                if (previousRow != Grid.GetRow(ellipse) && previousCol != Grid.GetColumn(ellipse))
                {
                    isUserTurn = true;
                    PCSteps = false;
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
