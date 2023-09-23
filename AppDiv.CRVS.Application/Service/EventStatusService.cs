using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Service
{
    public class EventStatusService:IEventStatusService
    {
         private readonly ISettingRepository _SettinglookupRepository;
        public EventStatusService(ISettingRepository SettinglookupRepository)
        {
            _SettinglookupRepository=SettinglookupRepository;
        }
      public   string ReturnEventStatus(string EventType,DateTime eventDate,DateTime EventRegDate){
         int days=0;
         int daysDelay=0;
          var setting=_SettinglookupRepository.GetAll();
            var birthSetting = setting.Where(x => x.Key == "birthSetting").FirstOrDefault();
            int BirthSetting =   int.Parse(birthSetting!.Value.Value<string>("active_registration")!);
            int BirthSettingDelay =   int.Parse(birthSetting!.Value.Value<string>("delay_registration")!);

            var adoptionSetting = setting.Where(x => x.Key == "adoptionSetting").FirstOrDefault();
            int AdoptionSetting = int.Parse(adoptionSetting!.Value.Value<string>("active_registration")!);
            int AdoptionSettingDelay = int.Parse(adoptionSetting!.Value.Value<string>("delay_registration")!);

            var marriageSetting = setting.Where(x => x.Key == "marriageSetting").FirstOrDefault();
            int MarriageSetting = int.Parse(marriageSetting!.Value.Value<string>("active_registration")!);
            int MarriageSettingDelay = int.Parse(marriageSetting!.Value.Value<string>("delay_registration")!);

            var divorceSetting = setting.Where(x => x.Key == "divorceSetting").FirstOrDefault();
            int DivorceSetting = int.Parse(divorceSetting!.Value.Value<string>("active_registration")!);
            int DivorceSettingDelay = int.Parse(divorceSetting!.Value.Value<string>("delay_registration")!);

            var deathSetting = setting.Where(x => x.Key == "deathSetting").FirstOrDefault();
            int DeathSetting =  int.Parse(deathSetting!.Value.Value<string>("active_registration")!);
            int DeathSettingDelay =  int.Parse(deathSetting!.Value.Value<string>("delay_registration")!);

            switch(EventType.ToLower()){
               case "birth":
                     days=BirthSetting;
                     daysDelay=BirthSettingDelay;
                     break; 
                case "death":
                     days=DeathSetting;
                     daysDelay=DeathSettingDelay;
                     break;
                case "divorce":
                     days=DivorceSetting;
                     daysDelay=DivorceSettingDelay;
                     break;
                case "adoption":
                     days=AdoptionSetting;
                     daysDelay=AdoptionSettingDelay;
                     break;
                case "marriage":
                     days=MarriageSetting;
                     daysDelay=MarriageSettingDelay;
                     break;
            }
            TimeSpan deff = EventRegDate - eventDate;
            int daysDef = Convert.ToInt32(deff.TotalDays);
            return daysDef <= days?"Active":daysDef <= daysDelay?"Delay":"Late";

      }
    }
}