
using System.Text.Json.Serialization;

public class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }
    public List<PhoneNumber> PhoneNumbers { get; set; } = [];
}

public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}

public class PhoneNumber
{
    public string? Number { get; set; }
    public PhoneNumberType Type { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<PhoneNumberType>))]
public enum PhoneNumberType
{
    Mobile,
    Home,
    Work,
    Other
}