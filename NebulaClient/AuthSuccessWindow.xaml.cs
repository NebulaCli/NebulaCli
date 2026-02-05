using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace NebulaClient
{
    public partial class AuthSuccessWindow : Window
    {
        public bool ShouldRemember { get; private set; }
        private int _secondsLeft = 3;

        public AuthSuccessWindow()
        {
            InitializeComponent();
            StartCooldown();
        }

        private async void StartCooldown()
        {
            while (_secondsLeft > 0)
            {
                CooldownText.Text = $"Syncing Session ({_secondsLeft}s)";
                await Task.Delay(1000);
                _secondsLeft--;
            }

            // Unlock the button
            CooldownText.Visibility = Visibility.Collapsed;
            ConfirmButton.IsEnabled = true;
            
            var sb = (Storyboard)this.Resources["ButtonUnlock"];
            sb.Begin();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            ShouldRemember = RememberMeCheck.IsChecked ?? false;
            this.DialogResult = true;
            this.Close();
        }
    }
}
