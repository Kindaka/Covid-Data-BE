using Quartz;
using System.Text;
using System.Data;
using ODataCovid.Models;

namespace ODataCovid.Services.Quartz;
[DisallowConcurrentExecution]
public class CovidContractJob : IJob
{
    private readonly HttpClient _client;
    private readonly CovidContext _covidContext;
 
    public CovidContractJob(HttpClient client, CovidContext covidContext)
    {
        _client = client;
        _covidContext = covidContext;
    }
    public async Task Execute(IJobExecutionContext context)
    {

        string code = string.Empty;
        string mess = string.Empty;
        try
        {
            //await SyncDataCountryRegion();
            await SyncDataCovidDaily();
        }
        catch(Exception )
        {
            //log.ErrorFormat("departmentset ", "Department object sent from client is {error} ", ex.ToString());
        }
        // return Task.CompletedTask;
    }
    public async Task<(string, string)> SyncDataCountryRegion()
    {
        try
        {
            HttpClient githubusercontentClient = new HttpClient();
            var response = await githubusercontentClient.GetAsync("https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_global.csv");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result.ToString();
                var today = DateTime.Today;
                var fullpath = "././Data/time_series_covid19_deaths_global" +  today.Year.ToString() + today.Month.ToString("00") + today.Day.ToString("00")+".csv";
                if (File.Exists(fullpath))
                {
                    File.Delete(fullpath);
                }
                using (FileStream fs = File.Create(fullpath))
                {
                    // Add some text to file    
                    Byte[] data = new UTF8Encoding(true).GetBytes(result);
                    fs.Write(data, 0, data.Length);
                }
                var dataSource = ConvertCSVtoDataTable(fullpath);
                var Countrys = new List<CountryRegion>();
                foreach (DataRow row in dataSource.Rows)
                {
                    try
                    {
                        var country = new CountryRegion
                        {
                            latitude = float.Parse(row["Lat"]?.ToString() ?? "0"),
                            longitude = float.Parse(row["Long"]?.ToString() ?? "0"),
                            countryName = row["Country/Region"]?.ToString()
                        };
                        Countrys.Add(country);
                    }
                    catch 
                    {
                        continue;
                    }
                }
                await _covidContext.CountryRegions.AddRangeAsync(Countrys);
                await _covidContext.SaveChangesAsync();
            }
            
            return ("OK", "Contract object sent from client is success");
        }
        catch (Exception ex)
        {
            return ("Error", ex.ToString());
        }
    }

    public async Task<(string, string)> SyncDataCovidDaily()
    {
        try
        {
            HttpClient githubusercontentClient = new HttpClient();
            var today = DateTime.Today;

            var fromDate = new DateTime(2021, 1, 1);
            var toDate = new DateTime(2022, 2, 20);
            var url = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/";
            
            while(fromDate < toDate)
            {
                fromDate = fromDate.AddDays(1);
                var dateFormat =   fromDate.Month.ToString("00") + "-" + fromDate.Day.ToString("00") + "-" + fromDate.Year.ToString();
                var suburl = url + dateFormat +".csv";
                var response = await githubusercontentClient.GetAsync(suburl);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result.ToString();
                    //var today = DateTime.Today;
                    var fullpath = "././Data/csse_covid_19_daily_reports" + today.Year.ToString() + today.Month.ToString("00") + today.Day.ToString("00") + ".csv";
                    if (File.Exists(fullpath))
                    {
                        File.Delete(fullpath);
                    }
                    using (FileStream fs = File.Create(fullpath))
                    {
                        // Add some text to file    
                        Byte[] data = new UTF8Encoding(true).GetBytes(result);
                        fs.Write(data, 0, data.Length);
                    }
                    var dataSource = ConvertCSVtoDataTable(fullpath);
                    var CovidDailys = new List<CovidDaily>();
                    foreach (DataRow row in dataSource.Rows)
                    {
                        try
                        {
                            var strDates = row["Last_Update"].ToString().Split(" ")[0].Split("-");
                            DateTime dayValue = new DateTime(int.Parse(strDates[0]), int.Parse(strDates[1]), int.Parse(strDates[2]));
                            var countryRegion = _covidContext.CountryRegions
                                .Where(c => c.countryName == row["Country_Region"].ToString() 
                                && c.latitude == float.Parse(row["Lat"].ToString()) 
                                && c.longitude == float.Parse(row["Long_"].ToString())).FirstOrDefault();
                            var covidDailyEntity = new CovidDaily();
                            if (countryRegion is null)
                            {
                                var obj = new CovidDaily
                                {
                                    day = dayValue,
                                    personConfirmed = row["Confirmed"].ToString() != "" ? long.Parse(row["Confirmed"]?.ToString()) : 0,
                                    personDeath = row["Deaths"].ToString() != "" ? long.Parse(row["Deaths"]?.ToString()) : 0,
                                    personRecovered = row["Recovered"].ToString() != "" ? long.Parse(row["Recovered"]?.ToString()) : 0,
                                    personActive = row["Active"].ToString() != "" ? long.Parse(row["Active"]?.ToString()) : 0,
                                    CountryRegion = new CountryRegion {
                                        countryName = row["Country_Region"]?.ToString(),
                                        latitude = float.Parse(row["Lat"].ToString()),
                                        longitude = float.Parse(row["Long_"].ToString())
                                    },
                                    CountryRegionId = 1
                                };
                                covidDailyEntity = obj;
                            }
                            else
                            {
                                var obj = new CovidDaily
                                {
                                    day = dayValue,
                                    personConfirmed = row["Confirmed"].ToString() != "" ? long.Parse(row["Confirmed"]?.ToString()) : 0,
                                    personDeath = row["Deaths"].ToString() != "" ? long.Parse(row["Deaths"]?.ToString()) : 0,
                                    personRecovered = row["Recovered"].ToString() != "" ? long.Parse(row["Recovered"]?.ToString()) : 0,
                                    personActive = row["Active"].ToString() != "" ? long.Parse(row["Active"]?.ToString()) : 0,
                                    CountryRegion = countryRegion,
                                    CountryRegionId = countryRegion.id
                                };
                                covidDailyEntity = obj;
                            }
                            
                            CovidDailys.Add(covidDailyEntity);
                        }
                        catch
                        {
                            continue;
                        }

                    }
                    await _covidContext.CovidDailies.AddRangeAsync(CovidDailys);
                    await _covidContext.SaveChangesAsync();
                }
            }
            //_client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(ePn.username + ":" + ePn.password)));
            //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return ("OK", "Contract object sent from client is success");
        }
        catch (Exception ex)
        {
            return ("Error", ex.ToString());
        }
    }

    public DataTable ConvertCSVtoDataTable(string strFilePath)
    {
        DataTable dt = new DataTable();
        dt.TableName = "DATA";
        using (StreamReader sr = new StreamReader(strFilePath))
        {
            string[] headers = sr.ReadLine().Split(',');
            foreach (string header in headers)
            {
                DataColumn col = new DataColumn(header);
                dt.Columns.Add(col);
            }
            dt.AcceptChanges();
            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split(',');
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }

        }
        dt.AcceptChanges();
        return dt;
    }
}

