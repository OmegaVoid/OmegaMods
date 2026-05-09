using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Thaumaturgy.BlockEntity;

public class BlockEntityPedestal : BlockEntityDisplay, IPointOfInterest
{
    public float Ticks { get; set; }

    public ICoreClientAPI? CApi => Api as ICoreClientAPI;
    public ICoreServerAPI? SApi => Api as ICoreServerAPI;

    public InventoryGeneric InventoryGeneric;
    public ItemSlot Slot => InventoryGeneric[0];

    public override InventoryBase Inventory => InventoryGeneric;
    public override string InventoryClassName => "pedestal";

    public BlockEntityPedestal()
    {
        InventoryGeneric = new InventoryDisplayed(this, 1, "pedestal-0", Api);
    }

    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);

        RegisterGameTickListener(OnGameTick, 1000 / 20);

        // RegisterGameTickListener(OnGameTick, 50);
        api.Logger.Event("Block Pedestal Block Placed!");
        if (api is ICoreServerAPI) api.ModLoader.GetModSystem<POIRegistry>().AddPOI(this);

        if (api.Side != EnumAppSide.Client)
            return;
    }

    private void OnGameTick(float dt)
    {
        Ticks += dt*20;
        // updateMeshes();
        // MarkDirty();
        // MarkMeshesDirty();
    }

    protected override float[][] genTransformationMatrices()
    {
        var h = MathF.Sin(Ticks % 32767.0F / 16.0F) * 0.05F;
        return
        [
            new Matrixf()
                .Scale(0.75f, 0.75f, 0.75f)
                // .RotateDeg(new Vec3f(Ticks % 360, Ticks % 360, Ticks % 360))
                .Translate(0, 1.15f + h, 0)
                .Values
        ];
    }


    public bool OnInteract(IPlayer byPlayer, BlockSelection blockSel)
    {
        var activeHotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;
        if (activeHotbarSlot.Empty)
            return TryTake(byPlayer, blockSel);
        var place = activeHotbarSlot.Itemstack?.Block?.Sounds?.Place;
        if (!TryPut(activeHotbarSlot, blockSel, byPlayer))
            return false;
        Api.World.PlaySoundAt(place ?? GlobalConstants.DefaultBuildSound, byPlayer.Entity, byPlayer);
        Api.World.Logger.Audit("{0} Put 1x{1} into Pedestal at {2}.", byPlayer.PlayerName,
            InventoryGeneric[0].Itemstack?.Collectible.Code, Pos);
        return true;
    }

    private bool TryPut(ItemSlot slot, BlockSelection blockSel, IPlayer player)
    {
        if (!Slot.Empty)
            return false;
        var num1 = slot.TryPutInto(Api.World, Slot);
        if (num1 > 0) MarkDirty(true);

        return num1 > 0;
    }

    private bool TryTake(IPlayer byPlayer, BlockSelection blockSel)
    {
        if (Slot.Empty)
            return false;
        var itemstack = Slot.TakeOut(1);
        if (byPlayer.InventoryManager.TryGiveItemstack(itemstack))
        {
            Api.World.PlaySoundAt(itemstack.Block?.Sounds?.Place ?? GlobalConstants.DefaultBuildSound,
                byPlayer.Entity, byPlayer);
            Api.World.Logger.Audit("{0} Took 1x{1} from DisplayCase at {2}.",
                byPlayer.PlayerName, itemstack.Collectible.Code, Pos);
        }

        if (itemstack.StackSize > 0)
            Api.World.SpawnItemEntity(itemstack, Pos);
        updateMesh(0);
        MarkDirty(true);
        return true;
    }

    public bool PopOut()
    {
        if (Slot.Empty)
            return false;
        var itemstack = Slot.TakeOut(1);
        Api.World.SpawnItemEntity(itemstack, Pos);
        updateMesh(0);
        MarkDirty(true);
        return true;
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder sb)
    {
        base.GetBlockInfo(forPlayer, sb);
        sb.AppendLine();
        if (forPlayer.CurrentBlockSelection == null)
            return;
        if (Slot.Empty)
            return;
        sb.AppendLine(Slot.Itemstack.GetName());
    }
    
    public void OnTransformed(
        IWorldAccessor worldAccessor,
        ITreeAttribute tree,
        int degreeRotation,
        Dictionary<int, AssetLocation> oldBlockIdMapping,
        Dictionary<int, AssetLocation> oldItemIdMapping,
        EnumAxis? flipAxis)
    {
        
        var treeAttribute = tree.GetTreeAttribute("inventory");
        InventoryGeneric.FromTreeAttributes(treeAttribute);
        var itemSlotArray = new ItemSlot[1];
        itemSlotArray[0] = InventoryGeneric[0];

        for (var index1 = 0; index1 < 4; ++index1)
        {
            InventoryGeneric[0] = itemSlotArray[0]; 
        }
        InventoryGeneric.ToTreeAttributes(treeAttribute);
        tree["inventory"] = treeAttribute;
    }

    public Vec3d Position =>  Pos.ToVec3d().Add(0.5, 0.5, 0.5);
    public string Type => "infusion-pedestal";
}