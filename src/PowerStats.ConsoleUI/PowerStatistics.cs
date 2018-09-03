using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PowerStats.ConsoleUI
{
    public class PowerStatistics
    {
        private static HttpClient client = new HttpClient();

        // update port number if required
        private const string API_HOST = "http://localhost:62059";
        private const string API_PATH = "api/powerstatistics";

        public async Task RunAsync()
        {
            // configure http client
            client.BaseAddress = new Uri(API_HOST);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var powerStatistics = await GetPowerStatisticsAsync(API_PATH);

                PrintStatistics(powerStatistics);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        private async Task<IList<PowerStatisticsModel>> GetPowerStatisticsAsync(string path)
        {
            IList<PowerStatisticsModel> powerStatistics = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    powerStatistics = JsonConvert.DeserializeObject<IList<PowerStatisticsModel>>(responseString);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return await Task.FromResult(powerStatistics);
        }

        private void PrintStatistics(IList<PowerStatisticsModel> statisticsList)
        {
            if (statisticsList.Count <= 0)
                return;

            // print statistics header
            Console.WriteLine($"{{File Name}} {{Date Time}} {{Value}} {{Median Value}}");

            // print statistics data
            foreach (var stats in statisticsList)
            {
                Console.WriteLine(
                    "{" + stats.FileName + "} " +
                    "{" + stats.ConsumptionDate.ToString("dd-MM-yyyy HH:mm") + "} " +
                    "{" + stats.Value.ToString("0.######") + "} " +
                    "{" + stats.MedianValue.ToString("0.######") + "}");
            }
        }
    }
}
