namespace BusinessLogic;

public class ConstantEnum
{
    public enum Role
    {
        Admin = 1,
        Patient = 2,
        MedicalExpert = 3
    }

    public enum AppointmentStatus
    {
        Confirmed,
        Pending,
        Cancelled,
    }

    public enum PaymentStatus
    {
        Success = 0,
    }
}