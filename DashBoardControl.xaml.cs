using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Net.Http;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Windows;

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
            try { serviceTotal.Text = String.Format("{0:N}", dashBoardData["dailyTotal"]);  } catch (Exception ex) { serviceTotal.Text = "0.00"; }
            try { customerTotal.Text = dashBoardData["totalCustomer"]; } catch (Exception ex) { customerTotal.Text = "0.00"; }
            


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
            try
            {
                var response = client.GetAsync(urlParameters.ToString());
                if (response.Result.IsSuccessStatusCode)
                {
                    var prods = await response.Result.Content.ReadAsStringAsync();
                    //dynamic data = J  Array.Parse(prods);
                    //dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(prods, new ExpandoObjectConverter());
                    Console.WriteLine(prods);
                    dynamic data = JsonConvert.DeserializeObject(prods);
                    smsTotal.Text = String.Format("{0:N}", data.data.acc_balance);

                }
                else
                {
                    smsTotal.Text = "0.00";
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to SMS Gateway, Please check your internet !");
                smsTotal.Text = "0.00";
            }

        }

        private IDictionary<string, string> LoadDashBoardData()
        {
            MySqlConnection conn = DbConn.getDBConnection();
            IDictionary<string, string> dashdata = new Dictionary<string, string>();
            float dailyTotal = 0f;

            try
            {
                conn.Open();
                String query = " select provider, FORMAT(sum(pack_amount),2),FORMAT(sum(addOn_amount),2) from reload " +
                    " where tx_date between @stDate and @endDate group by provider ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);

                DateTime stDate = DateTime.Today;
                DateTime endDate = stDate.Date.AddDays(1).AddTicks(-1);

                sqlCmd.Parameters.AddWithValue("@stDate", stDate);
                sqlCmd.Parameters.AddWithValue("@endDate", endDate);

                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();

                while (rdr.Read())
                {
                    
                    dashdata.Add(rdr.GetString(0), rdr.GetString(1));
                    dailyTotal += float.Parse(rdr.GetString(1));
                }

                dashdata.Add("dailyTotal", dailyTotal.ToString());
                sqlCmd.Dispose();
                rdr.Dispose();
               

                query = "select  count(id) from customer " ;
                sqlCmd = new MySqlCommand(query, conn);
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    dashdata.Add("totalCustomer", rdr.GetString(0));

                }
                rdr.Dispose();
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
