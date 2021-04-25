using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Variedades
{
    /// <summary>
    /// Interaction logic for SmsControll.xaml
    /// </summary>
    public partial class SmsControll : UserControl
    {
        List<String> selectedProviders = new List<string>();
        public SmsControll()
        {
            InitializeComponent();
        }

        public void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            string content = (sender as CheckBox).Content.ToString();
            selectedProviders.Add(content);
            mobile_number.Text = "";
            mobile_number.IsEnabled = false;
        }
        public void checkBox_Unchecked (object sender, RoutedEventArgs e) 
        {
            string content = (sender as CheckBox).Content.ToString();
            selectedProviders.Remove(content);
            mobile_number.Text = "";
            mobile_number.IsEnabled = true;
        }
        public void bt_save_Click(object sender, RoutedEventArgs e)
        {
             
            if ((selectedProviders.Count<1) && (string.IsNullOrEmpty(mobile_number.Text)))
            {
                MessageBox.Show("Please select Provider or Enter mobile number !");
                return;
            }


            TextRange txt = new TextRange(smsMsg.Document.ContentStart, smsMsg.Document.ContentEnd);

            if (txt.Text==String.Empty)
            {
                MessageBox.Show("Please add message !");
                return;
            }

            if (!string.IsNullOrEmpty(mobile_number.Text))
            {
                SaveSingle(mobile_number.Text, txt.Text);
                bt_reset_Click(sender, e);
            }
            else if(selectedProviders.Count >= 1) {
                SaveData(txt.Text, string.Join(",", selectedProviders));
                bt_reset_Click(sender, e);
            }

            
        }
        public void bt_reset_Click(object sender, RoutedEventArgs e)
        {
            VideoCon.IsChecked = false;
            TataSky.IsChecked = false;
            Airtel.IsChecked = false;
            DishTv.IsChecked = false;
            TvLanka.IsChecked = false;
            SunDirect.IsChecked = false;
            smsMsg.Document.Blocks.Clear();
            mobile_number.Clear();
        }

        private void SaveData(string msg, string providers)
        {
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = " insert into sms_queue (mobile_number,message,status) select mobile,@msg as message, 'PENDING' " +
"from customer where id in(select customer_id from reload where  FIND_IN_SET(provider, @providers))  ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
               
                sqlCmd.Parameters.AddWithValue("@msg", msg);
                sqlCmd.Parameters.AddWithValue("@providers", providers);

                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("SMS sent !");
                sqlCmd.Dispose();
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }
        }

        private void SaveSingle(String mobile,String msg)
        {
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = " insert into sms_queue (mobile_number,message,status) values(@mobile,@msg,'PENDING') ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);

                sqlCmd.Parameters.AddWithValue("@msg", msg);
                sqlCmd.Parameters.AddWithValue("@mobile", mobile);

                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("SMS sent !");
                sqlCmd.Dispose();
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
