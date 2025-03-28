﻿using HSeditor.Classes.Other;
using System.Windows;
using System.Windows.Controls;

namespace HSeditor.Windows
{
    /// <summary>
    /// Interaktionslogik für ContextMenuInv.xaml
    /// </summary>
    public partial class ContextMenuInv : ContextMenu
    {
        BindStruct bindStruct;

        private double startHeight;
        private double startWidth;
        struct BindStruct
        {
            public Item Item;
            public DragHandler.Origin Origin;
            public EquipmentSlot? EquipmentSlot;

            public BindStruct(Item item, DragHandler.Origin origin, EquipmentSlot? equipmentSlot)
            {
                this.Item = item;
                this.Origin = origin;
                this.EquipmentSlot = equipmentSlot;
            }
        }

        public ContextMenuInv(Item item, DragHandler.Origin origin, EquipmentSlot? slot)
        {
            InitializeComponent();
            this.bindStruct = new BindStruct(item, origin, slot);
            this.DataContext = this.bindStruct;
        }

        private void ContextMenu_CopyString_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            // Clipboard.SetText(this.bindStruct.Item.GetItemString());
            Forge forge = new Forge(bindStruct.Item);
        }

        private void ContextMenu_DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            switch (bindStruct.Origin)
            {
                case DragHandler.Origin.Equip:
                    bindStruct.EquipmentSlot.SetItem(null);
                    MainWindow.INSTANCE.UpdateEquippedItems();
                    break;
                case DragHandler.Origin.Inventory:
                    MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Remove(bindStruct.Item);
                    MainWindow.INSTANCE.UpdateInventory();
                    break;
                case DragHandler.Origin.Stash:
                    MainWindow.INSTANCE.SaveFileHandler.Shop.Stash.Remove(bindStruct.Item);
                    MainWindow.INSTANCE.UpdateStash();
                    break;
            }
        }

        private void ContextMenu_NewSeed_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            // Generate a random seed between 1 and 99999999 (8 digits)
            int randomSeed = MainWindow.INSTANCE.random.Next(1, 100000000);
            
            // Update the item's seed value
            if (!bindStruct.Item.SaveItem.ContainsKey("seed"))
                bindStruct.Item.SaveItem.Add("seed", randomSeed);
            else
                bindStruct.Item.SaveItem["seed"] = randomSeed;

            bindStruct.Item.RollID = randomSeed;

            // Update the appropriate view based on the origin
            switch (bindStruct.Origin)
            {
                case DragHandler.Origin.Equip:
                    MainWindow.INSTANCE.UpdateEquippedItems();
                    break;
                case DragHandler.Origin.Inventory:
                    MainWindow.INSTANCE.UpdateInventory();
                    break;
                case DragHandler.Origin.Stash:
                    MainWindow.INSTANCE.UpdateStash();
                    break;
            }
        }

        private void ContextMenu_NewSeedAll_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            
            switch (bindStruct.Origin)
            {
                case DragHandler.Origin.Equip:
                    // Randomize seeds for all equipped items
                    foreach (var slot in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipment())
                    {
                        if (slot.Item != null)
                        {
                            int randomSeed = MainWindow.INSTANCE.random.Next(1, 100000000);
                            
                            if (!slot.Item.SaveItem.ContainsKey("seed"))
                                slot.Item.SaveItem.Add("seed", randomSeed);
                            else
                                slot.Item.SaveItem["seed"] = randomSeed;

                            slot.Item.RollID = randomSeed;
                        }
                    }
                    MainWindow.INSTANCE.UpdateEquippedItems();
                    break;
                    
                case DragHandler.Origin.Inventory:
                    // Randomize seeds for all items in the current inventory tab
                    foreach (var item in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.InventoryItems)
                    {
                        if (item.Inv == MainWindow.INSTANCE.SelectedInv)
                        {
                            int randomSeed = MainWindow.INSTANCE.random.Next(1, 100000000);
                            
                            if (!item.SaveItem.ContainsKey("seed"))
                                item.SaveItem.Add("seed", randomSeed);
                            else
                                item.SaveItem["seed"] = randomSeed;

                            item.RollID = randomSeed;
                        }
                    }
                    MainWindow.INSTANCE.UpdateInventory();
                    break;
                    
                case DragHandler.Origin.Stash:
                    // Randomize seeds for all items in the current stash tab
                    foreach (var item in MainWindow.INSTANCE.SaveFileHandler.Shop.Stash)
                    {
                        if (item.Inv == MainWindow.INSTANCE.SelectedStash)
                        {
                            int randomSeed = MainWindow.INSTANCE.random.Next(1, 100000000);
                            
                            if (!item.SaveItem.ContainsKey("seed"))
                                item.SaveItem.Add("seed", randomSeed);
                            else
                                item.SaveItem["seed"] = randomSeed;

                            item.RollID = randomSeed;
                        }
                    }
                    MainWindow.INSTANCE.UpdateStash();
                    break;
            }
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
            if (this.startHeight == 0) this.startHeight = this.ActualHeight;
            if (this.startWidth == 0) this.startWidth = this.ActualWidth;

            this.Width = this.startWidth * (bindStruct.Origin == DragHandler.Origin.Stash ? MainWindow.INSTANCE.Stash.SizeFactor : MainWindow.INSTANCE.SizeFactor);
            this.Height = this.startHeight * (bindStruct.Origin == DragHandler.Origin.Stash ? MainWindow.INSTANCE.Stash.SizeFactor : MainWindow.INSTANCE.SizeFactor);
        }

        private void ContextMenu_ClearItems_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            switch (bindStruct.Origin)
            {
                case DragHandler.Origin.Equip:
                    MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.Clear();
                    MainWindow.INSTANCE.UpdateEquippedItems();
                    break;
                case DragHandler.Origin.Inventory:
                    MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.InventoryItems.RemoveAll(o => o.Inv == MainWindow.INSTANCE.SelectedInv);
                    MainWindow.INSTANCE.UpdateInventory();
                    break;
                case DragHandler.Origin.Stash:
                    MainWindow.INSTANCE.SaveFileHandler.Shop.Stash.RemoveAll(o => o.Inv == MainWindow.INSTANCE.SelectedStash);
                    MainWindow.INSTANCE.UpdateStash();
                    break;
            }
        }

        private void ContextMenu_EditBase_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as Button).Visibility = bindStruct.Item.isRuneword ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_EditBase_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            RunewordBaseSelection runewordBaseSelection = new RunewordBaseSelection(bindStruct.Item, bindStruct.Origin);
        }
    }
}
