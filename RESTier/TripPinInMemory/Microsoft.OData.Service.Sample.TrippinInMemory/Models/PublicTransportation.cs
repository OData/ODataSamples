
namespace Microsoft.OData.Service.Sample.TrippinInMemory.Models
{
    public class PublicTransportation : PlanItem
    {
        public string SeatNumber { get; set; }

        public override object Clone()
        {
            var newPlan = new PublicTransportation()
            {
                ConfirmationCode = this.ConfirmationCode,
                Duration = this.Duration,
                EndsAt = this.EndsAt,
                PlanItemId = this.PlanItemId,
                StartsAt = this.StartsAt,
                SeatNumber = this.SeatNumber,
            };
            return newPlan;
        }
    }
}