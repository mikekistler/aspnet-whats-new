using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true
};

{
// The following example demonstrates:
// 1. The "add", "replace", and "remove" operations.
// 2. Operations on nested properties.
// 3. Adding a new item to an array.
// 4. Using a JSON String Enum Converter in a JSON patch document.

// Original object
var person = new Person {
    FirstName = "John",
    LastName = "Doe",
    Email = "johndoe@gmail.com",
    PhoneNumbers = [new() {Number = "123-456-7890", Type = PhoneNumberType.Mobile}],
    Address = new Address
    {
        Street = "123 Main St",
        City = "Anytown",
        State = "TX"
    }
};

// Raw JSON patch document
string jsonPatch = """
[
    { "op": "replace", "path": "/FirstName", "value": "Jane" },
    { "op": "remove", "path": "/Email"},
    { "op": "add", "path": "/Address/ZipCode", "value": "90210" },
    { "op": "add", "path": "/PhoneNumbers/-", "value": { "Number": "987-654-3210", "Type": "Work" } }
]
""";

// Deserialize the JSON patch document
var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<Person>>(jsonPatch);

// Apply the JSON patch document
patchDoc!.ApplyTo(person);

// Output updated object
Console.WriteLine(JsonSerializer.Serialize(person, serializerOptions));
}

// ---------------------------------------------------------------------------------- //

{
// The following example demonstrates:
// 1. Handling errors when an operation fails.
// 2. The "test" operation.
// 3. The object passed to the ApplyTo method is modified in place. It is the caller's
//    responsiblity to discard these changes if any operation fails.

// Original object
var person = new Person {
    FirstName = "John",
    LastName = "Doe",
    Email = "johndoe@gmail.com"
};

// Raw JSON patch document
string jsonPatch = """
[
    { "op": "replace", "path": "/Email", "value": "janedoe@gmail.com"},
    { "op": "test", "path": "/FirstName", "value": "Jane" },
    { "op": "replace", "path": "/LastName", "value": "Smith" }
]
""";

// Deserialize the JSON patch document
var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<Person>>(jsonPatch);

// Apply the JSON patch document, catching any errors
Dictionary<string, string[]>? errors = null;
patchDoc!.ApplyTo(person, jsonPatchError =>
    {
        errors ??= new ();
        var key = jsonPatchError.AffectedObject.GetType().Name;
        if (!errors.ContainsKey(key))
        {
            errors.Add(key, new string[] { });
        }
        errors[key] = errors[key].Append(jsonPatchError.ErrorMessage).ToArray();
    });
if (errors != null)
{
    // Print the errors
    foreach (var error in errors)
    {
        Console.WriteLine($"Error in {error.Key}: {string.Join(", ", error.Value)}");
    }
}

// Output updated object
Console.WriteLine(JsonSerializer.Serialize(person, serializerOptions));

// Output:
// {
//   "firstName": "John",
//   "lastName": "Smith",
//   "email": "janedoe@gmail.com",
//   "phoneNumbers": []
// }
}


