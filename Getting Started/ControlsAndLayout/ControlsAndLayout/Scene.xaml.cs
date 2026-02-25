// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace ControlsAndLayout
{
    public partial class Scene
    {
        public bool RealTimeUpdate = true;

        private void HandleSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender == null)
                return;

            Details.DataContext = (sender as ListBox).DataContext;
        }

        protected void HandleTextChanged(object sender, TextChangedEventArgs me)
        {
            if (RealTimeUpdate) ParseCurrentBuffer();
        }

        private void ParseCurrentBuffer()
        {
            try
            {
                var str = TextBox1.Text;
                // Guard: don't try to parse empty or whitespace-only text.
                if (string.IsNullOrWhiteSpace(str))
                {
                    // Clear preview area and reset error state.
                    cc.Children.Clear();
                    TextBox1.Foreground = Brushes.Black;
                    ErrorText.Text = "";
                    return;
                }
                var ms = new MemoryStream();
                var sw = new StreamWriter(ms);
                sw.Write(str);
                sw.Flush();
                ms.Flush();
                ms.Position = 0;
                try
                {
                    var content = XamlReader.Load(ms);
                    if (content != null)
                    {
                        cc.Children.Clear();
                        cc.Children.Add((UIElement) content);
                    }
                    TextBox1.Foreground = Brushes.Black;
                    ErrorText.Text = "";
                }

                catch (XamlParseException xpe)
                {
                    TextBox1.Foreground = Brushes.Red;
                    TextBox1.TextWrapping = TextWrapping.Wrap;
                    ErrorText.Text = xpe.Message;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        protected void OnClickParseButton(object sender, RoutedEventArgs args)
        {
            ParseCurrentBuffer();
        }

        protected void ShowPreview(object sender, RoutedEventArgs args)
        {
            PreviewRow.Height = new GridLength(1, GridUnitType.Star);
            CodeRow.Height = new GridLength(0);
        }

        protected void ShowCode(object sender, RoutedEventArgs args)
        {
            PreviewRow.Height = new GridLength(0);
            CodeRow.Height = new GridLength(1, GridUnitType.Star);
        }

        protected void ShowSplit(object sender, RoutedEventArgs args)
        {
            PreviewRow.Height = new GridLength(1, GridUnitType.Star);
            CodeRow.Height = new GridLength(1, GridUnitType.Star);
        }
        //to clear the selection when the expander is expanded
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            LayoutListBox.Focus();
            ControlListBox.Focus();
            // Clear current selection  
            try
            {
                // clear any existing selection 
                LayoutListBox.SelectedIndex = -1;
                ControlListBox.SelectedIndex = -1;
            }
            catch
            {
                // ignore exceptions in UI event (optional: log)
            }
        }
    }
}