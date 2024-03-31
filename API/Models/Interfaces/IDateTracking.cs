namespace API.Models.Interfaces;

public interface IDateTracking
{
    DateTime CreateDate { get; set; }

    DateTime? UpdateDate { get; set; }
}
