using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FeedbackV1
{
    public static class Extesions
    {
        public static void AddPagination(this HttpResponse response,
            int currentPage, int itemsPerPage, int totalItems, int totalPages)
            {
                var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
                var camelCaseFormatter = new JsonSerializerSettings();
                camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
                response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
                response.Headers.Add("Acces-Control-Expose-Headers", "Pagination");

            }
    }
}