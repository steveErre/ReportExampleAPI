using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using ShpAzfService.Models;
using ShpAzfService.MyModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShpAzfService.Functions
{
    public class GetListItems
    {
        private readonly ILogger<GetListItems> _logger;

        public GetListItems(ILogger<GetListItems> logger)
        {
            _logger = logger;
        }

        [Function("GetListItems")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Lettura del corpo della richiesta (per il POST)
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Assuming these are passed from the frontend
            RequestBody? request = null;
            FilterItems? filter = null;
            int pageNumber = 1;  // Page number passed from React
            int pageSize = 10;   // Page size passed from React
            string? sortField = "Date";  // Field to sort by
            bool isDescending = true;  // Sorting direction (true = descending, false = ascending)

            if (!string.IsNullOrEmpty(requestBody))
            {
                request = JsonConvert.DeserializeObject<RequestBody>(requestBody);
                filter = request?.Filter;
                pageNumber = request.PageNumber;  // Page number passed from React
                pageSize = request.PageSize;   // Page size passed from React
                sortField = request.SortField;  // Field to sort by
                isDescending = request.IsSortedDescending;  // Sorting direction (true = descending, false = ascending)
            }

            //recupero context sharepoint
            var context = await Classes.MSALAuthAppOnlyCertificateSHP.sampleInvocationAppOnly("Report", _logger);

            string siteUrl = "//request.Filter.SiteUrl;
            string listName =// request.Filter.ListName;
            int totalItems = 0;

            List<MyListItems> filteredItems = new List<MyListItems>();

            List list = context.Web.Lists.GetByTitle(listName);
            CamlQuery query = new CamlQuery();

            string filterCondition = string.IsNullOrEmpty(request.Filter.SearchInput)
                ? ""
                : $"<Contains><FieldRef Name='Title'/><Value Type='Text'>{request.Filter.SearchInput}</Value></Contains>";

            query.ViewXml = $@"<View>
                                <Query>
                                    <Where>
                                        {filterCondition}
                                    </Where>
                                    <OrderBy>
                                        <FieldRef Name='{(string.IsNullOrEmpty(request.SortField) ? "Date" : request.SortField)}' Ascending='{(!request.IsSortedDescending).ToString().ToUpper()}' />
                                    </OrderBy>
                                </Query>
                                <RowLimit>{request.PageSize}</RowLimit>
                                <ViewFields>
                                    <FieldRef Name='ID'/>
                                    <FieldRef Name='Title'/>
                                    <FieldRef Name='Value'/>
                                    <FieldRef Name='Date'/>
                                </ViewFields>
                              </View>";

            ListItemCollection items = list.GetItems(query);
            context.Load(items);
            await context.ExecuteQueryAsync();

            totalItems = items.Count;

            filteredItems = items.Select(i => new MyListItems
            {
                Id = i.Id,
                Titolo = i["Title"].ToString(),
                Valore = Convert.ToDouble(i["Valore"]),
                Data = Convert.ToDateTime(i["Date"])
            }).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();


            return new OkObjectResult(new ResultItems
        {
            Items = filteredItems,
            TotalItems = totalItems
    });
        }
    }
}
