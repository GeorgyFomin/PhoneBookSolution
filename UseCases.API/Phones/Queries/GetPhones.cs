using AutoMapper;
using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Exceptions;
using UseCases.API.Dto;
namespace UseCases.API.Phones.Queries
{
    public class GetPhones
    {
        public class Query : IRequest<IEnumerable<PhoneDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<PhoneDto>>
        {
            private readonly PhonesDBContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(PhonesDBContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<IEnumerable<PhoneDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Phones == null)
                {
                    return Enumerable.Empty<PhoneDto>();
                }
                List<Phone> phones = await _context.Phones
                    .ToListAsync(cancellationToken);
                if (phones == null)
                {
                    throw new EntityNotFoundException("Phones not found");
                }
                return _mapper.Map<List<PhoneDto>>(phones);
            }
        }
    }
}
