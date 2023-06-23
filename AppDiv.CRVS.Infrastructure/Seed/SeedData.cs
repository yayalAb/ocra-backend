using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Entities.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Infrastructure.Seed
{
    internal class SeedData
    {
        internal static void SeedUsers(ModelBuilder builder)
        {
            List<UserGroup> groups = new List<UserGroup>();
            List<RoleDto> roles = new List<RoleDto>{
                    new RoleDto{
                        page = "dashboard",
                        title = "dashboard",
                        canAdd = true,
                        canDelete = true,
                        canView = true,
                        canViewDetail = true,
                        canUpdate = true
                        },
                        new RoleDto{
                        page = "birth",
                        title = "birth",
                        canAdd = true,
                        canDelete = true,
                        canView = true,
                        canViewDetail = true,
                        canUpdate = true
                        },
                        new RoleDto{
                        page = "death",
                        title = "death",
                        canAdd = true,
                        canDelete = true,
                        canView = true,
                        canViewDetail = true,
                        canUpdate = true
                        }
                };
            var groupId = new Guid("67998869-cebb-4d3f-a241-fb96b350993f");
            groups.Add(new UserGroup
            {
                Id = groupId,
                GroupName = "adminGroup",

                Roles = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(roles))
            });
            var personalInfoId = new Guid("67998869-cebb-4d3f-a241-fb96b350993f");
            var personalInfo = new PersonalInfo
            {
                Id = personalInfoId,
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

            };
            ApplicationUser user = new ApplicationUser()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                LockoutEnabled = false,
                PhoneNumber = "1234567890",
                PersonalInfoId = personalInfoId,

            };

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123");

            builder.Entity<PersonalInfo>().HasData(personalInfo);
            // builder.Entity<UserGroup>().HasData(groups.First());

            builder.Entity<ApplicationUser>().HasData(user);

            // }

            // internal static void SeedRoles(ModelBuilder builder)
            // {
            //     builder.Entity<IdentityRole>().HasData(
            //         new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
            //         new IdentityRole() { Id = "c7b013f0-5201-4317-abd8-c211f91b7330", Name = "HR", ConcurrencyStamp = "2", NormalizedName = "Human Resource" }
            //         );
            // }

            // internal static void SeedUserRoles(ModelBuilder builder)
            // {
            //     builder.Entity<IdentityUserRole<string>>().HasData(
            //         new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
            //         );
            // }

            // internal static void SeedGender(ModelBuilder builder)
            // {
            //     builder.Entity<Gender>().HasData(
            //         new Gender()
            //         {
            //             Name = "Male",
            //             Code = "M",
            //             CreatedAt = DateTime.Now
            //         },
            //         new Gender()
            //         {
            //             Name = "Female",
            //             Code = "F",
            //             CreatedAt= DateTime.Now
            //         }
            //   );
            // }
            // internal static void SeedSuffix(ModelBuilder builder)
            // {
            //     builder.Entity<Suffix>().HasData(             

            //         new Suffix()
            //         {
            //             Name = "Mr.",
            //             CreatedAt= DateTime.Now,
            //         },
            //         new Suffix()
            //         {
            //             Name="Mrs.",
            //             CreatedAt= DateTime.Now,

            //         }

            //   );
        }
    }
}
