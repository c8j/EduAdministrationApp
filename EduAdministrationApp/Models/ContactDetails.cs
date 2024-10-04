namespace EduAdministrationApp.Models;

public record ContactDetails
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PersonalNumber { get; init; }
    public required string AddressLine { get; init; }
    public required string PostalCode { get; init; }
    public required string City { get; init; }

}
