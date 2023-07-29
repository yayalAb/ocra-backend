using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace AppDiv.CRVS.Application.Service
{
    public class ConvertFileToObjectService : IConvertFileToObjectService
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;
        public ConvertFileToObjectService(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<BaseResponse> ConvertFileToObject(IFormFile ImportedFile, Guid? id, Guid? AreaTypeId, CancellationToken cancellationToken)
        {
            using (var package = new ExcelPackage(ImportedFile.OpenReadStream()))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int columnCount = worksheet.Dimension.Columns;

                var addresses = Enumerable.Range(2, rowCount - 1)
                    .Select(row =>
                    {
                        JObject jobject = new JObject();
                        jobject["am"] = worksheet.Cells[row, 1].Value?.ToString();
                        jobject["en"] = worksheet.Cells[row, 2].Value?.ToString();
                        jobject["or"] = worksheet.Cells[row, 3].Value?.ToString();

                        return new Address
                        {
                            AddressName = jobject,
                            StatisticCode = worksheet.Cells[row, 4].Value?.ToString(),
                            Code = worksheet.Cells[row, 5].Value.ToString(),
                            CodePerfix = worksheet.Cells[row, 6].Value.ToString(),
                            CodePostfix = worksheet.Cells[row, 7].Value.ToString(),
                            AdminLevel = int.Parse(worksheet.Cells[row, 7].Value.ToString()),
                            ParentAddressId = id,
                            AreaTypeLookupId = AreaTypeId
                        };
                    })
                    .ToList();
                var AddressTo = CustomMapper.Mapper.Map<ICollection<Address>>(addresses);
                await _AddresslookupRepository.Import(addresses, cancellationToken);
                await _AddresslookupRepository.SaveChangesAsync(cancellationToken);
                foreach (var add in addresses)
                {
                    Console.WriteLine("address : {0} ", add.Code);
                }


                return new BaseResponse
                {
                    Message = ""
                };
            }


        }
    }
}

