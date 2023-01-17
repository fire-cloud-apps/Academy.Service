namespace Academy.Entity.Common;

public class Contact
{
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    /// <summary>
    /// Building or Flat or Plot Number
    /// </summary>
    public string BuildingNo { get; set; }
    /// <summary>
    /// PinCode or ZipCode
    /// </summary>
    public string PinCode { get; set; }
    public string Country { get; set; }
    public string PrimaryPhoneNo { get; set; }
    public string SecondaryPhoneNo { get; set; }
    public string Email { get; set; }
    
    /// <summary>
    /// Some other notes, about the address or contact details it can be near by location etc.
    /// </summary>
    public string Notes { get; set; }
}