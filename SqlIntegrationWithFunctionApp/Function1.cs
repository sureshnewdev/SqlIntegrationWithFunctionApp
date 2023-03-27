using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace SqlIntegrationWithFunctionApp
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string message = "Error while inserting data";

            int empId = int.Parse( req.Query["empid"].ToString());
            string empName = req.Query["empname"];
            string address = req.Query["address"];


            // Get the connection string from app settings and use it to create a connection.
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            //using (SqlConnection conn = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=empdb;Integrated Security=True"))
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"insert into employee values(  {empId}, '{empName}', '{address}' )";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {

                    // Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteNonQueryAsync();
                    log.LogInformation($"{rows} rows were updated");
                    message = "Employee information added successfully";
                }
            }

            return new OkObjectResult(message);
        }
    }
}
