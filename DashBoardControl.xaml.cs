﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Net.Http;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Variedades
{
    /// <summary>
    /// Interaction logic for DashBoardControl.xaml
    /// </summary>
    public partial class DashBoardControl : UserControl
    {
        public DashBoardControl()
        {
            InitializeComponent();
            IDictionary<string, string> dashBoardData = LoadDashBoardData();

            updateCells(dashBoardData);
            GetSMSAccountStatus();
        }

        private void updateCells(IDictionary<string, string> dashBoardData)
        {
            try { d2hTotal.Text = dashBoardData["VideoCon"]; } catch (Exception ex) { d2hTotal.Text = "0.00"; }
            try { dishTotal.Text = dashBoardData["DishTv"]; } catch (Exception ex) { dishTotal.Text = "0.00"; }
            try { sunTotal.Text = dashBoardData["SunDirect"]; } catch (Exception ex) { sunTotal.Text = "0.00"; }
            try { airtelTotal.Text = dashBoardData["Airtel"]; } catch (Exception ex) { airtelTotal.Text = "0.00"; }
            try { tataTotal.Text = dashBoardData["TataSky"]; } catch (Exception ex) { tataTotal.Text = "0.00"; }
            try { tvlTotal.Text = dashBoardData["TvLanka"]; } catch (Exception ex) { tvlTotal.Text = "0.00"; }
            try { serviceTotal.Text = dashBoardData["serviceCharge"]; } catch (Exception ex) { serviceTotal.Text = "0.00"; }



        }
        private async void GetSMSAccountStatus()
        {
            Console.WriteLine("Start SMS Statuc Method call ");
            string URL = "https://app.notify.lk/api/v1/status";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            var smsUserId = ConfigurationManager.AppSettings["SmsAccount"].Split(new char[] { ';' })[0];
            var smsKey = ConfigurationManager.AppSettings["SmsAccount"].Split(new char[] { ';' })[1];
            var urlParameters = (FormattableString)$"?user_id={smsUserId}&api_key={smsKey}";
            var response = client.GetAsync(urlParameters.ToString());
            if (response.Result.IsSuccessStatusCode)
            {
                var prods = await response.Result.Content.ReadAsStringAsync();
                //dynamic data = J  Array.Parse(prods);
                //dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(prods, new ExpandoObjectConverter());
                Console.WriteLine(prods);
                dynamic data = JsonConvert.DeserializeObject (prods );
                smsTotal.Text = String.Format("{0:N}", data.data.acc_balance);
                 
            } else
            {
                smsTotal.Text = "0.00";
            }        
        }

        private IDictionary<string, string> LoadDashBoardData()
        {
            MySqlConnection conn = DbConn.getDBConnection();
            IDictionary<string, string> dashdata = new Dictionary<string, string>();
            try
            {
                conn.Open();
                String query = " select provider, FORMAT(sum(pack_amount),2),FORMAT(sum(addOn_amount),2) from reload " +
                    " where tx_date between @stDate and @endDate group by provider ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);

                DateTime endDate = DateTime.Today;
                DateTime stDate = new DateTime(endDate.Year, endDate.Month, 1);
                sqlCmd.Parameters.AddWithValue("@stDate", stDate);
                sqlCmd.Parameters.AddWithValue("@endDate", endDate);

                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();

                while (rdr.Read())
                {
                    dashdata.Add(rdr.GetString(0), rdr.GetString(1));

                }
                sqlCmd.Dispose();

                query = "select  FORMAT(sum(extracharge_amount),2) " +
                    " where tx_date between @stDate and @endDate  ";
                sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@stDate", stDate);
                sqlCmd.Parameters.AddWithValue("@endDate", endDate);
                while (rdr.Read())
                {
                    dashdata.Add("serviceCharge", rdr.GetString(1));

                }
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
            return dashdata;
        }
        
    }
}