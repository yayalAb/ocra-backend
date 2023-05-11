
using AppDiv.CRVS.Application.Contracts.DTOs;
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

        public CRVSDbContextInitializer(ILogger<CRVSDbContextInitializer> logger, CRVSDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {

                await _context.Database.MigrateAsync();
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
            // await SeedUser();
            await SeedSetting();


        }
       public async Task SeedSetting(){
        if (!_context.Settings.Where(s => s.Key == "DateSetting").Any()){
            var currentYearSetting = new Setting{
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
                    Page = page,
                    Title = page,
                    CanAdd = true,
                    CanDelete = true,
                    CanView = true,
                    CanViewDetail = true,
                    CanUpdate = true
                });
                roles2.Add(new RoleDto
                {
                    Page = page,
                    Title = page,
                    CanAdd = true,
                    CanDelete = true,
                    CanView = true,
                    CanViewDetail = true,
                    CanUpdate = true
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


    }
}