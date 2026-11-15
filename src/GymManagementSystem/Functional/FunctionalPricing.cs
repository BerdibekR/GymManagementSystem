using GymManagementSystem.Models;

namespace GymManagementSystem.Functional;

public static class FunctionalPricing
{
    public static Func<decimal, Func<MembershipType, decimal>>
        CreateDiscountCalculator()
    {
        return discountPercent =>
            membershipType =>
            {
                if (discountPercent < 0 || discountPercent > 100)
                    throw new ArgumentOutOfRangeException(
                        nameof(discountPercent));

                var basePrice = membershipType switch
                {
                    MembershipType.Basic => 10000m,
                    MembershipType.Standard => 18000m,
                    MembershipType.Premium => 30000m,
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(membershipType))
                };

                return basePrice * (1m - discountPercent / 100m);
            };
    }

    public static Func<MembershipType, decimal> CreateStudentPricing()
    {
        var discountCalculator = CreateDiscountCalculator();

        return discountCalculator(20m);
    }

    public static Func<MembershipType, decimal> CreateVIPPricing()
    {
        var discountCalculator = CreateDiscountCalculator();

        return discountCalculator(10m);
    }
}