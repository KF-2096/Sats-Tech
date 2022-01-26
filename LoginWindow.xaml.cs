using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;

namespace Variedades
{
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            
        }

        //Start main window
        private void LoginButton(object sender, RoutedEventArgs e)
        {
            errorTxt.Text="";
			MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query1 = " delete from sms_queue where status !='PENDING' and datediff(created_date, now()) < 0";
                MySqlCommand sqlCmd1 = new MySqlCommand(query1, conn);
                sqlCmd1.Prepare();
                sqlCmd1.ExecuteNonQuery();
                sqlCmd1.Dispose();

                String query = " select * from user where name=@name and password=@pass";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@name", UserTextBox.Text);
                sqlCmd.Parameters.AddWithValue("@pass", PassTextBox.Password);
                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                if (rdr.HasRows) 
                {
                    var main = new MainWindow();
                    main.Show();
                    this.Close();
                }else
                {
					errorTxt.Text="Invalid Credentials. Please try again!";
                }
                rdr.Dispose();
                sqlCmd.Dispose();
                
            }
			catch(Exception ex)
            {

            }
			finally
			{
				conn.Close();
			}
        }
    }
}
