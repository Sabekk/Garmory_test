using Gameplay.Character;
using Gameplay.Controller.Module;
using Gameplay.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Equipment
{
    public class InventoryModule : ControllerModuleBase
    {
        #region VARIABLES

        [SerializeField] private List<Item> itemsInventory;

        #endregion

        #region PROPERTIES

        public List<Item> ItemsInventory
        {
            get
            {
                if (itemsInventory == null)
                    itemsInventory = new();
                return itemsInventory;
            }
        }
        public int EmptySlots => MaxItemsCount - ItemsInventory.Count;
        private int MaxItemsCount => Character.Data.MaxInventorySlots;

        #endregion

        #region METHODS

        public bool ContainItem(Item item)
        {
            return ItemsInventory.ContainsId(item.Id);
        }

        public bool CanAddItem()
        {
            return EmptySlots > 0;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            Character.EquipmentController.OnItemEquip += HandleItemEquip;
            Character.EquipmentController.OnItemUnequip += HandleItemUnequip;

            Character.EquipmentController.OnItemCollect += HandleItemCollect;
            Character.EquipmentController.OnItemRemove += HandleItemRemove;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            Character.EquipmentController.OnItemEquip -= HandleItemEquip;
            Character.EquipmentController.OnItemUnequip -= HandleItemUnequip;

            Character.EquipmentController.OnItemCollect -= HandleItemCollect;
            Character.EquipmentController.OnItemRemove -= HandleItemRemove;
        }

        private void AddItem(Item item)
        {
            ItemsInventory.Add(item);
        }

        private void RemoveItem(Item item)
        {
            ItemsInventory.Remove(item);
        }

        #region HANDLERS

        private void HandleItemEquip(Item item)
        {
            RemoveItem(item);
        }

        private void HandleItemUnequip(Item item)
        {
            AddItem(item);
        }

        private void HandleItemCollect(Item item)
        {
            AddItem(item);
        }

        private void HandleItemRemove(Item item)
        {
            RemoveItem(item);
        }

        #endregion

        #endregion
    }
}