using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Domain.Enums
{
    public class lookupModel {
        public string? en {get; set; }
        public string? am {get; set; }
        public string? or {get; set; }
        public string? statisticCode {get; set; }
        public string? code {get; set; }
        

        

    }
    public  class EnumDictionary
    {
        public static Dictionary<Facility, lookupModel> facilityDict = new Dictionary<Facility, lookupModel>{
            {
                Facility.Government , new lookupModel{
                    en = Enum.GetName<Facility>(Facility.Government),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                Facility.NonGovernment , new lookupModel{
                    en = Enum.GetName<Facility>(Facility.NonGovernment),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                Facility.Private , new lookupModel{
                    en = Enum.GetName<Facility>(Facility.Private),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                Facility.OutOfFacility , new lookupModel{
                    en = Enum.GetName<Facility>(Facility.OutOfFacility),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            }

     };
        public static Dictionary<EventType, lookupModel> eventTypeDict = new Dictionary<EventType, lookupModel>{
            {
                EventType.Birth , new lookupModel{
                    en = Enum.GetName<EventType>(EventType.Birth),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                EventType.Death , new lookupModel{
                    en = Enum.GetName<EventType>(EventType.Death),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                EventType.Marriage , new lookupModel{
                    en = Enum.GetName<EventType>(EventType.Marriage),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                EventType.Adoption , new lookupModel{
                    en = Enum.GetName<EventType>(EventType.Adoption),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
             {
                EventType.Divorce , new lookupModel{
                    en = Enum.GetName<EventType>(EventType.Divorce),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            }

     };

             public static Dictionary<MarriageStatus, lookupModel> marriageStatusDict = new Dictionary<MarriageStatus, lookupModel>{
            {
                MarriageStatus.single , new lookupModel{
                    en = Enum.GetName<MarriageStatus>(MarriageStatus.single),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                MarriageStatus.married , new lookupModel{
                    en = Enum.GetName<MarriageStatus>(MarriageStatus.married),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                MarriageStatus.divorced , new lookupModel{
                    en = Enum.GetName<MarriageStatus>(MarriageStatus.divorced),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                MarriageStatus.widowed , new lookupModel{
                    en = Enum.GetName<MarriageStatus>(MarriageStatus.widowed),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            }
     };
      public static Dictionary<MarriageType, lookupModel> marriageTypeDict = new Dictionary<MarriageType, lookupModel>{
            {
                MarriageType.Civil , new lookupModel{
                    en = Enum.GetName<MarriageType>(MarriageType.Civil),
                    or = "seera siivilii",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                MarriageType.Cultural , new lookupModel{
                    en = Enum.GetName<MarriageType>(MarriageType.Cultural),
                    or = "aadaa",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                MarriageType.Religion , new lookupModel{
                    en = Enum.GetName<MarriageType>(MarriageType.Religion),
                    or = "amantaa",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            }
     };
       public static Dictionary<PaymentType, lookupModel> paymentTypeDict = new Dictionary<PaymentType, lookupModel>{
            {
                PaymentType.CertificateGeneration , new lookupModel{
                    en = Enum.GetName<PaymentType>(PaymentType.CertificateGeneration),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                PaymentType.Authentication , new lookupModel{
                    en = Enum.GetName<PaymentType>(PaymentType.Authentication),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            },
            {
                PaymentType.Authorization , new lookupModel{
                    en = Enum.GetName<PaymentType>(PaymentType.Authorization),
                    or = "",
                    am = "",
                    statisticCode = "",
                    code = ""
                }
            }
     };
    }


}