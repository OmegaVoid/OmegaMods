// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Vintagestory.API;
// using Vintagestory.API.Client;
// using Vintagestory.API.Common;
// using Vintagestory.API.Common.Entities;
// using Vintagestory.API.Config;
// using Vintagestory.API.Datastructures;
// using Vintagestory.API.MathTools;
// using Vintagestory.API.Server;
// using Vintagestory.API.Util;
// using Vintagestory.GameContent;
//
//
// namespace BackpackOverhaul.BackpackBase;
// [DocumentAsJson]
// [AddDocumentationProperty("interactPassthrough", "Use this on a collectible type. If True, skips the attach interaction for this item when it is attached and the attach key is not pressed, allowing the interaction to pass through to mount or other entity interactions", "System.Boolean", "Optional", "False", true)]
// public class BlockEntityBehaviorAttachable(BlockEntity blockentity) : 
//   BlockEntityBehavior(blockentity),
//   ICustomInteractionHelpPositioning
// {
//   [DocumentAsJson("Required", "", false)]
//   internal WearableSlotConfig[] wearableSlots;
//   protected InventoryGeneric inv;
//   [DocumentAsJson("Optional", "false", false)]
//   public bool UseShiftAttach;
//
//   public  InventoryBase Inventory => (InventoryBase) this.inv;
//
//   public  string InventoryClassName => "wearablesInv";
//
//   // public override bool TryEarlyLoadCollectibleMappings(
//   //   IWorldAccessor worldForCollectibleResolve,
//   //   Dictionary<int, AssetLocation> oldBlockIdMapping,
//   //   Dictionary<int, AssetLocation> oldItemIdMapping,
//   //   bool resolveImports,
//   //   EntityProperties entityProperties,
//   //   JsonObject behaviorConfig)
//   // {
//   //   (this.Blockentity as BlockEntityGroundStorage).Inventory
//   //   WearableSlotConfig[] wearableSlots = behaviorConfig["wearableSlots"].AsObject<WearableSlotConfig[]>();
//   //   this.inv = new InventoryGeneric(wearableSlots.Length, $"{this.InventoryClassName}-EarlyLoad-{this.entity.EntityId.ToString()}", this.entity.Api, (NewSlotDelegate) ((id, inv) => (ItemSlot) new ItemSlotWearable((InventoryBase) inv, wearableSlots[id].ForCategoryCodes)));
//   //   this.loadInv();
//   //   return this.inv.Empty || base.TryEarlyLoadCollectibleMappings(worldForCollectibleResolve, oldBlockIdMapping, oldItemIdMapping, resolveImports, entityProperties, behaviorConfig);
//   // }
//
//   public void Initialize(EntityProperties properties, JsonObject attributes)
//   {
//     this.Api = this.Blockentity.Api;
//     this.wearableSlots = attributes["wearableSlots"].AsObject<WearableSlotConfig[]>();
//     this.inv = (this.Blockentity as BlockEntityGroundStorage).Inventory as InventoryGeneric;
//     // this.inv = new InventoryGeneric(this.wearableSlots.Length, $"{this.InventoryClassName}-{this.entity.EntityId.ToString()}", this.entity.Api, (NewSlotDelegate) ((id, inv) => (ItemSlot) new ItemSlotWearable((InventoryBase) inv, this.wearableSlots[id].ForCategoryCodes)));
//     // this.loadInv();
//     // this.entity.WatchedAttributes.RegisterModifiedListener((this.Blockentity as BlockEntityGroundStorage).InventoryClassName, new Action(this.wearablesModified));
//     this.UseShiftAttach = attributes["useShiftAttach"].AsBool();
//     base.Initialize(Api, attributes);
//   }
//
//   // public override void AfterInitialized(bool onFirstSpawn)
//   // {
//   //   base.AfterInitialized(onFirstSpawn);
//   //   this.updateSeats();
//   // }
//   //
//   // private void wearablesModified()
//   // {
//   //   this.loadInv();
//   //   this.updateSeats();
//   //   this.entity.MarkShapeModified();
//   // }
//
//   // private void updateSeats()
//   // {
//   //   IVariableSeatsMountable variableSeatsMountable = this.entity.GetInterface<IVariableSeatsMountable>();
//   //   if (variableSeatsMountable == null)
//   //     return;
//   //   for (int slotId = 0; slotId < this.wearableSlots.Length; ++slotId)
//   //   {
//   //     WearableSlotConfig wearableSlot = this.wearableSlots[slotId];
//   //     wearableSlot.SeatConfig = (SeatConfig) null;
//   //     ItemSlot itemSlot = this.inv[slotId];
//   //     if (itemSlot.Empty)
//   //     {
//   //       if (wearableSlot.ProvidesSeatId != null)
//   //       {
//   //         variableSeatsMountable.RemoveSeat(wearableSlot.ProvidesSeatId);
//   //         wearableSlot.ProvidesSeatId = (string) null;
//   //       }
//   //     }
//   //     else
//   //     {
//   //       SeatConfig seatConfig1 = itemSlot.Itemstack?.ItemAttributes?["attachableToEntity"]?["seatConfigBySlotCode"][wearableSlot.Code]?.AsObject<SeatConfig>() ?? itemSlot.Itemstack?.ItemAttributes?["attachableToEntity"]?["seatConfig"]?.AsObject<SeatConfig>();
//   //       if (seatConfig1 != null)
//   //       {
//   //         seatConfig1.SeatId = "attachableseat-" + slotId.ToString();
//   //         SeatConfig seatConfig2 = seatConfig1;
//   //         if (seatConfig2.APName == null)
//   //           seatConfig2.APName = wearableSlot.AttachmentPointCode;
//   //         SeatConfig seatConfig3 = seatConfig1;
//   //         if (seatConfig3.SelectionBox == null)
//   //           seatConfig3.SelectionBox = wearableSlot.AttachmentPointCode;
//   //         wearableSlot.SeatConfig = seatConfig1;
//   //         variableSeatsMountable.RegisterSeat(wearableSlot.SeatConfig);
//   //         wearableSlot.ProvidesSeatId = wearableSlot.SeatConfig.SeatId;
//   //       }
//   //       else if (wearableSlot.ProvidesSeatId != null)
//   //         variableSeatsMountable.RemoveSeat(wearableSlot.ProvidesSeatId);
//   //     }
//   //   }
//   // }
//
//   // public  bool TryGiveItemStack(ItemStack itemstack, ref EnumHandling handling)
//   // {
//   //   int num = 0;
//   //   DummySlot dummySlot = new DummySlot(itemstack);
//   //   foreach (ItemSlot itemSlot in (InventoryBase) this.inv)
//   //   {
//   //     if (this.GetSlotFromSelectionBoxIndex(num) != null && this.TryAttach((ItemSlot) dummySlot, num, (EntityAgent) null))
//   //     {
//   //       handling = EnumHandling.PreventDefault;
//   //       return true;
//   //     }
//   //     ++num;
//   //   }
//   //   return false;
//   //   // return base.TryGiveItemStack(itemstack, ref handling);
//   // }
//   //
//   // public void OnInteract(
//   //   EntityAgent byEntity,
//   //   ItemSlot itemslot,
//   //   Vec3d hitPosition,
//   //   EnumInteractMode mode,
//   //   ref EnumHandling handled)
//   // {
//   //   EntitySelection entitySelection = (byEntity as EntityPlayer).EntitySelection;
//   //   int num = entitySelection != null ? entitySelection.SelectionBoxIndex : -1;
//   //   if (num <= 0)
//   //     return;
//   //   int selectionBoxIndex = this.GetSlotIndexFromSelectionBoxIndex(num - 1);
//   //   ItemSlot itemslot1 = selectionBoxIndex >= 0 ? this.inv[selectionBoxIndex] : (ItemSlot) null;
//   //   if (itemslot1 == null)
//   //     return;
//   //   handled = EnumHandling.PreventSubsequent;
//   //   EntityControls entityControls = byEntity.MountedOn?.Controls ?? byEntity.Controls;
//   //   bool flag = this.UseShiftAttach ? entityControls.ShiftKey : entityControls.CtrlKey;
//   //   if (mode == EnumInteractMode.Interact && !entityControls.CtrlKey)
//   //   {
//   //     ItemStack itemstack = itemslot1.Itemstack;
//   //     // ISSUE: explicit non-virtual call
//   //     // if ((itemstack != null ? (__nonvirtual (itemstack.Collectible).Attributes?.IsTrue("interactPassthrough").GetValueOrDefault() ? 1 : 0) : 0) != 0)
//   //     // {
//   //     //   handled = EnumHandling.PassThrough;
//   //     //   return;
//   //     // }
//   //     if (itemslot1.Empty && this.wearableSlots[selectionBoxIndex].EmptyInteractPassThrough)
//   //     {
//   //       handled = EnumHandling.PassThrough;
//   //       return;
//   //     }
//   //     if (this.wearableSlots[selectionBoxIndex].SeatConfig != null)
//   //     {
//   //       handled = EnumHandling.PassThrough;
//   //       return;
//   //     }
//   //   }
//   //   // IAttachedInteractions collectibleInterface = itemslot1.Itemstack?.Collectible.GetCollectibleInterface<IAttachedInteractions>();
//   //   // if (collectibleInterface != null)
//   //   // {
//   //   //   EnumHandling handled1 = EnumHandling.PassThrough;
//   //   //   collectibleInterface.OnInteract(itemslot1, num - 1, this.entity, byEntity, hitPosition, mode, ref handled1, new Action(((EntityBehaviorContainer) this).storeInv));
//   //   //   if (handled1 == EnumHandling.PreventDefault || handled1 == EnumHandling.PreventSubsequent)
//   //   //     return;
//   //   // }
//   //   if (mode != EnumInteractMode.Interact || !flag)
//   //   {
//   //     handled = EnumHandling.PassThrough;
//   //   }
//   //   else
//   //   {
//   //     if (!itemslot.Empty)
//   //     {
//   //       if (this.TryAttach(itemslot, num - 1, byEntity))
//   //       {
//   //         this.onAttachmentToggled(byEntity, itemslot);
//   //         return;
//   //       }
//   //     }
//   //     else if (this.TryRemoveAttachment(byEntity, num - 1))
//   //     {
//   //       this.onAttachmentToggled(byEntity, itemslot);
//   //       return;
//   //     }
//   //     // base.OnInteract(byEntity, itemslot, hitPosition, mode, ref handled);
//   //   }
//   // }
//   //
//   // private void onAttachmentToggled(EntityAgent byEntity, ItemSlot itemslot)
//   // {
//   //   // this.Api.World.PlaySoundAt(itemslot.Itemstack?.Block?.Sounds.Place ?? GlobalConstants.DefaultBuildSound, this.entity, (byEntity as EntityPlayer).Player);
//   //   this.Blockentity.MarkDirty(true);
//   //   // this.Blockentity.Api.World.BlockAccessor.GetChunkAtBlockPos(this.Blockentity.Pos).MarkModified();
//   // }
//   //
//   // private bool TryRemoveAttachment(EntityAgent byEntity, int selectionBoxIndex)
//   // {
//   //   int selectionBoxIndex1 = this.GetSlotIndexFromSelectionBoxIndex(selectionBoxIndex);
//   //   ItemSlot itemslot = this.inv[selectionBoxIndex1];
//   //   if (itemslot.Empty)
//   //     return false;
//   //   // EntityBehaviorSeatable behavior1 = this.entity.GetBehavior<EntityBehaviorSeatable>();
//   //   // if (behavior1 != null)
//   //   // {
//   //   //   string apname = this.entity.GetBehavior<EntityBehaviorSelectionBoxes>().selectionBoxes[selectionBoxIndex].AttachPoint.Code;
//   //   //   if (((IEnumerable<IMountableSeat>) behavior1.Seats).FirstOrDefault<IMountableSeat>((System.Func<IMountableSeat, bool>) (seat => seat.Config.APName == apname || seat.Config.SelectionBox == apname))?.Passenger != null)
//   //   //   {
//   //   //     if (this.entity.World.Api is ICoreClientAPI api)
//   //   //       api.TriggerIngameError((object) this, "requiredisembark", Lang.Get("Passenger must disembark first before being able to remove this seat"));
//   //   //     return false;
//   //   //   }
//   //   // }
//   //   // IAttachedInteractions collectibleInterface = itemslot.Itemstack?.Collectible.GetCollectibleInterface<IAttachedInteractions>();
//   //   // if ((collectibleInterface != null ? (!collectibleInterface.OnTryDetach(itemslot, selectionBoxIndex1, this.entity) ? 1 : 0) : 0) != 0)
//   //   //   return false;
//   //   // EntityBehaviorOwnable behavior2 = this.entity.GetBehavior<EntityBehaviorOwnable>();
//   //   // if (behavior2 != null && !behavior2.IsOwner(byEntity))
//   //   // {
//   //   //   if (this.entity.World.Api is ICoreClientAPI api)
//   //   //     api.TriggerIngameError((object) this, "requiersownership", Lang.Get("mount-interact-requiresownership"));
//   //   //   return false;
//   //   // }
//   //   bool flag = itemslot.StackSize == 0;
//   //   if (!flag && !byEntity.TryGiveItemStack(itemslot.Itemstack))
//   //     return false;
//   //   // itemslot.Itemstack?.Collectible.GetCollectibleInterface<IAttachedListener>()?.OnDetached(itemslot, selectionBoxIndex1, this.entity, byEntity);
//   //   if (this.Api.Side == EnumAppSide.Server && !flag)
//   //   {
//   //     itemslot.Itemstack.StackSize = 1;
//   //     // this.Api.World.Logger.Audit("{0} removed from a {1} at {2}, slot {4}: {3}", (object) byEntity?.GetName(), (object) this.entity.Code.ToShortString(), (object) this.entity.Pos.AsBlockPos, (object) itemslot.Itemstack?.ToString(), (object) selectionBoxIndex1);
//   //   }
//   //   itemslot.Itemstack = (ItemStack) null;
//   //   // this.storeInv();
//   //   return true;
//   // }
//
//   // private bool TryAttach(ItemSlot itemslot, int selectionBoxIndex, EntityAgent byEntity)
//   // {
//   //   IAttachableToEntity attachableToEntity = IAttachableToEntity.FromCollectible(itemslot.Itemstack.Collectible);
//   //   if (attachableToEntity == null || !attachableToEntity.IsAttachable(this.entity, itemslot.Itemstack))
//   //     return false;
//   //   int selectionBoxIndex1 = this.GetSlotIndexFromSelectionBoxIndex(selectionBoxIndex);
//   //   ItemSlot itemSlot1 = this.inv[selectionBoxIndex1];
//   //   string categoryCode = attachableToEntity.GetCategoryCode(itemslot.Itemstack);
//   //   WearableSlotConfig slotConfig = this.wearableSlots[selectionBoxIndex1];
//   //   EntityBehaviorSeatable behavior1 = this.entity.GetBehavior<EntityBehaviorSeatable>();
//   //   if (behavior1 != null)
//   //   {
//   //     int index = behavior1.SeatConfigs.IndexOf<SeatConfig>((System.Func<SeatConfig, bool>) (x => x.SelectionBox == slotConfig.AttachmentPointCode));
//   //     if (index > -1 && behavior1.Seats[index].Passenger != null)
//   //     {
//   //       if (this.Api is ICoreClientAPI api)
//   //         api.TriggerIngameError((object) this, "alreadyoccupied", Lang.Get("mount-interact-alreadyoccupied"));
//   //       return false;
//   //     }
//   //   }
//   //   if (!slotConfig.CanHold(categoryCode) || !itemSlot1.Empty)
//   //     return false;
//   //   if (attachableToEntity.RequiresBehindSlots > 0)
//   //   {
//   //     if (slotConfig.BehindSlots.Length < attachableToEntity.RequiresBehindSlots)
//   //     {
//   //       if (this.entity.World.Api is ICoreClientAPI api)
//   //       {
//   //         string text = Lang.Get("mount-interact-requiresbehindslots", (object) attachableToEntity.RequiresBehindSlots);
//   //         api.TriggerIngameError((object) this, "notenoughspace", text);
//   //       }
//   //       return false;
//   //     }
//   //     int slotId = this.wearableSlots.IndexOf<WearableSlotConfig>((System.Func<WearableSlotConfig, bool>) (sc => sc.Code == slotConfig.BehindSlots[0]));
//   //     if (slotId >= 0 && !this.inv[slotId].Empty)
//   //     {
//   //       if (this.entity.World.Api is ICoreClientAPI api)
//   //       {
//   //         string text = Lang.Get("mount-interact-alreadyoccupiedbehind", (object) (attachableToEntity.RequiresBehindSlots + 1));
//   //         api.TriggerIngameError((object) this, "alreadyoccupied", text);
//   //       }
//   //       return false;
//   //     }
//   //   }
//   //   int slotId1 = this.wearableSlots.IndexOf<WearableSlotConfig>((System.Func<WearableSlotConfig, bool>) (sc =>
//   //   {
//   //     string[] behindSlots = sc.BehindSlots;
//   //     return behindSlots != null && behindSlots.Contains<string>(slotConfig.Code);
//   //   }));
//   //   if (slotId1 >= 0)
//   //   {
//   //     ItemSlot itemSlot2 = this.inv[slotId1];
//   //     if (!itemSlot2.Empty && IAttachableToEntity.FromCollectible(itemSlot2.Itemstack.Collectible).RequiresBehindSlots > 0)
//   //     {
//   //       if (this.entity.World.Api is ICoreClientAPI api)
//   //       {
//   //         string text = Lang.Get("mount-interact-alreadyoccupiedinfront", (object) attachableToEntity.RequiresBehindSlots);
//   //         api.TriggerIngameError((object) this, "alreadyoccupied", text);
//   //       }
//   //       return false;
//   //     }
//   //   }
//   //   EntityBehaviorOwnable behavior2 = this.entity.GetBehavior<EntityBehaviorOwnable>();
//   //   if (behavior2 != null && !behavior2.IsOwner(byEntity))
//   //   {
//   //     if (this.entity.World.Api is ICoreClientAPI api)
//   //       api.TriggerIngameError((object) this, "requiersownership", Lang.Get("mount-interact-requiresownership"));
//   //     return false;
//   //   }
//   //   EntityBehaviorSeatable behavior3 = this.entity.GetBehavior<EntityBehaviorSeatable>();
//   //   // ISSUE: explicit non-virtual call
//   //   if ((behavior3 != null ? ((IEnumerable<IMountableSeat>) __nonvirtual (behavior3.Seats)).FirstOrDefault<IMountableSeat>((System.Func<IMountableSeat, bool>) (s => s.Config.APName == slotConfig.AttachmentPointCode))?.Passenger : (Entity) null) != null)
//   //     return false;
//   //   IAttachedInteractions collectibleInterface1 = itemslot.Itemstack.Collectible.GetCollectibleInterface<IAttachedInteractions>();
//   //   if ((collectibleInterface1 != null ? (!collectibleInterface1.OnTryAttach(itemslot, selectionBoxIndex1, this.entity) ? 1 : 0) : 0) != 0)
//   //     return false;
//   //   IAttachedListener collectibleInterface2 = itemslot.Itemstack?.Collectible.GetCollectibleInterface<IAttachedListener>();
//   //   if (this.entity.World.Side != EnumAppSide.Server)
//   //     return true;
//   //   \u003C\u003Ey__InlineArray5<object> buffer = new \u003C\u003Ey__InlineArray5<object>();
//   //   // ISSUE: reference to a compiler-generated method
//   //   \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 0) = (object) byEntity?.GetName();
//   //   // ISSUE: reference to a compiler-generated method
//   //   \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 1) = (object) this.entity.Code.ToShortString();
//   //   // ISSUE: reference to a compiler-generated method
//   //   \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 2) = (object) this.entity.Pos.AsBlockPos;
//   //   // ISSUE: reference to a compiler-generated method
//   //   \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 3) = (object) itemslot.Itemstack.ToString();
//   //   // ISSUE: reference to a compiler-generated method
//   //   \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 4) = (object) selectionBoxIndex1;
//   //   // ISSUE: reference to a compiler-generated method
//   //   string message = string.Format("{0} attached to a {1} at {2}, slot {4}: {3}", \u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray5<object>, object>(in buffer, 5));
//   //   int num = itemslot.TryPutInto(this.entity.World, itemSlot1) > 0 ? 1 : 0;
//   //   if (num == 0)
//   //     return num != 0;
//   //   this.Api.World.Logger.Audit(message);
//   //   collectibleInterface2?.OnAttached(itemSlot1, selectionBoxIndex1, this.entity, byEntity);
//   //   this.storeInv();
//   //   return num != 0;
//   // }
//   //
//   // public ItemSlot GetSlotFromSelectionBoxIndex(int boxIndex)
//   // {
//   //   int selectionBoxIndex = this.GetSlotIndexFromSelectionBoxIndex(boxIndex);
//   //   return selectionBoxIndex == -1 ? (ItemSlot) null : this.inv[selectionBoxIndex];
//   // }
//   //
//   // public int GetSlotIndexFromSelectionBoxIndex(int boxIndex)
//   // {
//   //   if (boxIndex < 0)
//   //     return -1;
//   //   EntityBehaviorSelectionBoxes behavior = this.entity.GetBehavior<EntityBehaviorSelectionBoxes>();
//   //   ArgumentNullException.ThrowIfNull((object) behavior, "selectionBehavior");
//   //   string apCode = behavior.selectionBoxes[boxIndex].AttachPoint.Code;
//   //   return this.wearableSlots.IndexOf<WearableSlotConfig>((System.Func<WearableSlotConfig, bool>) (elem => elem.AttachmentPointCode == apCode));
//   // }
//   //
//   // public ItemSlot GetSlotConfigFromAPName(string apCode)
//   // {
//   //   AttachmentPointAndPose[] selectionBoxes = this.entity.GetBehavior<EntityBehaviorSelectionBoxes>().selectionBoxes;
//   //   int slotId = this.wearableSlots.IndexOf<WearableSlotConfig>((System.Func<WearableSlotConfig, bool>) (elem => elem.AttachmentPointCode == apCode));
//   //   return slotId < 0 ? (ItemSlot) null : this.inv[slotId];
//   // }
//   //
//   // protected override Shape addGearToShape(
//   //   Shape entityShape,
//   //   ItemSlot gearslot,
//   //   string slotCode,
//   //   string shapePathForLogging,
//   //   ref bool shapeIsCloned,
//   //   ref string[] willDeleteElements,
//   //   Dictionary<string, StepParentElementTo> overrideStepParent = null)
//   // {
//   //   int index = gearslot.Inventory.IndexOf<ItemSlot>((ActionBoolReturn<ItemSlot>) (slot => slot == gearslot));
//   //   overrideStepParent = this.wearableSlots[index].StepParentTo;
//   //   slotCode = this.wearableSlots[index].Code;
//   //   return base.addGearToShape(entityShape, gearslot, slotCode, shapePathForLogging, ref shapeIsCloned, ref willDeleteElements, overrideStepParent);
//   // }
//   //
//   // public override void OnEntityDespawn(EntityDespawnData despawn)
//   // {
//   //   int num = 0;
//   //   foreach (ItemSlot itemslot in (InventoryBase) this.inv)
//   //     itemslot.Itemstack?.Collectible.GetCollectibleInterface<IAttachedInteractions>()?.OnEntityDespawn(itemslot, num++, this.entity, despawn);
//   //   base.OnEntityDespawn(despawn);
//   // }
//   //
//   // public override void OnReceivedClientPacket(
//   //   IServerPlayer player,
//   //   int packetid,
//   //   byte[] data,
//   //   ref EnumHandling handled)
//   // {
//   //   int slotIndex = 0;
//   //   foreach (ItemSlot itemslot in (InventoryBase) this.inv)
//   //   {
//   //     itemslot.Itemstack?.Collectible.GetCollectibleInterface<IAttachedInteractions>()?.OnReceivedClientPacket(itemslot, slotIndex, this.entity, player, packetid, data, ref handled, new Action(((EntityBehaviorContainer) this).storeInv));
//   //     ++slotIndex;
//   //     if (handled == EnumHandling.PreventSubsequent)
//   //       break;
//   //   }
//   // }
//   //
//   // public override WorldInteraction[] GetInteractionHelp(
//   //   IClientWorldAccessor world,
//   //   EntitySelection es,
//   //   IClientPlayer player,
//   //   ref EnumHandling handled)
//   // {
//   //   return es.SelectionBoxIndex > 0 ? AttachableInteractionHelp.GetOrCreateInteractionHelp(world.Api, this, this.wearableSlots, es.SelectionBoxIndex - 1, this.GetSlotFromSelectionBoxIndex(es.SelectionBoxIndex - 1)) : base.GetInteractionHelp(world, es, player, ref handled);
//   // }
//   //
//   // public override string PropertyName() => "dressable";
//   //
//   // public void Dispose()
//   // {
//   // }
//   //
//   // public Vec3d GetInteractionHelpPosition()
//   // {
//   //   ICoreClientAPI api = this.entity.Api as ICoreClientAPI;
//   //   if (api.World.Player.CurrentEntitySelection == null)
//   //     return (Vec3d) null;
//   //   int selectionBoxIndex = api.World.Player.CurrentEntitySelection.SelectionBoxIndex - 1;
//   //   if (selectionBoxIndex < 0)
//   //     return (Vec3d) null;
//   //   return this.entity.GetBehavior<EntityBehaviorSelectionBoxes>().GetCenterPosOfBox(selectionBoxIndex)?.Add(0.0, 0.5, 0.0);
//   // }
//   //
//   // public override void OnEntityDeath(DamageSource damageSourceForDeath)
//   // {
//   //   int num = 0;
//   //   foreach (ItemSlot itemslot in (InventoryBase) this.inv)
//   //     itemslot.Itemstack?.Collectible.GetCollectibleInterface<IAttachedInteractions>()?.OnEntityDeath(itemslot, num++, this.entity, damageSourceForDeath);
//   //   base.OnEntityDeath(damageSourceForDeath);
//   // }
//   //
//   // public bool TransparentCenter => false;
// }
