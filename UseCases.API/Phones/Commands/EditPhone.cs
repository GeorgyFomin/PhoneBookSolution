using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using PhoneNumbers;

namespace UseCases.API.Phones.Commands
{
    public class EditPhone
    {
        private static readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
        static PhoneNumber GetPhoneNumber(string phNumber)
        {
            if (string.IsNullOrWhiteSpace(phNumber) || phNumber.Length < 2 || phNumber.Length > 10 || !ulong.TryParse(phNumber, out _))
            {
                phNumber = "10";
            }
            return phoneUtil.Parse("+7" + phNumber, "ru");
        }
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public ulong PhoneNumber { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly PhonesDBContext _context;

            public CommandHandler(PhonesDBContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Phones == null)
                {
                    return default;
                }
                Phone? phone = await _context.Phones.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (phone == null)
                    return default;
                phone.Name = request.Name;
                phone.PhoneNumder = GetPhoneNumber(request.PhoneNumber.ToString());
                await _context.SaveChangesAsync(cancellationToken);
                return phone.Id;
            }
        }

    }
}
