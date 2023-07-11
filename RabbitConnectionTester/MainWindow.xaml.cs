using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RabbitConnectionTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtConnectionString.Text = "host=localhost;username=guest;password=guest;virtualHost=/";
            ChangeConnectionString(txtConnectionString.Text);
        }

        #region User Events

        private void txtConnectionString_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionString(txtConnectionString.Text);
        }

        private void txtHost_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionStringPart(txtConnectionString.Text, "host", txtHost.Text);
        }

        private void txtVirtualHost_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionStringPart(txtConnectionString.Text, "virtualHost", txtVirtualHost.Text);
        }

        private void txtUserName_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionStringPart(txtConnectionString.Text, "username", txtUserName.Text);
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeConnectionStringPart(txtConnectionString.Text, "password", txtPassword.Text);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            btnTest.IsEnabled = false;

            try
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.UserName = txtUserName.Text;
                factory.Password = txtPassword.Text;
                factory.VirtualHost = txtVirtualHost.Text;
                factory.HostName = txtHost.Text;

                IConnection conn = factory.CreateConnection();
                conn.Dispose();

                MessageBox.Show("Connection successfully established to " + txtHost.Text, "Test successful", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to connect", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnTest.IsEnabled = true;
            }
        }

        private void lblConnectionString_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtConnectionString.Text);
        }

        private void lblHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtHost.Text);
        }

        private void lblVirtualHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtVirtualHost.Text);
        }

        private void lblUserName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtUserName.Text);
        }

        private void lblPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtPassword.Text);
        }

        #endregion User Events

        #region Methods

        private void ChangeConnectionString(string newConnectionString)
        {
            DisplayParts(newConnectionString);
        }

        private void ChangeConnectionStringPart(string connectionString, string partName, string partValue)
        {
            List<string> newParts = new List<string>();

            var parts = connectionString.Split(';');
            bool updated = false;

            foreach (var part in parts.Select(s => s.Trim()).Where(s => s.Length > 0 && s.IndexOf('=') > 0))
            {
                var partBits = part.Split('=');

                if (partBits[0].Equals(partName, StringComparison.CurrentCultureIgnoreCase)
                     ||
                     (partName.Equals("User", StringComparison.CurrentCultureIgnoreCase) &&
                      partBits[0].Equals("User ID", StringComparison.CurrentCultureIgnoreCase)))
                {
                    newParts.Add(String.Format("{0}={1}", partBits[0], partValue));
                    updated = true;
                }
                else
                {
                    newParts.Add(String.Format("{0}={1}", partBits[0], partBits[1]));
                }
            }

            if (updated == false)
            {
                newParts.Add(String.Format("{0}={1}", partName, partValue));
            }

            txtConnectionString.Text = String.Join("; ", newParts);
        }

        private void DisplayParts(string newConnectionString)
        {
            var parts = newConnectionString.Split(';');

            txtHost.Text = "";
            txtVirtualHost.Text = "";
            txtUserName.Text = "";
            txtPassword.Text = "";

            foreach (var part in parts)
            {
                var partBits = part.Split('=');

                switch (partBits[0].Trim().ToLower())
                {
                    case "host":
                        txtHost.Text = partBits[1];
                        break;
                    case "virtualhost":
                        txtVirtualHost.Text = partBits[1];
                        break;
                    case "username":
                        txtUserName.Text = partBits[1];
                        break;
                    case "password":
                        txtPassword.Text = partBits[1];
                        break;
                }
            }
        }

        #endregion Methods
    }
}
