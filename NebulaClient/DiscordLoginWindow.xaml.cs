using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;

namespace NebulaClient
{
    public partial class DiscordLoginWindow : Window
    {
        public string? AuthCode { get; private set; }
        private readonly string _startUrl;

        public DiscordLoginWindow(string startUrl)
        {
            InitializeComponent();
            _startUrl = startUrl;
            Loaded += DiscordLoginWindow_Loaded;
        }

        private async void DiscordLoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            webView.Source = new Uri(_startUrl);
        }

        private void CoreWebView2_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            string url = webView.Source.ToString();
            
            // Look for the code in the redirect URL
            if (url.Contains("code="))
            {
                int startIndex = url.IndexOf("code=") + 5;
                int endIndex = url.IndexOf("&", startIndex);
                AuthCode = endIndex == -1 ? url.Substring(startIndex) : url.Substring(startIndex, endIndex - startIndex);
                
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
