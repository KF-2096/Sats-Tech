using MySql.Data.MySqlClient;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Variedades;

namespace Variedades
{
    class ReminderJob : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                
                String query = " insert into sms_queue(mobile_number, message, status) " +
" select mobile, CONCAT('Dear customer,\nYour ', provider, ' subscription is going to expire.\nPlease pay your bill on or before ', " +
" DATE_FORMAT(DATE_ADD(now(), INTERVAL 4 DAY), '%Y/%m/%d'), ' . Call for recharge 0112339920 / 0768866972.\nif you have paid ignore this messege.')  as message , 'PENDING' as status " +
" from reload join customer on reload.customer_id = customer.id " +
" where datediff(expiry_date, now()) = 4 and (select last_run from bg_task_last_run where id =1 )< DATE_FORMAT( now() , '%Y/%m/%d') ";

                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Prepare();
                int affectedRows = sqlCmd.ExecuteNonQuery();

                if (affectedRows > 0) { 
                    // Update last run table
                    query = "update bg_task_last_run set last_run= now() where id =1";
                    sqlCmd = new MySqlCommand(query, conn);
                    sqlCmd.Prepare();
                    sqlCmd.ExecuteNonQuery();
                }
                //Purge SMS_QUEUE
                //query = "delete from sms_queue where status !='PENDING'";
                //sqlCmd = new MySqlCommand(query, conn);
                //sqlCmd.Prepare();
                //sqlCmd.ExecuteNonQuery();


                sqlCmd.Dispose();
            }
            catch(Exception err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }

            await Console.Out.WriteLineAsync("Greetings from ReminderJob Job!");
        }
    }
}
