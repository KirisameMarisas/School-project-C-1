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

namespace WpfTutorialSamples.Rich_text_controls
{
    public partial class RichTextEditorSample : Window
    {
        private string fileName = null;

        private string dataFormat = null;

        private bool flagChange = false;

        public RichTextEditorSample()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            var names = typeof(Brushes).GetProperties().Select(p => p.Name).ToArray();
            cmbColour.ItemsSource = names;
            rtbEditor.AddHandler(RichTextBox.DropEvent, new DragEventHandler(rtbEditor_Drop), true);
            rtbEditor.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(rtbEditor_DragOver), true);
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
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

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt";
            if (dlg.ShowDialog() == true)
            {
                dataFormat = dlg.FilterIndex == 2 ? DataFormats.Text : DataFormats.Rtf;
                Open_File(dlg.FileName);
            }
        }

        private void Open_File(string fileName)
        {
            try
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Open);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                this.fileName = fileName;
                range.Load(fileStream, dataFormat);
                fileStream.Close();
            }catch(Exception e)
            {
                MessageBox.Show("File could not be opened. Make sure the file is a supported file.");
                return;
            }
        }
        private void Close_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            if (flagChange)
            {
                MessageBoxResult messageBox = MessageBox.Show("This file has changed, Save?", "Save tip", MessageBoxButton.YesNoCancel);

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
            rtbEditor.Document.Blocks.Clear();
            fileName = null;
            
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FileStream fileStream = null;
            TextRange range =  new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            if (fileName != null)
            {
                fileStream = new FileStream(fileName, FileMode.Create);
                range.Save(fileStream, dataFormat);
                fileStream.Close();
                flagChange = false;
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt";
            if (dlg.ShowDialog() == true)
            {
                fileStream = new FileStream(dlg.FileName, FileMode.Create);
                dataFormat = dlg.FilterIndex == 2 ? DataFormats.Text : DataFormats.Rtf;
                range.Save(fileStream, dataFormat);
            }
            if (fileStream != null)
            {
                fileStream.Close();
            }
            flagChange = false;
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
        }


        private void rtbEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            flagChange = true;
        }

        private void cmbColour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string temp = cmbColour.SelectedItem.ToString();
            var values = typeof(Brushes).GetProperties().Select(p => new { Name = p.Name, Brush = p.GetValue(null) as Brush }).ToArray();
            if (cmbColour.SelectedItem != null)
            {
                Brush brush = null;
                
                foreach (var element in values)
                {
                    if (element.Name == temp)
                    {
                        brush = element.Brush;
                        break;
                    }
                }
                rtbEditor.Selection.ApplyPropertyValue(Inline.ForegroundProperty, brush);
            }
        }

        private void rtbEditor_Drop(object sender, DragEventArgs e)
        {
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
    }
}