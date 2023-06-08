namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PersonIdObj
    {

       public Guid? MotherId {get; set; }
       public Guid? FatherId { get; set; }
       public Guid? ChildId { get; set; }
       public Guid? DeceasedId {get; set; }
       public Guid? RegistrarId {get; set; }
       public Guid? WifeId{ get; set; }
       public Guid? HusbandId{ get; set; }
       public List<Guid>? WitnessIds { get; set;}



    }
}