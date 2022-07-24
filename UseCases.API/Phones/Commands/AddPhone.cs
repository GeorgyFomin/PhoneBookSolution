using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using PhoneNumbers;

namespace UseCases.API.Phones.Commands
{
    public class AddPhone
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
            public string? Name { get; set; }
            public ulong PhoneNumber { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly PhonesDBContext _context;
            public CommandHandler(PhonesDBContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Phone phone = new()
                {
                    Name = request.Name,
                    PhoneNumder = GetPhoneNumber(request.PhoneNumber.ToString())
                };
                if (_context.Phones == null)
                {
                    return default;
                }
                await _context.Phones.AddAsync(phone, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return phone.Id;
            }
        }
    }
}
