﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Input;


namespace Variedades
{
    /// <summary>
    /// Interaction logic for ReloadForm.xaml
    /// </summary>
    public partial class ReloadForm : Window
    {
        public Customer selectedCustomer = null;
        bool editMode = false;
        private Reload selectedReload = null;
        public ReloadForm(int reload)
        {
            InitializeComponent();
            var ProviderList = new List<String>(ConfigurationManager.AppSettings["ProviderList"].Split(new char[] { ';' }));

            CMB_ProviderList.ItemsSource = ProviderList;
            editMode = true;
            GetReloadDetail(reload);

        }
        
        public ReloadForm (String VcNumber)
        {
            InitializeComponent();
            var ProviderList = new List<String>(ConfigurationManager.AppSettings["ProviderList"].Split(new char[] { ';' }));

            CMB_ProviderList.ItemsSource = ProviderList;

            selectedCustomer = GetCustomerFromVC(VcNumber);
            CustomerName.Text = selectedCustomer.VC;
            CustomerName.IsEnabled = false;
            GetLastProvider(selectedCustomer.ID);

        }

        public ReloadForm(Customer customer)
        {
            
            InitializeComponent();
            var ProviderList = new List<String>(ConfigurationManager.AppSettings["ProviderList"].Split(new char[] { ';' }));

            CMB_ProviderList.ItemsSource = ProviderList; 
            selectedCustomer = customer;
            CustomerName.Text = customer.VC;
            CustomerName.IsEnabled = false;
            GetLastProvider(selectedCustomer.ID);
        }
       
        private void bt_save_Click(object sender, RoutedEventArgs e)
        {
            if ( editMode)
            {
                if (Expiry_Date.SelectedDate == null)
                {
                    MessageBox.Show("Please selecte expiry date !");
                    Expiry_Date.Focus();
                    return;
                }
                updateExpiryDate();
                return ;
            }
            bool save = true;
            if (selectedCustomer == null)
            {
                MessageBox.Show("Please select customer ! ");
                save = false;
            }
            if ((string.IsNullOrEmpty(CMB_ProviderList.Text)) ||   (CMB_ProviderList.SelectedIndex == -1))
            {
                MessageBox.Show("Please select Provider ! ");
                save = false;
            }
            
            Decimal packageAmount;

            if (!Decimal.TryParse(PackagePrice.Text, out packageAmount))
            {
                MessageBox.Show("Package Amount Invalid ! ");
                save = false;
            }

            if ((packageAmount > 0) && (string.IsNullOrEmpty(packageDesc.Text)))
            {
                MessageBox.Show("Package Description Invalid ! ");
                save = false;
            }

            Decimal addOnAmount;

            if (!Decimal.TryParse(AddOnPrice.Text, out addOnAmount))
            {
                MessageBox.Show("Add-On Amount Invalid ! ");
                save = false;
            }

            if ((addOnAmount > 0) && (string.IsNullOrEmpty(AddOnDesc.Text)))
            {
                MessageBox.Show("Add-On Description Invalid ! ");
                save = false;
            }

            Decimal extraCharge;

            if (!Decimal.TryParse(ExtraChargeAmount.Text, out extraCharge))
            {
                MessageBox.Show("ExtraCharge Amount Invalid ! ");
                save = false;
            }
            if ((extraCharge > 0) && (string.IsNullOrEmpty(ExtraChargeDesc.Text)))
            {
                MessageBox.Show("Add-On Description Invalid ! ");
                save = false;
            }
            if ((packageAmount+ addOnAmount+ extraCharge)<1)
            {
                MessageBox.Show("Invalid Total ! ");
                save = false;
            }
            if (save)
            {
                SaveReload(selectedCustomer.ID, CMB_ProviderList.Text, packageAmount, packageDesc.Text.ToString(), addOnAmount, 
                    AddOnDesc.Text.ToString(), extraCharge, ExtraChargeDesc.Text.ToString(), Expiry_Date.SelectedDate,selectedCustomer.Mobile);
            }
        }

