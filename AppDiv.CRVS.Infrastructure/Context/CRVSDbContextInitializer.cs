
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure
{
    public class CRVSDbContextInitializer
    {
        private readonly ILogger<CRVSDbContextInitializer> _logger;
        private readonly CRVSDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILookupRepository _lookupRepository;
        private readonly IAddressLookupRepository _addressLookupRepository;
        private readonly ISettingRepository _settingRepository;


        public CRVSDbContextInitializer(ILogger<CRVSDbContextInitializer> logger,
                                        CRVSDbContext context,
                                        UserManager<ApplicationUser> userManager,
                                        RoleManager<IdentityRole> roleManager,
                                        ILookupRepository lookupRepository,
                                        IAddressLookupRepository addressLookupRepository,
                                        ISettingRepository settingRepository
                                        )
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _lookupRepository = lookupRepository;
            _addressLookupRepository = addressLookupRepository;
            _settingRepository = settingRepository;
        }

        public async Task InitialiseAsync()
        {
            try
            {

                // await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
    }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            await SeedUser();
            await SeedSystemLookups();
            await SeedSetting();
            await _lookupRepository.InitializeLookupCouch();
            await _addressLookupRepository.InitializeAddressLookupCouch();
            await _settingRepository.InitializeSettingCouch();  
        }
        public async Task SeedSetting()
        {
            if (!_context.Settings.Where(s => s.Key == "DateSetting").Any())
            {
                var currentYearSetting = new Setting
                {
                    Key = "DateSetting",
                    Value = new JObject{
                    {"currentYear",DateTime.Now.Year}
                }
                };
                await _context.Settings.AddAsync(currentYearSetting);
                await _context.SaveChangesAsync();
            }
        }
        public async Task SeedUser()
        {
            List<UserGroup> groups = new List<UserGroup>();
            List<RoleDto> roles = new List<RoleDto>();
            List<RoleDto> roles2 = new List<RoleDto>();
            Enum.GetNames(typeof(Page)).ToList().ForEach(page =>
            {
                roles.Add(new RoleDto
                {
                    page = page,
                    title = page,
                    canAdd = true,
                    canDelete = true,
                    canView = true,
                    canViewDetail = true,
                    canUpdate = true
                });
                roles2.Add(new RoleDto
                {
                    page = page,
                    title = page,
                    canAdd = true,
                    canDelete = true,
                    canView = true,
                    canViewDetail = true,
                    canUpdate = true
                });
            });

            var groupId = new Guid("67998869-cebb-4d3f-a241-fb96b350993f");
            groups.Add(new UserGroup
            {
                // Id = groupId,
                GroupName = "adminGroup",
                Roles = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(roles))
            }
            );
            groups.Add(new UserGroup
            {
                // Id = groupId,
                GroupName = "registrarGroup",
                Roles = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(roles2))
            }
            );
            var personalInfoId = new Guid("67998869-cebb-4d3f-a241-fb96b350993f");


            ApplicationUser user = new ApplicationUser()
            {
                // Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                LockoutEnabled = false,
                PhoneNumber = "1234567890",
                Address = new Address
                {
                    AddressName = new JObject{
                        {"en","Kebele User Address"}
                      },
                    StatisticCode = "code34726746",
                    Code = "cc8989890809",
                    AdminLevel = 1,
                    AreaTypeLookup = new Lookup
                    {
                        Key = "AreaType",
                        Value = new JObject{
                        {"en","zone"}
                      }
                    },

                },
                PersonalInfo = new PersonalInfo
                {
                    // Id = personalInfoId,
                    FirstName = new JObject{
                      {"en","admin"} ,
                    },
                    MiddleName = new JObject{
                        {"en","admin"}
                    },
                    NationalId = "jwpi897587348",
                    SexLookup = new Lookup
                    {
                        Key = "sex",
                        Value = new JObject{
                        {"en","male"}
                      }
                    },
                    NationalityLookup = new Lookup
                    {
                        Key = "nationality",
                        Value = new JObject{
                        {"en","Ethiopian"}
                      }
                    },
                    EducationalStatusLookup = new Lookup
                    {
                        Key = "Edu",
                        Value = new JObject{
                        {"en","Ethiopian"}
                      }
                    },
                    MarraigeStatusLookup = new Lookup
                    {
                        Key = "MarriageStatus",
                        Value = new JObject{
                        {"en","single"}
                      }
                    },
                    NationLookup = new Lookup
                    {
                        Key = "Nation",
                        Value = new JObject{
                        {"en","oromo"}
                      }
                    },
                    ContactInfo = new ContactInfo
                    {
                        Email = "admin@gmail.com"
                    },
                    BirthAddress = new Address
                    {
                        AddressName = new JObject{
                        {"en","some place"}
                      },
                        StatisticCode = "code34726746",
                        Code = "cc8989890809",
                        AdminLevel = 1,
                        AreaTypeLookup = new Lookup
                        {
                            Key = "AreaType",
                            Value = new JObject{
                        {"en","zone"}
                      }
                        },

                    },
                    ResidentAddress = new Address
                    {
                        AddressName = new JObject{
                        {"en","some place"}
                      },
                        StatisticCode = "code34726746",
                        Code = "cc8989890809",
                        AdminLevel = 1,
                        AreaTypeLookup = new Lookup
                        {
                            Key = "AreaType",
                            Value = new JObject{
                        {"en","zone"}
                      }
                        },

                    },

                },
                UserGroups = groups,


            };

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123");
            await _userManager.CreateAsync(user);


        }

        public async Task SeedSystemLookups()
        {
            var eventTypeLookups = new List<Lookup>();
            EnumDictionary.eventTypeDict.ToList().ForEach(eventType =>
            {
                eventTypeLookups.Add(new Lookup
                {
                    Key = "event",
                    Value = new JObject{
                        {"en", eventType.Value.en},
                        {"am", eventType.Value.am},
                        {"or", eventType.Value.or}
                    },
                    IsSystemLookup = true,
                    StatisticCode = eventType.Value.statisticCode,
                    Code = eventType.Value.code
                });
            });
            var facilityLookups = new List<Lookup>();
            EnumDictionary.facilityDict.ToList().ForEach(facility =>
            {
                facilityLookups.Add(new Lookup
                {
                    Key = "facility",
                    Value = new JObject{
                        {"en", facility.Value.en},
                        {"am", facility.Value.am},
                        {"or", facility.Value.or}
                    },
                    IsSystemLookup = true,
                    StatisticCode = facility.Value.statisticCode,
                    Code = facility.Value.code
                });
            });
            var marriageStatusLookups = new List<Lookup>();
            EnumDictionary.marriageStatusDict.ToList().ForEach(marriageStatus =>
            {
                marriageStatusLookups.Add(new Lookup
                {
                    Key = "marriage-status",
                    Value = new JObject{
                        {"en", marriageStatus.Value.en},
                        {"am", marriageStatus.Value.am},
                        {"or", marriageStatus.Value.or}
                    },
                    IsSystemLookup = true,
                    StatisticCode = marriageStatus.Value.statisticCode,
                    Code = marriageStatus.Value.code
                });
            });
            var marriageTypeLookups = new List<Lookup>();
            EnumDictionary.marriageTypeDict.ToList().ForEach(marriageType =>
            {
                marriageTypeLookups.Add(new Lookup
                {
                    Key = "form-of-marriage",
                    Value = new JObject{
                        {"en", marriageType.Value.en},
                        {"am", marriageType.Value.am},
                        {"or", marriageType.Value.or}
                    },
                    IsSystemLookup = true,
                    StatisticCode = marriageType.Value.statisticCode,
                    Code = marriageType.Value.code
                });
            });
            var paymentTypeLookups = new List<Lookup>();
            EnumDictionary.paymentTypeDict.ToList().ForEach(paymentType =>
            {
                paymentTypeLookups.Add(new Lookup
                {
                    Key = "payment-type",
                    Value = new JObject{
                        {"en", paymentType.Value.en},
                        {"am", paymentType.Value.am},
                        {"or", paymentType.Value.or}
                    },
                    IsSystemLookup = true,
                    StatisticCode = paymentType.Value.statisticCode,
                    Code = paymentType.Value.code
                });
            });
            if (!_context.Lookups.Where(l => l.Key == "event").Any())
            {

                await _context.Lookups.AddRangeAsync(eventTypeLookups);
            }
            if (!_context.Lookups.Where(l => l.Key == "facility").Any())
            {

                await _context.Lookups.AddRangeAsync(facilityLookups);
            }
            if (!_context.Lookups.Where(l => l.Key == "marriage-status").Any())
            {
                await _context.Lookups.AddRangeAsync(marriageStatusLookups);
            }
            if (!_context.Lookups.Where(l => l.Key == "form-of-marriage").Any())
            {
                await _context.Lookups.AddRangeAsync(marriageTypeLookups);
            }
            if (!_context.Lookups.Where(l => l.Key == "payment-type").Any())
            {

                await _context.Lookups.AddRangeAsync(paymentTypeLookups);
            }
            await _context.SaveChangesAsync();




        }
    }
}