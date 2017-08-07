namespace Sitecore.Support.MediaFramework.Analytics.Dashboard
{
  #region Namespaces

  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Data;
  using System.Globalization;
  using System.Text;
  using System.Web;
  using System.Web.Http;
  using System.Web.Http.Results;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Serialization;
  using Sitecore.Analytics.Reporting;
  using Data;
  using ExperienceAnalytics.Api;
  using ExperienceAnalytics.Api.Encoding;
  using Sitecore.MediaFramework.Analytics.Dashboard;
  using Services.Infrastructure.Web.Http;

  #endregion

  public class MediaFrameworkReportsController : ServicesApiController
  {
    [HttpGet]
    public IHttpActionResult Get(string datasource, string siteName)
    {
      var reportingDataProvider = ApiContainer.Configuration.GetReportingDataProvider();
      var cachingPolicy = new CachingPolicy();
      var str = Database.GetDatabase("core").GetItem(new ID(datasource)).Fields["{0AA8B742-BBDF-4405-AB8D-6FAC7E79433B}"].Value;

      NameValueCollection values = HttpUtility.ParseQueryString(base.Request.RequestUri.Query);
      var time = DateTime.ParseExact(values["dateFrom"], "dd-MM-yyyy", new DateTimeFormatInfo());
      var time2 = DateTime.ParseExact(values["dateTo"], "dd-MM-yyyy", new DateTimeFormatInfo());
      var str2 = time.ToString("yyyy-MM-dd");
      var str3 = time2.ToString("yyyy-MM-dd");

      if (time.Equals(time2) && (values["dateTo"].Length <= 10))
      {
        str2 = time.ToString("yyyy-MM-dd 00:00:00");
        str3 = time2.ToString("yyyy-MM-dd 23:59:59");
      }

      str = str.Replace("@StartDate", "'" + str2 + "'").Replace("@EndDate", "'" + str3 + "'");
      var newValue = "0";

      if (siteName != "all")
      {
        newValue = new Hash32Encoder().Encode(siteName);
        str = str.Replace("@SiteNameIdOperator", "=");
      }
      else
      {
        str = str.Replace("@SiteNameIdOperator", "!=");
      }

      var query = new ReportDataQuery(str.Replace("@SiteNameId", newValue));
      DataTableReader reader = reportingDataProvider.GetData("reporting", query, cachingPolicy).GetDataTable().CreateDataReader();
      var data = new ReportData();
      var num = 0;

      while (reader.Read())
      {
        var row = new Dictionary<string, string>();

        for (var i = 0; i < reader.FieldCount; i++)
        {
          row.Add(reader.GetName(i), reader[i].ToString());
        }

        data.AddRow(row);
        num++;
      }
      var content = new ReportResponse
      {
        data = data,
        TotalRecordCount = num
      };
      return new JsonResult<ReportResponse>(content, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
    }
  }
}
