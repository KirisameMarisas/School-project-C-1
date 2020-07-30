using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Reflection;

namespace WpfTutorialSamples.Rich_text_controls
{
    public partial class RichTextEditorSample : Window
    {
        private string fileName = null;

        private string dataFormat = null;

        private bool flagChange = false;

        private RichTextBox rtbEditor = null;

        private int items = 1;

        private List<TextRange> SearchResults;

        private int NextReplaceIndex = 0;

        public RichTextEditorSample()
        {
            InitializeComponent(); //Initialize the original window from .xaml file.
            rtbEditor = rtbEditor1; // Set the current tab Editor
            
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source); // Initialize the Font Families
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 }; // Initialize the Font Size

            cmbColour.ItemsSource = typeof(Colors).GetProperties(); //Initiallze the Colour tab

            rtbEditor.AddHandler(RichTextBox.DropEvent, new DragEventHandler(rtbEditor_Drop), true); //Bind the Drop function
            rtbEditor.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(rtbEditor_DragOver), true); // Ibid.
        }

        private void NewFile(string fileName)
        {
            string temp = "rtbEtitor" + (items++).ToString(); // Get New File Tab Name

            fileName = Path.GetFileName(fileName);

            RichTextBox newRtb = new RichTextBox
            {
                Name = temp
            };

            
            TabItem newItem = new TabItem
            {
                Header = fileName,
                Name = temp,
                Content = newRtb
            };

            rtbEditor = newRtb; // Set the current tab Editor
            rtbEditor.AddHandler(RichTextBox.DropEvent, new DragEventHandler(rtbEditor_Drop), true);
            rtbEditor.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(rtbEditor_DragOver), true);
            tabControl.Items.Add(newItem); // TabController add this tab
            newItem.IsSelected = true; // Select this tab
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            this.ClearSearchResult();
            return;
            // It has something worng in this function , so it will be deleted
            //rtbEditor.Selection.Select(rtbEditor.Selection.Start, rtbEditor.Selection.Start);
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();

        }

        //Open the file trigger
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt";
            if (dlg.ShowDialog() == true)
            {
                dataFormat = dlg.FilterIndex == 2 ? DataFormats.Text : DataFormats.Rtf;
                NewFile(dlg.FileName);
                Open_File(dlg.FileName);
            }
        }

        //Open the file function 
        private void Open_File(string fileName)
        {
            try
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Open);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                this.fileName = fileName;
                range.Load(fileStream, dataFormat);
                fileStream.Close();
                (tabControl.SelectedItem as TabItem).Header = Path.GetFileName(fileName);
            }catch(Exception e)
            {
                MessageBox.Show("File could not be opened. Make sure the file is a supported file.");
                return;
            }
        }

        //Close File trigger
        private void Close_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            if (flagChange)
            {
                MessageBoxResult messageBox = MessageBox.Show("This file has changed, Save?", "Save tip", MessageBoxButton.YesNoCancel); //Show the 'Save tip' Message

                switch (messageBox)
                {
                    case MessageBoxResult.Yes:
                    {
                        Save_Executed(null, null);
                        break;
                    }
                    case MessageBoxResult.No:
                    {
                        break;
                    }
                    case MessageBoxResult.Cancel:
                    {
                        return;
                    }
                }
            }
            rtbEditor.Document.Blocks.Clear(); //Clear the Editor Context
            fileName = null;

            // Modify the TabController 
            int length = tabControl.Items.Count;
            if (length == 1)
            {
                (tabControl.SelectedItem as TabItem).Header = "File1";
            }else
            {
                int itemIndex = tabControl.SelectedIndex;
                if (itemIndex != 1)
                {
                    tabControl.SelectedIndex = itemIndex - 1;
                }
                tabControl.Items.RemoveAt(itemIndex);
                fileName = (tabControl.SelectedItem as TabItem).Header as string;
            }
            rtbEditor = ((tabControl.SelectedItem as TabItem).Content) as RichTextBox;
            flagChange = false;
        }

        //Save trigger
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FileStream fileStream = null;
            TextRange range =  new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            // If the file if created by program
            if (fileName != null)
            {
                fileStream = new FileStream(fileName, FileMode.Create);
                range.Save(fileStream, dataFormat);
                fileStream.Close();
                flagChange = false;
                (tabControl.SelectedItem as TabItem).Header = Path.GetFileName(fileName);
                return;
            }
            // If the file opened
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt";
            if (dlg.ShowDialog() == true)
            {
                fileStream = new FileStream(dlg.FileName, FileMode.Create);
                (tabControl.SelectedItem as TabItem).Header = Path.GetFileName(dlg.FileName);
                dataFormat = dlg.FilterIndex == 2 ? DataFormats.Text : DataFormats.Rtf;
                range.Save(fileStream, dataFormat);
                fileName = dlg.FileName;
            }
            if (fileStream != null)
            {
                fileStream.Close();
            }
            flagChange = false;
        }

        //Change the Font Family trigger
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbFontFamily.SelectedItem != null)
                    rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
            }catch(Exception)
            {
                return;
            }
            
        }

        //Change the Font Size trigger
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
            }catch(Exception)
            {
                return;
            }
        }


        // Flag the Editor Change
        private void rtbEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            flagChange = true;
        }

        // Change Colour trigger
        private void cmbColour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbColour.SelectedItem == null)
            {
                return;
            }
            Color tempColor = ((Color)(cmbColour.SelectedItem as PropertyInfo).GetValue(null, null));
            SolidColorBrush solidColorBrush = new SolidColorBrush(tempColor);
            
            rtbEditor.Selection.ApplyPropertyValue(Inline.ForegroundProperty, solidColorBrush);
        }


        // Drop file to the Editor field function
        private void rtbEditor_Drop(object sender, DragEventArgs e)
        {
            this.ClearSearchResult();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] docPath = (string[])e.Data.GetData(DataFormats.FileDrop);

                string typeofFile = Path.GetExtension(docPath[0]);

                if (typeofFile == ".rtf")
                {
                    dataFormat = DataFormats.Rtf;

                }
                else
                {
                    if (typeofFile == ".txt")
                    {
                        dataFormat = DataFormats.Text;
                    }
                    else
                    {
                        //error 
                        dataFormat = null;
                        //MessageBox.Show("File could not be opened. Make sure the file is a supported file.");
                    }
                
                }
                Open_File(docPath[0]);
                
            }
        }

        // Drop over function
        private void rtbEditor_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = false;
        }

        //Change the Editor after changing the tab
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ClearSearchResult();
            if (tabControl.SelectedItem != null)
            {
                rtbEditor = ((tabControl.SelectedItem) as TabItem).Content as RichTextBox;
            }
        }

        //Reset the Editor
        private void ClearSearchResult()
        {
            if (this.SearchResults != null)
            {
                foreach (TextRange range in this.SearchResults)
                {
                    range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
                }
            }
            this.SearchResults = null;
            this.NextReplaceIndex = 0;
        }
        // Search  function
        private List<TextRange> Search(TextPointer position, string word)
        {
            List<TextRange> matchingText = new List<TextRange>();
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    //带有内容的文本
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    //查找关键字在这文本中的位置
                    int indexInRun = textRun.IndexOf(word);
                    int indexHistory = 0;
                    while (indexInRun >= 0)
                    {
                        TextPointer start = position.GetPositionAtOffset(indexInRun + indexHistory);
                        TextPointer end = start.GetPositionAtOffset(word.Length);
                        matchingText.Add(new TextRange(start, end));

                        indexHistory = indexHistory + indexInRun + word.Length;
                        textRun = textRun.Substring(indexInRun + word.Length);//去掉已经采集过的内容
                        indexInRun = textRun.IndexOf(word);//重新判断新的字符串是否还有关键字
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }
            return matchingText;
        }

        //Replace Next function
        private void ReplaceNext(string keyword)
        {
            if (this.SearchResults == null || this.NextReplaceIndex >= this.SearchResults.Count)
            {
                this.ClearSearchResult();
                return;
            }
            TextRange range = this.SearchResults[this.NextReplaceIndex];
            range.Text = keyword;
            range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
            this.NextReplaceIndex += 1;
        }

        // Replace All function
        private void ReplaceAll(string keyword)
        {
            while (this.SearchResults != null)
            {
                this.ReplaceNext(keyword);
            }
        }

        // Initiallze ToolBar
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolbar = sender as ToolBar;

            var overflowGrid = toolbar.Template.FindName("OverflowGrid", toolbar) as FrameworkElement;
            var mainPanelBorder = toolbar.Template.FindName("MainPanelBorder", toolbar) as FrameworkElement;

            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        // Click Buttons trigger
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.SearchTextBox.Text))
            {
                return;
            }
            this.ClearSearchResult();
            this.SearchResults = this.Search(this.rtbEditor.Document.ContentStart, this.SearchTextBox.Text);
            foreach (TextRange range in this.SearchResults)
            {
                range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.DarkCyan);
            }
        }

        private void ReplaceNextButton_Click(object sender, RoutedEventArgs e)
        {
            this.ReplaceNext(this.ReplaceTextBox.Text);
        }

        private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            this.ReplaceAll(this.ReplaceTextBox.Text);
        }

        //Windows closing trigger
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Close_Executed(null, null);
        }
    }
}