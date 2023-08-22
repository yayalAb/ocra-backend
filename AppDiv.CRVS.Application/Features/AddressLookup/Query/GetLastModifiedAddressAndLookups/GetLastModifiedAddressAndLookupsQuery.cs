using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetDefualtAddress
{
    public class GetLastModifiedAddressAndLookupsQuery : IRequest<object>
    {
        public DateTime Since { get; set; }

    }

    public class GetLastModifiedAddressAndLookupsQueryHandler : IRequestHandler<GetLastModifiedAddressAndLookupsQuery, object>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public GetLastModifiedAddressAndLookupsQueryHandler(IAddressLookupRepository AddresslookupRepository, ILookupRepository lookupRepository, IAuditLogRepository auditLogRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
            _lookupRepository = lookupRepository;
            _auditLogRepository = auditLogRepository;
        }
        public async Task<object> Handle(GetLastModifiedAddressAndLookupsQuery request, CancellationToken cancellationToken)
        {
            var timestamp = DateTime.Now;
            var lookupRes = _lookupRepository.GetLastModifiedLookups(request.Since).Result;
            var addressRes = _AddresslookupRepository.GetLastUpdatedAddresses(request.Since).Result;
            var deletedIds = _auditLogRepository.GetAll()
                                    .Where(al => al.Action.ToLower() == "delete"
                                        && (al.EntityType.ToLower() == "lookup" || al.EntityType.ToLower() == "address")
                                        && al.AuditDate > request.Since)
                                    .Select(al => new
                                    {
                                        type = al.EntityType,
                                        id = al.TablePk
                                    }).ToList();

            return new
            {
                checkedTimeStamp = timestamp,
                lookups = lookupRes.lookups,
                deletedLookupIds = deletedIds.Where(i => i.type.ToLower() == "lookup").Select(i => i.id).ToList(),
                addresses = addressRes.addresses,
                deletedAddressIds = deletedIds.Where(i => i.type.ToLower() == "address").Select(i => i.id).ToList(),

            };

        }
    }
}