using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetDefualtAddress
{
    // Customer DefualtAddressQuery with  response
    public class DefualtAddressQuery : IRequest<List<AddressForLookupDTO>>
    {
        public bool IsRegion { get; set; } = false;

    }

    public class DefualtAddressQueryHandler : IRequestHandler<DefualtAddressQuery, List<AddressForLookupDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;
        private readonly ISettingRepository _SettinglookupRepository;

        public DefualtAddressQueryHandler(IAddressLookupRepository AddresslookupRepository, ISettingRepository SettinglookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
            _SettinglookupRepository = SettinglookupRepository;
        }
        public async Task<List<AddressForLookupDTO>> Handle(DefualtAddressQuery request, CancellationToken cancellationToken)
        {
            var defualtAddress = _SettinglookupRepository.GetAll().Where(x => x.Key == "generalSetting").FirstOrDefault();
            if (defualtAddress == null)
            {
                throw new NotFoundException("Defualt Address not Found");
            }
            Guid defualtCountryId = new Guid(defualtAddress.Value.Value<JObject>("defaults").Value<string>("default_country"));
            Guid defualtRegionId = new Guid(defualtAddress.Value.Value<JObject>("defaults").Value<string>("default_region"));


            Guid parentId = Guid.Empty;
            if (request.IsRegion)
            {
                parentId = defualtCountryId;
            }
            else
            {
                parentId = defualtRegionId;
            }
            if (parentId == null)
            {
                throw new NotFoundException("not found");
            }
            var selectedAddress = _AddresslookupRepository.GetAll().
             Include(ad => ad.AdminTypeLookup)
            .Where(x => x.ParentAddressId == (Guid.Equals(parentId, Guid.Empty) ? null : parentId) && !x.Status);
            // var lng = "";
            var formatedAddress = selectedAddress.Select(an => new AddressForLookupDTO
            {
                id = an.Id,
                ParentAddressId = an.ParentAddressId,
                AddressName = an.AddressNameLang,
                AdminType = string.IsNullOrEmpty(an.AdminTypeLookup.ValueLang) ? "" : an.AdminTypeLookup.ValueLang
            });
            return formatedAddress.ToList();            //CustomMapper.Mapper.Map<List<AddressForLookupDTO>>(formatedAddress);
            // return selectedCustomer;
        }
    }
}