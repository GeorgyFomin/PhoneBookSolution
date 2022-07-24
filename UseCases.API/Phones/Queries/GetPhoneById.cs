using AutoMapper;
using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;
namespace UseCases.API.Phones.Queries
{
    public class GetPhoneById
    {
        public class Query : IRequest<PhoneDto?>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, PhoneDto?>
        {
            private readonly PhonesDBContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(PhonesDBContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<PhoneDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Phones == null)
                {
                    return null;
                }
                Phone? phone = await _context
                    .Phones
                    .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
                if (phone == null)
                {
                    throw new EntityNotFoundException("Phone not found");
                }
                return _mapper.Map<PhoneDto?>(phone);
            }
        }
    }
}
