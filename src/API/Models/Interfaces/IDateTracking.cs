﻿namespace API.Models.Interfaces;

public interface IDateTracking
{
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    DateTime UpdatedDate { get; set; }
}
