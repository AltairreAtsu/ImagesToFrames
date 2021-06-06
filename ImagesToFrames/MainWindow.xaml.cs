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
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace ImagesToFrames
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SettingsManager settingsManager;
		private AppLogger AppLogger;
		private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
		private static bool IsTextAllowed(string text)
		{
			return !_regex.IsMatch(text);
		}

		private List<FrameNameControl> frameNameControls;

		public MainWindow()
		{
			InitializeComponent();

			LoadConfig();
			StartLogging();
			frameNameControls = new List<FrameNameControl>();
		}

		private void LoadConfig()
		{
			settingsManager = new SettingsManager();
			bool success = settingsManager.LoadConfiguration();
			if (!success) System.Windows.MessageBox.Show("Error loading or creating config flie.");
			OutputPathField.Text = settingsManager.settings.defaultExportPath;
		}
		private void StartLogging()
		{
			AppLogger = new AppLogger();
			try
			{
				AppLogger.BeginFileLogging();
			}
			catch
			{
				System.Windows.MessageBox.Show("Error starting logging. Unable to create filestream.");
			}
		}

		private int[] TryParseDimensionsArray()
		{
			try
			{
				int[] dimensions = new int[] { int.Parse(DimensionsFieldX.Text), int.Parse(DimensionsFieldY.Text) };
				return dimensions;
			}
			catch
			{
				System.Windows.MessageBox.Show("Integar Parsing error! Please ensure only numerical values are entered into dimensions fields.");
				return null;
			}
		}
		private int[] TryParseSizeArray()
		{
			try
			{
				int[] size = new int[] { int.Parse(SizeFieldX.Text), int.Parse(SizeFieldY.Text) };
				return size;
			}
			catch
			{
				System.Windows.MessageBox.Show("Integar Parsing error! Please ensure only numerical values are entered into size fields.");
				return null;
			}
		}
		private bool ArrayContainsZero(int[] testArray)
		{
			for(int i = 0; i < testArray.Length; i++)
			{
				if (testArray[i] == 0)
				{
					System.Windows.MessageBox.Show("Error parsing size and dimensions fields. Fields cannot contain a value of zero.");
					return true;
				}
			}
			return false;
		}

		private void SaveFrame_Click(object sender, RoutedEventArgs e)
		{
			StandardFrame frame = new StandardFrame();

			int[] dimensions = TryParseDimensionsArray();
			int[] size = TryParseSizeArray();
			if (dimensions == null || size == null) return;
			if (ArrayContainsZero(dimensions) || ArrayContainsZero(size)) return;
			int xDimension = dimensions[0];
			int yDimension = dimensions[1];

			frame.frameGrid.dimensions = dimensions;
			frame.frameGrid.size = size;
			if ((bool)UseCustomFrameNames_Check.IsChecked)
			{
				bool parsingSuccess = ParseFrameNames(frame, dimensions);
				if (!parsingSuccess)
				{
					System.Windows.MessageBox.Show("Error Parsing frame names. Ensure no frame names are left blank!");
					return;
				}
			}
			else
			{
				GenerateDefaultFrameNames(frame, xDimension, yDimension);
			}

			string imageName = System.IO.Path.GetFileName(ImagePathTextField.Text);
			string path = OutputPathField.Text + "\\" + imageName;
			path = System.IO.Path.ChangeExtension(path, "frames");
			bool success = FrameGenerator.GenerateFramesFile(frame, path);
			if (!success)
			{
				System.Windows.MessageBox.Show("Error: Failed to generate frames file. Check for Invalid file paths.");
			}
		}
		private void LoadFrame_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Frames files (*.frames)|*.frames|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				StandardFrame frame = FrameGenerator.LoadFrameFromFile(openFileDialog.FileName);
				if(frame == null)
				{
					System.Windows.MessageBox.Show("Error Loading Frame file from path. Please ensure correct path is used and program has file access.");
					return;
				}
				FillInterfraceFromFrame(frame, openFileDialog.FileName);
			}
		}
		private void FillInterfraceFromFrame(StandardFrame frame, string path)
		{
			ImagePathTextField.Text = path;
			string directoryPath = System.IO.Path.GetDirectoryName(path);
			OutputPathField.Text = directoryPath;

			try
			{
				SizeFieldX.Text = frame.frameGrid.size[0].ToString();
				SizeFieldY.Text = frame.frameGrid.size[1].ToString();
			}
			catch
			{
				System.Windows.MessageBox.Show("Error Loading Size Field. Canceling File Load.");
				return;
			}

			try
			{
				DimensionsFieldX.Text = frame.frameGrid.dimensions[0].ToString();
				DimensionsFieldY.Text = frame.frameGrid.dimensions[1].ToString();
			}
			catch
			{
				System.Windows.MessageBox.Show("Error Loading Dimensions Field. Canceling File Load.");
				return;
			}
			try
			{
				UseCustomFrameNames_Check.IsChecked = true;

				string[,] frameNames = frame.frameGrid.names;
				int numberOfFrames = frameNames.Length;
				int numberOfRows = frameNames.GetLength(0);
				int numberOfColumns = frameNames.GetLength(1);
				int column = 0;
				int row = 0;


				for(int i = 0; i < numberOfFrames; i++)
				{
					frameNameControls[i].FrameNameField.Text = frameNames[row, column];
					if(frame.aliases != null)
					{
						foreach (KeyValuePair<string, string> keyValuePair in frame.aliases)
						{
							if( keyValuePair.Value.Equals(frameNames[row, column]))
							{
								frameNameControls[i].AliasField.Text = keyValuePair.Key;
							}
						}
					}
					column++;
					if(column == numberOfColumns)
					{
						column = 0;
						row++;
					}
				}
			}
			catch(Exception e)
			{
				System.Windows.MessageBox.Show("Error loading frame names or aliases.");
				System.Console.Write(e.Message);
				return;
			}
		}

		private static void GenerateDefaultFrameNames(StandardFrame frame, int xDimension, int yDimension)
		{
			string[,] names;
			if (xDimension == 1 && yDimension == 1)
			{
				names = new string[,] { { "default" } };

			}
			else
			{
				names = new string[yDimension, xDimension];
				int i = 0;
				for (int row = 0; row < yDimension; row++)
				{
					for (int column = 0; column < xDimension; column++)
					{
						names[row, column] = "default." + i;
						i++;
					}
				}

				frame.aliases = new Dictionary<string, string>();
				frame.aliases.Add("default.default", "default.0");

			}
			frame.frameGrid.names = names;
		}

		private void LocateFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ImagePathTextField.Text = openFileDialog.FileName;
			}
		}

		private void FillFromImage_Click(object sender, RoutedEventArgs e)
		{
			int[] imageSize = FrameGenerator.LoadDimensionsFromImage(ImagePathTextField.Text);
			if(imageSize == null)
			{
				System.Windows.MessageBox.Show("Could Not Find Image on path provided!");
				return;
			}
			SizeFieldX.Text = imageSize[0].ToString();
			SizeFieldY.Text = imageSize[1].ToString();
		}

		private void DivideByFrames_Click(object sender, RoutedEventArgs e)
		{
			int[] imageSize = FrameGenerator.LoadDimensionsFromImage(ImagePathTextField.Text);
			if (imageSize == null)
			{
				System.Windows.MessageBox.Show("Could Not Find Image on path provided!");
				return;
			}
			int[] dimensions = TryParseDimensionsArray();
			if (dimensions == null || ArrayContainsZero(dimensions)) return;
			SizeFieldX.Text = (imageSize[0] / dimensions[0]).ToString();
			SizeFieldY.Text = (imageSize[1] / dimensions[1]).ToString();
		}

		private void SetOutputPath_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			System.Windows.Forms.DialogResult dr = folderBrowserDialog.ShowDialog();
			if(dr == System.Windows.Forms.DialogResult.OK)
			{
				OutputPathField.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text);
		}
		private void Pasting_Event(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(String)))
			{
				String text = (String)e.DataObject.GetData(typeof(String));
				if (!IsTextAllowed(text))
				{
					e.CancelCommand();
				}
			}
			else
			{
				e.CancelCommand();
			}
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			OpenFramesScroller();
		}
		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			CloseFramesScroller();
		}
		
		private void OpenFramesScroller()
		{
			FramesScroller.Visibility = Visibility.Visible;
			this.Height += FramesScroller.Height;
			PopulateStack();
		}
		private void CloseFramesScroller()
		{
			FramesScroller.Visibility = Visibility.Collapsed;
			this.Height -= FramesScroller.Height;
			FramesStack.Children.Clear();
			frameNameControls.Clear();
			FramesStack.Height = 30;
		}

		private void PopulateStack()
		{
			int[] dimensions = TryParseDimensionsArray();
			if(dimensions == null)
			{
				CloseFramesScroller();
				return;
			}
			int numberOfFrames = dimensions[0] * dimensions[1];
			for(int i = 0; i < numberOfFrames; i++)
			{
				AddFrameNameControl(i);
			}
		}
		private void ReEvaluateStack()
		{
			int[] dimensions = TryParseDimensionsArray();
			if (dimensions == null)
			{
				CloseFramesScroller();
				return;
			}
			int numberOfFrames = dimensions[0] * dimensions[1];

			if(numberOfFrames > frameNameControls.Count)
			{
				for (int i = frameNameControls.Count; i < numberOfFrames; i++)
				{
					AddFrameNameControl(i);
				}
			}
			if(numberOfFrames < frameNameControls.Count)
			{
				int countToRemove = frameNameControls.Count - numberOfFrames;
				int startingIndex = frameNameControls.Count - countToRemove;

				frameNameControls.RemoveRange(startingIndex, countToRemove);
				FramesStack.Children.RemoveRange(startingIndex, countToRemove);
			}
		}
		private void AddFrameNameControl(int index)
		{
			FrameNameControl control = new FrameNameControl();
			control.FrameNameLabel.Content = index + ":";
			FramesStack.Children.Add(control);
			FramesStack.Height += control.Height;
			frameNameControls.Add(control);
		}

		private bool ParseFrameNames(StandardFrame frame, int[] dimensions)
		{
			string[] frameNames = new string[frameNameControls.Count];
			Dictionary<string, string> aliases = new Dictionary<string, string>();

			for (int i = 0; i < frameNameControls.Count; i++)
			{
				string alias = frameNameControls[i].AliasField.Text;
				string frameName = frameNameControls[i].FrameNameField.Text;
				if (string.IsNullOrWhiteSpace(frameName)) return false;

				frameNames[i] = frameName;
				
				if (!string.IsNullOrWhiteSpace(alias))
				{
					aliases.Add(alias, frameName);
				}
			}
			frame.frameGrid.names = ConvertFlatArrayToTwoDimensionalArray(frameNames, dimensions[1], dimensions[0]);
			if (aliases.Count > 0) frame.aliases = aliases;
			return true;
		}
		private string[,] ConvertFlatArrayToTwoDimensionalArray(string[] flatArray, int rows, int columns)
		{
			if (flatArray.Length != rows * columns) return null;
			string[,] twoDimensionaArray = new string[rows, columns];
			int column = 0;
			int row = 0;
			for(int i = 0; i < flatArray.Length; i++)
			{
				twoDimensionaArray[row, column] = flatArray[i];
				column++;
				if(column == columns)
				{
					column = 0;
					row++;
				}
			}
			return twoDimensionaArray;
		}

		private void DimensionsChangedEvent(object sender, TextChangedEventArgs e)
		{
			System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
			if (string.IsNullOrWhiteSpace(tb.Text)) return;
			if(UseCustomFrameNames_Check != null && (bool)UseCustomFrameNames_Check.IsChecked)
				ReEvaluateStack();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			AppLogger.EndLogging();
		}
	}
}
