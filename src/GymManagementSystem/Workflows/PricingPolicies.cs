using GymManagementSystem.Models;

namespace GymManagementSystem.Workflows;

public static class PricingPolicies
{
    public static decimal StandardPricing(
        MembershipType type)
    {
        return type switch
        {
            MembershipType.Basic => 10000m,
            MembershipType.Standard => 18000m,
            MembershipType.Premium => 30000m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Unknown membership type.")
        };
    }

    public static Func<MembershipType, decimal>
        DiscountPricing(decimal discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentOutOfRangeException(
                nameof(discountPercent));

        return type =>
            StandardPricing(type) *
            (1m - discountPercent / 100m);
    }

    public static Func<MembershipType, decimal>
        FixedPricing(decimal price)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(price));

        return _ => price;
    }
}