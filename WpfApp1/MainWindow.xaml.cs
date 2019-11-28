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
        private string _fileName = null;

        private string _dataFormat = null;

        private bool _flagChange = false;

        public RichTextEditorSample()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
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
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                _dataFormat = dlg.FilterIndex == 2 ? DataFormats.Text : DataFormats.Rtf;
                _fileName = dlg.FileName;
                range.Load(fileStream, _dataFormat);
                fileStream.Close();
            }
        }

        private void Close_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            if (_flagChange)
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
            _fileName = null;
            
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FileStream fileStream = null;
            TextRange range =  new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            if (_fileName != null)
            {
                fileStream = new FileStream(_fileName, FileMode.Create);
                range.Save(fileStream, _dataFormat);
                fileStream.Close();
                _flagChange = false;
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt";
            if (dlg.ShowDialog() == true)
            {
                fileStream = new FileStream(dlg.FileName, FileMode.Create);
                _dataFormat = dlg.FilterIndex == 2 ? DataFormats.Text : DataFormats.Rtf;
                range.Save(fileStream, _dataFormat);
            }
            if (fileStream != null)
            {
                fileStream.Close();
            }
            _flagChange = false;
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
            _flagChange = true;
        }
    }
}