using Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public static class TextConstant
    {
        public static Dictionary<StatusController, string> statusController = new Dictionary<StatusController, string>()
        {
            { StatusController.Success, "Operation completed successfully" },
            { StatusController.NoEnoughMoney, "Not enough money" },
            { StatusController.Immature, "Agriculture is not yet mature" },
            { StatusController.NoProducts, "No products available" },
            { StatusController.InsufficientItems, "Insufficient items" },
            { StatusController.InvalidLandIndex, "Invalid land index" },
            { StatusController.LandAlreadyOccupied, "Land is already occupied" },
            { StatusController.ShopItemNotFound, "Shop item not found" },
            { StatusController.InvalidItemType, "Invalid item type" },
            { StatusController.NoAgriculture, "No agriculture found on the land" },
            { StatusController.ProducedLimited, "Production has reached its limit" },
            { StatusController.Other, "An unknown error occurred" }
        };


        public const string NOTIFICATION = "Notification";
        public const string EXPAND_LAND = "Do you want to expand the land for {0} price?";
        public const string HIRE_WORKER = "Do you want to hire workers at price {0}?";
        public const string EXPAND_LAND_SUCCESS = "Purchased successfully please check your bag";
        public const string HIRE_WORKER_SUCCESS = "Hire Workers Successfully";
        public const string YES = "Yes";
        public const string NO = "No";
        public const string OK = "Ok";
        public const string TIME_TO_MATURE = "Time to mature";
        public const string PRODUCTION = "Production: {0}";
        public const string PRODUCTION_LIMITED = "Production limited: {0}";
        public const string AGRICULTURE_DEAD = "Agriculture Dead";
        public const string DESTROY_AGRICULTURE = "Do you want to destroy this agriculture?";
        public const string LEVEL = "Level: {0}";
        public const string PRODUCTIVITY_INCREASED = "Productivity increased by {0}%";
    }
}
