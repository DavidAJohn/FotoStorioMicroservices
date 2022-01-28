namespace Store.BlazorWasm.Models;

public class Address
{
    public Address()
    {
    }

    public Address(string firstName, string lastName, string street, string secondLine, string city, string county, string postCode)
    {
        FirstName = firstName;
        LastName = lastName;
        Street = street;
        SecondLine = secondLine;
        City = city;
        County = county;
        PostCode = postCode;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string SecondLine { get; set; }
    public string City { get; set; }
    public string County { get; set; }
    public string PostCode { get; set; }
}
