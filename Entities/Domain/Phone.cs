using CSharpFunctionalExtensions;
using PhoneNumbers;
namespace Entities.Domain
{
    public class Phone : Entity<int>
    {
        public string? Name { get; set; }
        public PhoneNumber? PhoneNumder { get; set; }
    }
}
