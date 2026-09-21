namespace Web.Companies.Get;

public class GetCompanyRequest
{
    public const string Route = "/settings/companies/{CompanyId:int}";
    public static string BuildRoute(int companyId) => Route.Replace("{CompanyId:int}", companyId.ToString());

    public int CompanyId { get; set; }
}
