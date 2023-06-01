using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{

    public class EventHistoryDto
    {
        public string EventOwner { get; set; }
        public string? Status { get; set; }
        public string Event { get; set; }
        public string? Informant { get; set; }
        public string EventId { get; set; }
        public string? EventDate { get; set; }
        public string? EventAddress { get; set; }
        public List<EventHistory>? Historys { get; set; }
    }

    public class EventHistory
    {
        public string? Action { get; set; }
        public DateTime? Date { get; set; }
        public string? By { get; set; }
        public string? Type { get; set; }
        public string? Address { get; set; }
    }



}