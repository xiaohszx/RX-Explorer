﻿using RX_Explorer.Class;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;

namespace RX_Explorer.Dialog
{
    public sealed partial class NewFileDialog : QueueContentDialog
    {
        public string NewFileName { get; private set; }

        public NewFileDialog()
        {
            InitializeComponent();

            Extension.Items.Add($"{Globalization.GetString("File_Type_TXT_Description")}(.txt)");
            Extension.Items.Add($"{Globalization.GetString("File_Type_Compress_Description")}(.zip)");
            Extension.Items.Add($"{Globalization.GetString("File_Type_RTF_Description")}(.rtf)");
            Extension.Items.Add("Microsoft Word(.docx)");
            Extension.Items.Add("Microsoft PowerPoint(.pptx)");
            Extension.Items.Add("Microsoft Excel(.xlsx)");
        }

        private void QueueContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(NewFileNameTextBox.Text))
            {
                args.Cancel = true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Extension.Text))
                {
                    InvalidInputTip.IsOpen = true;
                    args.Cancel = true;
                }
                else
                {
                    NewFileName = NewFileNameTextBox.Text + Regex.Match(Extension.SelectedItem.ToString(), @"\.\w+").Value;
                }
            }
        }

        private void Extension_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            if (sender.Items.All((Item) => Item.ToString() != args.Text))
            {
                if (args.Text.Length <= 1 || !args.Text.StartsWith(".") || args.Text.LastIndexOf(".") != 0)
                {
                    InvalidInputTip.IsOpen = true;
                    args.Handled = true;
                }
            }
        }

        private void TypeQuestion_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            TypeTip.IsOpen = true;
        }
    }
}