        private void updateExpiryDate()
        {
            MySqlConnection conn = DbConn.getDBConnection();
            MySqlTransaction tr = null;
            DateTime expDate;
            expDate = Expiry_Date.SelectedDate.Value;
            try
            {
                conn.Open();
                tr = conn.BeginTransaction();
                String query = "update reload set expiry_date=@expDate where id=@id";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@expDate", expDate.Date.ToString("yyyy-MM-dd HH:mm"));
                sqlCmd.Parameters.AddWithValue("@id", selectedReload.ID);
                sqlCmd.Prepare();
                
                sqlCmd.ExecuteNonQuery();
                tr.Commit();
                MessageBox.Show(" Updated Successfully ! ");
                sqlCmd.Dispose();
                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        private void SaveReload(int customerId, string provider, decimal packageAmount, string packageDesc, decimal addOnAmount, string addonDesc,
            decimal extraCharge, string extraChargeDesc, DateTime? selectedDate,string mobile)
        {
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {

                conn.Open();
                String query = "INSERT INTO reload (customer_id,tx_date,expiry_date,provider,total, pack_desc, pack_amount, addOn_desc, addOn_amount, extracharge_desc, extracharge_amount) " +
                    "values(@customer,@txDate,@expiry,@provider,@total,@packageDesc,@packageAmount,@addOnDesc,@addOnAmount,@extraChargeDesc,@extraChargeAmount)";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@customer", customerId);
                sqlCmd.Parameters.AddWithValue("@txDate", DateTime.Now);
                sqlCmd.Parameters.AddWithValue("@expiry", selectedDate);
                sqlCmd.Parameters.AddWithValue("@provider", provider);
                sqlCmd.Parameters.AddWithValue("@total", packageAmount + addOnAmount + extraCharge);
                sqlCmd.Parameters.AddWithValue("@packageDesc", packageDesc); 
                sqlCmd.Parameters.AddWithValue("@packageAmount", packageAmount);
                sqlCmd.Parameters.AddWithValue("@addOnDesc", addonDesc);
                sqlCmd.Parameters.AddWithValue("@addOnAmount", addOnAmount);
                sqlCmd.Parameters.AddWithValue("@extraChargeDesc", extraChargeDesc);
                sqlCmd.Parameters.AddWithValue("@extraChargeAmount", extraCharge);
                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();

                query = "insert into sms_queue (mobile_number,message,status) values(@mobile,@message,@status)";
                sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@mobile", mobile);
                sqlCmd.Parameters.AddWithValue("@message", "Dear customer\nThank you for recharging through our shop.\nPlease keep the setup box  switched on\nHelp Line : 0768866972 / 0112339920 ");
                sqlCmd.Parameters.AddWithValue("@status", "PENDING");
                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
               
                //SendSMS();
                query = "select max(id) from reload";
                sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                int invNumber = 0;
                while (rdr.Read())
                {
                    invNumber = rdr.GetInt32(0);
                }

                Bill bill = new Bill();
                bill.BillNumber = invNumber;
                bill.AddOnAmt = (float) addOnAmount ;
                bill.AddOnDesc = addonDesc;
                bill.BillAmount = (float) (packageAmount + addOnAmount + extraCharge);
                bill.BillDate = DateTime.Now;
                bill.CustomerMobile = selectedCustomer.Mobile;
                bill.CustomerSID = selectedCustomer.SID;
                bill.CustomerType = provider;
                bill.ExtraChargeAmt = (float) extraCharge;
                bill.ExtraChargeDesc = extraChargeDesc;
                bill.PackageAmt = (float) packageAmount;
                bill.PackageDesc = packageDesc;
                try
                {
                    RdlcPrint rdlcPrint = new RdlcPrint();
                    rdlcPrint.Run(bill);
                    if (MessageBox.Show("Do you want to print another copy?",
"Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        rdlcPrint.Run(bill);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to Print " + ex.ToString());
                }
                MessageBox.Show(" Saved Successfully ! ");
                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                conn.Close();
                
            }
        }

        private void bt_reset_Click(object sender, RoutedEventArgs e)
        {
            PackagePrice.Text = "0.00";
            packageDesc.Text = "";
            AddOnDesc.Text = "";
            AddOnPrice.Text = "0.00";
            ExtraChargeDesc.Text = "";
            ExtraChargeAmount.Text = "0.00";
            Expiry_Date.SelectedDate = null;
        }
        private void GetReloadDetail(int reloadId)
        {
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = " select  reload.id, customer.vc_number, tx_date, expiry_date, provider, total, pack_desc, coalesce(pack_amount, 0),  " +
                       " addOn_desc, coalesce(addOn_amount, 0),     " +
                      " extracharge_desc, coalesce(extracharge_amount, 0),customer.id, name, mobile, sid, vc_number " +
                     " from reload join customer on reload.customer_id = customer.id where reload.id = @id "  ;
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@id", reloadId);
                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                Reload reload = new Reload();
                Customer customer = new Customer();
                while (rdr.Read())
                {
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
                    customer.ID = rdr.GetInt32(12);
                    customer.Name = rdr.GetString(13);
                    customer.SID = rdr.GetString(14);
                    customer.VC = rdr.GetString(15);

                }
                sqlCmd.Dispose();
                rdr.Dispose();

                selectedReload = reload;

                selectedCustomer = customer;

                CMB_ProviderList.SelectedItem = reload.provider;
                PackagePrice.Text = reload.packAmount.ToString(); 
                packageDesc.Text = reload.packDesc;
                AddOnPrice.Text = reload.addOnAmount.ToString();
                AddOnDesc.Text = reload.addOnDesc;
                ExtraChargeAmount.Text = reload.extrachargeAmount.ToString();
                ExtraChargeDesc.Text = reload.extrachargeDesc;
                Expiry_Date.Text = reload.expDate;
                UpdateTotal();


                CustomerName.Text = customer.VC;
                CustomerName.IsEnabled = false;
                CMB_ProviderList.IsEnabled = false;
                PackagePrice.IsEnabled = false;
                packageDesc.IsEnabled = false;
                AddOnPrice.IsEnabled = false;
                AddOnDesc.IsEnabled = false;
                ExtraChargeAmount.IsEnabled = false;
                ExtraChargeDesc.IsEnabled = false;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private Customer GetCustomerFromVC(string vc)
        {
            Customer customer = new Customer();
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = " select id, name, mobile, sid, vc_number from customer where vc_number=@vc ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@vc", vc);
                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                
                 
                while (rdr.Read())
                {
                    customer.ID = rdr.GetInt32(0);
                    customer.Name = rdr.GetString(1);
                    customer.Mobile = rdr.GetString(2);
                    customer.SID = rdr.GetString(3);
                    customer.VC = rdr.GetString(4);
                }
                sqlCmd.Dispose();
                return customer;

            }catch(Exception err)
            {
                MessageBox.Show("GetCustomerFromVC("+vc+") "+ err.Message);
            }
            finally
            {
                conn.Close();
            }
            return null;
        }
        private void GetLastProvider(int sid)
        {
            string lastProvider = "";
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = "select  provider from reload where customer_id=@sid order by tx_date desc limit 1";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@sid", sid);
                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    lastProvider = rdr.GetString(0);
                }
                sqlCmd.Dispose();
                rdr.Dispose();
            }
            catch (Exception err)
            {
                MessageBox.Show("GetLastProvider(" + sid + ") " + err.Message);
            }
            finally
            {
                conn.Close();
                CMB_ProviderList.SelectedItem = lastProvider;
            }
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
           
        }
        private void UpdateTotal()
        {
            float tot = 0;
            tot += float.Parse(PackagePrice.Text ?? "0.00");
            tot += float.Parse(AddOnPrice.Text ?? "0.00");
            tot += float.Parse(ExtraChargeAmount.Text ?? "0.00");
            TotalTxt.Text = "Total : " + string.Format("{0:#,0.00}", tot);
        }

        private void PackagePrice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(PackagePrice.Text) )
            {
                PackagePrice.Text = "0.00";
            }
            UpdateTotal();
        }

        private void AddOnPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(AddOnPrice.Text))
            {
                AddOnPrice.Text = "0.00";
            }
            UpdateTotal();
        }

        private void ExtraChargeAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(ExtraChargeAmount.Text))
            {
                ExtraChargeAmount.Text = "0.00";
            }
            UpdateTotal();
        }

        private void PackagePrice_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PackagePrice.Text.Equals("0.00"))
            {
                PackagePrice.Text = "";
            }
        }

        private void AddOnPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            if (AddOnPrice.Text.Equals("0.00"))
            {
                AddOnPrice.Text = "";
            }
        }

        private void ExtraChargeAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ExtraChargeAmount.Text.Equals("0.00"))
            {
                ExtraChargeAmount.Text = "";
            }
        }
    }
}
