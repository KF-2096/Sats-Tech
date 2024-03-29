﻿using System;
using System.Collections.Generic;
using System.Data;
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
using MySql.Data.MySqlClient;


namespace Variedades
{
    /// <summary>
    /// Lógica de interacción para UserControlClient.xaml
    /// </summary>
    public partial class UserControlClient : UserControl
    {
        public UserControlClient()
        {
            InitializeComponent();
            
            customer_grid.ItemsSource = LoadCustomerGrid();

        }
        public void Bt_search_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = DbConn.getDBConnection();
            List<Customer> customers = new List<Customer>();
            try
            {

                conn.Open();
                String query = "select  id,name, mobile, vc_number, sid from customer where name like @searchTxt or mobile like @searchTxt or vc_number like @searchTxt or sid like @searchTxt limit 10";

                MySqlCommand sqlCmd = new MySqlCommand(query, conn);

                sqlCmd.Parameters.AddWithValue("@searchTxt", "%" + customerSearch.Text + "%");
                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();


                while (rdr.Read())
                {
                    Customer customer = new Customer();
                    customer.ID = rdr.GetInt32(0);
                    customer.Name = rdr.GetString(1);
                    customer.Mobile = rdr.GetString(2);
                    customer.VC = rdr.GetString(3);
                    customer.SID = rdr.GetString(4);
                    customers.Add(customer);
                }

                rdr.Dispose();
                sqlCmd.Dispose();
                customer_grid.ItemsSource = customers;
            }
            catch (Exception err)
            {
                MessageBox.Show("Search " + err.Message);
            }
            finally 
            {
                conn.Close();
            }
           
           if (customers.Count == 0)
            {
                Bt_new_Click(sender, e);
            }
        }
        
        public void Bt_reload_Click(object sender, RoutedEventArgs e)
        {
            Customer selectedCustomer = (Customer)customer_grid.SelectedItem;
            ReloadForm rf = new ReloadForm(selectedCustomer);
            rf.ShowDialog();
        }

        public void Bt_edit_Click(object sender, RoutedEventArgs e)
        {
             
            Customer selectedCustomer = (Customer)customer_grid.SelectedItem;
            UserEdit ue = new UserEdit(selectedCustomer);
             
            ue.ShowDialog();

        }
        public void Bt_history_Click(object sender, RoutedEventArgs e)
        {
            Customer selectedCustomer = (Customer)customer_grid.SelectedItem;
            ReloadHistoryView rhv = new ReloadHistoryView(selectedCustomer);
            rhv.ShowDialog();


        }
        private void Bt_new_Click(object sender, RoutedEventArgs e)
        {
            UserEdit ue = new UserEdit();
            
            ue.ShowDialog();
        }
        
        private List<Customer> LoadCustomerGrid()
        {
            MySqlConnection conn = DbConn.getDBConnection();
            List<Customer> customers = new List<Customer>();
            try
            {
                conn.Open();
                String query = "select  id, name, mobile, vc_number, sid from customer limit 10";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
               

                while (rdr.Read())
                {
                    Customer customer = new Customer();
                    customer.ID = rdr.GetInt32(0);
                    customer.Name = rdr.GetString(1);
                    customer.Mobile = rdr.GetString(2);
                    customer.VC = rdr.GetString(3);
                    customer.SID = rdr.GetString(4);
                    customers.Add(customer);
                }
                sqlCmd.Dispose();
                rdr.Dispose();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                conn.Close();
            }
             
            return customers;
        }

       
    }


    
}
