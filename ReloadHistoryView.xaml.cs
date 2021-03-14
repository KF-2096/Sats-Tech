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
    /// Interaction logic for ReloadHistoryView.xaml
    /// </summary>
    public partial class ReloadHistoryView : Window
    {
        private Customer selectedCustomer;
        public ReloadHistoryView(Customer customer)
        {
            InitializeComponent();
            selectedCustomer = customer;
            reloadHistory_grid.ItemsSource = LoadReloadHistory();
            txtBlk_customer.Text = "Customer : " + selectedCustomer.VC;
        }

        private List<Reload> LoadReloadHistory()
        {
            MySqlConnection conn = DbConn.getDBConnection();
            List<Reload> reloads = new List<Reload>();
            try
            {
                conn.Open();
                String query = " select  reload.id, customer.vc_number, tx_date, expiry_date, provider, total, pack_desc, coalesce(pack_amount, 0),  " +
                      " addOn_desc, coalesce(addOn_amount, 0),     " +
                     " extracharge_desc, coalesce(extracharge_amount, 0) " +
                    " from reload join customer on reload.customer_id = customer.id  where customer.id=@id"+
                    " order by tx_date  desc ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@id", selectedCustomer.ID);
                sqlCmd.Prepare();

                MySqlDataReader rdr = sqlCmd.ExecuteReader();

                while (rdr.Read())
                {
                    Reload reload = new Reload();
                    reload.ID = rdr.GetInt32(0);
                    reload.customer = rdr.GetString(1);
                    reload.txDate = rdr.GetDateTime(2);
                    reload.expDate = rdr.IsDBNull(3) ? null : rdr.GetString(3);
                    reload.provider = rdr.GetString(4);
                    reload.total = rdr.GetDecimal(5);
                    reload.packDesc = rdr.GetString(6);
                    reload.packAmount = rdr.GetDecimal(7);
                    reload.addOnDesc = rdr.GetString(8);
                    reload.addOnAmount = rdr.GetDecimal(9);
                    reload.extrachargeDesc = rdr.GetString(10);
                    reload.extrachargeAmount = rdr.GetDecimal(11);
                    reloads.Add(reload);
                }
                sqlCmd.Dispose();
                rdr.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                conn.Close();
            }
            return reloads;
        }
    }
}
