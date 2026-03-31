using System.Windows;

namespace PredaNaplo
{
    public partial class JelszoValtoztatasDialog : Window
    {
        public string NewPassword => NewPasswordBox.Password;
        public bool IsConfirmed { get; private set; }

        public JelszoValtoztatasDialog()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("A jelszó nem lehet üres!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            IsConfirmed = true;
            DialogResult = true;
            Close();
        }
    }
}