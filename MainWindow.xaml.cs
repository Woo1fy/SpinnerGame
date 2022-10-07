using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SpinnersGame.GameLogic;

namespace SpinnersGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			_GameLogic.InitState(GameDifficulty.Easy);
			DrawBoard();
		}

		readonly GameLogic.GameLogic _GameLogic = new GameLogic.GameLogic();

		private void PlayerClicksSpace(Object sender, RoutedEventArgs e) {
			var space = (Button)sender;
			if (String.IsNullOrWhiteSpace(space.Content?.ToString())) {
				return;
			}

			SequenceActionsClass sequence = _GameLogic.ParseToIndex(space.Name);

			_GameLogic.SelectField(sequence.x, sequence.y);
			DrawBoard();
		}

		private void btnRestartGame_Click(object sender, RoutedEventArgs e) {
			_GameLogic.InitState(_GameLogic.GetLastDifficulty());
			DrawBoard();
		}
		
		private void DrawBoard() {
			// Соре за костыль, я пытался найти контрол по названию, но у меня не вышло.
			// 
			if (generalGrid.Children.Count > 2) {
				generalGrid.Children.RemoveAt(2);
			}

			cbSelectDifficulty.Text = _GameLogic.GetLastDifficulty().ToString();

			generalGrid.Children.Add(grid(_GameLogic.GetMatrixSize()));
		}

		private void cbSelectDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ComboBoxItem typeItem = (ComboBoxItem)cbSelectDifficulty.SelectedItem;
			TextBlock tbText = (TextBlock)typeItem.Content;

			_GameLogic.InitState(Enum.Parse<GameDifficulty>(tbText.Text));
			DrawBoard();
		}

		#region Controls
		Button button(bool value, string name, int col, int row) {
			Button btn = new Button();
			btn.Name = name;

			btn.SetValue(Grid.ColumnProperty, col);
			btn.SetValue(Grid.RowProperty, row);

			btn.Style = (Style)Resources["fieldsGrid"];

			Image sprite = new Image();
			sprite.Source = new BitmapImage(new Uri("Resources/spinner.png", UriKind.Relative));
			sprite.RenderTransformOrigin = Point.Parse("0.5,0.5");

			btn.Background = new SolidColorBrush(_GameLogic.GetFieldColor(value));

			if (value == SpinnerMode.on) {
				RotateTransform rotateTransform = new RotateTransform(90);
				sprite.RenderTransform = rotateTransform;
			}
			else if (value == SpinnerMode.off) {
				RotateTransform rotateTransform = new RotateTransform(0);
				sprite.RenderTransform = rotateTransform;
			}
			
			btn.Content = sprite;

			return btn;
		}

		Grid grid(int size) {

			Grid gd = new Grid();
			gd.Name = "gridBoard";

			gd.SetValue(Grid.RowProperty, 1);

			gd.MaxWidth = 500;
			gd.MaxHeight = 500;

			gd.Background = new SolidColorBrush(_GameLogic.GetBackroundColor());

			for (int i = 0; i < size; i++) {

				RowDefinition rowDefinition = new RowDefinition();
				rowDefinition.Height = new GridLength(1.0, GridUnitType.Star);
				gd.RowDefinitions.Add(rowDefinition);

				ColumnDefinition colDefinition = new ColumnDefinition();
				colDefinition.Width = new GridLength(1.0, GridUnitType.Star);

				gd.ColumnDefinitions.Add(new ColumnDefinition());
			}

			for (int i = 0; i < _GameLogic.GetMatrixSize(); i++) {
				for (int j = 0; j < _GameLogic.GetMatrixSize(); j++) {
					gd.Children.Add(button(_GameLogic.GetMatrixData(i, j), $"fieldBtn{i}{j}", i, j));
				}
			}

			return gd;
		}
		#endregion
	}
}
