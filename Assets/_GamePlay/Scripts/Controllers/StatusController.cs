namespace Controllers
{
    public enum StatusController
    {
        Success,
        NoEnoughMoney,
        Immature,
        NoProducts,
        InsufficientItems,
        InvalidLandIndex,
        LandAlreadyOccupied,
        ShopItemNotFound,
        InvalidItemType,
        NoAgriculture,
        ProducedLimited,
        ProducedNoLimited,

        AgricultureImmature,
        AgricultureHalfMature,
        AgricultureMature,
        AgricultureMatureLimit,
        AgricultureDead,


        Other = 1000
    }
}
