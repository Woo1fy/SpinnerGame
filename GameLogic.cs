using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace SpinnersGame {

	public static class SpinnerMode {
		public const bool on = true;
		public const bool off = false;
	}

	public class SequenceActionsClass {
		public int x;
		public int y;
	}

	public enum GameDifficulty { Easy, Medium, Hard }

	public class GameLogic {

		private List<List<bool>> matrix = new List<List<bool>>();

		private int countMatrix = default;

		private bool lastMove = default;

		private GameDifficulty lastDifficulty = GameDifficulty.Hard;

		public SequenceActionsClass ParseToIndex(string controlName) {
			SequenceActionsClass sequence = new SequenceActionsClass();

			sequence.x = int.Parse(controlName.Where(x => Char.IsDigit(x)).ToArray()) / 10;
			sequence.y = int.Parse(controlName.Where(x => Char.IsDigit(x)).ToArray()) % 10;
			return sequence;
		}

		public void InitState(GameDifficulty difficulty) {
			SetDifficulty(difficulty);
		}

		void SetDifficulty(GameDifficulty difficulty) {
			lastDifficulty = difficulty;

			switch (difficulty) {
				case GameDifficulty.Easy:
					countMatrix = 3;
					SetEmptyFields();
					RandomizeMatrix(4);
					break;
				case GameDifficulty.Medium:
					countMatrix = 4;
					SetEmptyFields();
					RandomizeMatrix(5);
					break;
				case GameDifficulty.Hard:
					countMatrix = 5;
					SetEmptyFields();
					RandomizeMatrix(6);
					break;
				default: MessageBox.Show("Уровень сложности отсутствует");
					break;
			}
		}

		void RandomizeMatrix(int shuffleCount) {
			Random random = new Random();

			for (var i = 0; i < shuffleCount; i++) {
				var x = random.Next(countMatrix);
				var y = random.Next(countMatrix);
				SelectField(x, y, true);
			}
		}

		public void SelectField(int x, int y, bool randomize = false) {

			var newValue = lastMove == SpinnerMode.on ? SpinnerMode.off : SpinnerMode.on;

			lastMove = newValue;

			for (var i = 0; i < countMatrix; i++) {
				for (var j = 0; j < countMatrix; j++) {
					if (i == x || j == y) {
						if (matrix[i][j] == newValue) {
							matrix[i][j] = newValue == SpinnerMode.on ? SpinnerMode.off : SpinnerMode.on;
							continue;
						}
						matrix[i][j] = newValue;
					}
				}
			}

			if (!randomize) {
				if (isEnd()) {
					MessageBox.Show("You won");
				}
			} else if (randomize) {
				if (isEnd()) {
					SelectField(x, y);
				}
			}

		}

		bool isEnd() {
			Debug.WriteLine(lastMove);
			if (matrix.All(s => s.All(f => f == SpinnerMode.on)) 
				|| matrix.All(s => s.All(f => f == SpinnerMode.off))) { 
				return true;
			}
			return false;
		}

		void SetEmptyFields() => matrix = new bool[countMatrix][].Select(x =>
					new bool[countMatrix].Select(t => SpinnerMode.off)
					.ToList()).ToList();

	

		#region Get Methods
		public bool GetMatrixData(int x, int y) {
			return matrix[x][y];
		}

		public int GetMatrixSize() {
			return countMatrix;
		}

		public GameDifficulty GetLastDifficulty() {
			return lastDifficulty;
		}

		public Color GetBackroundColor() {
			var thisMove = lastMove == SpinnerMode.on ? SpinnerMode.off : SpinnerMode.on;

			return GetFieldColor(thisMove);
		}

		public Color GetFieldColor(bool value) {
			switch (value) {
				case SpinnerMode.off:
					return Colors.Blue;
				case SpinnerMode.on:
					return Colors.Red;
			}
		}
		#endregion

	}
}
