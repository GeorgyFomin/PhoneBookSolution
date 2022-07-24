using Entities.Domain;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Phones.Commands
{
    public class DeletePhone
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, Unit>
        {
            private readonly PhonesDBContext _context;
            public CommandHandler(PhonesDBContext context) => _context = context;
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Phones == null)
                {
                    return Unit.Value;
                }
                Phone? phone = await _context.Phones.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (phone == null) return Unit.Value;
                _context.Phones.Remove(phone);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
