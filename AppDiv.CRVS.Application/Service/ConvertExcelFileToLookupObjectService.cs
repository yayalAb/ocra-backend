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
    public class ConvertExcelFileToLookupObjectService : IConvertExcelFileToLookupObjectService
    {
        private readonly ILookupRepository _LookupRepository;
        public ConvertExcelFileToLookupObjectService(ILookupRepository LookupRepository)
        {
            _LookupRepository = LookupRepository;
        }
        public async Task<BaseResponse> ConvertFileToObject(IFormFile ImportedFile, CancellationToken cancellationToken)
        {
            using (var package = new ExcelPackage(ImportedFile.OpenReadStream()))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int columnCount = worksheet.Dimension.Columns;

                var lookups = Enumerable.Range(2, rowCount - 1)
                    .Select(row =>
                    {
                        JObject jobject = new JObject();
                        jobject["am"] = worksheet.Cells[row, 2].Value?.ToString();
                        jobject["en"] = worksheet.Cells[row, 3].Value?.ToString();
                        jobject["or"] = worksheet.Cells[row, 4].Value?.ToString();

                        return new Lookup
                        {
                            Key = worksheet.Cells[row, 1].Value?.ToString(),
                            Value = jobject,
                            StatisticCode = worksheet.Cells[row, 5].Value?.ToString(),
                            Code = worksheet.Cells[row, 6].Value?.ToString(),
                            IsSystemLookup = false
                        };
                    })
                    .ToList();
                await _LookupRepository.Import(lookups, cancellationToken);
                await _LookupRepository.SaveChangesAsync(cancellationToken);
                foreach (var add in lookups)
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

