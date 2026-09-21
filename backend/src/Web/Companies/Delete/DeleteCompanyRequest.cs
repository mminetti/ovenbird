namespace Web.Companies.Delete;

public class DeleteCompanyRequest
{
    public const string Route = "/settings/companies/{CompanyId:int}";
    public static string BuildRoute(int companyId) => Route.Replace("{CompanyId:int}", companyId.ToString());

    public int CompanyId { get; set; }
}
