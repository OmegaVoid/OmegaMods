using AttributeRenderingLibrary;
using HarmonyLib;
using Thaumaturgy.BlockBehaviour;
using Thaumaturgy.Renderer;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace Thaumaturgy.BlockEntity;

public class BlockEntityInfusionMatrix : Vintagestory.API.Common.BlockEntity, IPointOfInterest
{
    
    
    private BlockEntityAnimationUtil animUtil => GetBehavior<BEBehaviorAnimatable>().animUtil;

    public bool Active { get; set; } = true;
    public float Startup { get; set; }
    public float Instability => Symmetry;
    public float Symmetry { get; set; }
    public float Ticks { get; set; }
    public bool Crafting { get; set; } = false;
    public int CraftCount { get; set; } = 1;

    public List<BlockEntityPedestal> Pedestals = [];
    public List<Block> Stuff = [];

    public InfusionMatrixRenderer? renderer;

    public MeshData? ModelData;
    public MultiTextureMeshRef? MeshRef;

    public ICoreClientAPI? CApi => Api as ICoreClientAPI;
    public ICoreServerAPI? SApi => Api as ICoreServerAPI;

    protected MeshData? InfusionMatrixMesh
    {
        get
        {
            Api.ObjectCache.TryGetValue("infusionmatrixmesh", out var quernBaseMesh);
            return (MeshData)quernBaseMesh!;
        }
        set => Api.ObjectCache["infusionmatrixmesh"] = value;
    }


    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);
        RegisterGameTickListener(OnGameTick, 1000 / 20);
        api.Logger.Event("Block InfusionMatrix Block Placed!");
        
        if (api is ICoreServerAPI) api.ModLoader.GetModSystem<POIRegistry>().AddPOI(this);
        if (api.Side != EnumAppSide.Client)
            return;

        renderer = new InfusionMatrixRenderer((api as ICoreClientAPI)!, this);

        renderer.Initialize();
        CApi!.Event.RegisterRenderer(renderer, EnumRenderStage.Opaque, "infusion_matrix.opaque");
        CApi!.Event.RegisterRenderer(renderer, EnumRenderStage.ShadowFar, "infusion_matrix.ShadowFar");
        CApi!.Event.RegisterRenderer(renderer, EnumRenderStage.ShadowNear, "infusion_matrix.ShadowNear");
        // CApi!.Event.RegisterRenderer(renderer, EnumRenderStage.OIT, "infusion_matrix.oit");

        GenMesh();
    }

    public void OnGameTick(float dt)
    {
        Ticks += dt;
        switch (Active)
        {
            case true when Math.Abs(Startup - 1f) > 0.00001f:
                if (Startup < 1f)
                    Startup += MathF.Max(Startup / 10, 0.001f);
                if (Startup > 0.999) Startup = 1f;
                break;
            case false when Startup > 0:
                Startup -= Startup / 10;
                if (Startup < 0.001) Startup = 0f;
                break; 
        }
        CalculateSymmetry();

        // this.Api.World.BlockAccessor.MarkBlockDirty(this.Pos, new Action(this.OnRetesselated));
    }
    
    public bool OnInteract(IPlayer byPlayer, BlockSelection blockSel)
    {
        Api.World.Logger.Audit("{0} Started Infusion at {1}.", byPlayer.PlayerName, Pos);
        return true;
    }


    
    public void Activate()
    {
        if (Api == null) return;

        Active = true;
        animUtil?.StartAnimation(new AnimationMetaData()
            { Animation = "startup", Code = "startup", EaseInSpeed = 1, EaseOutSpeed = 2, AnimationSpeed = 1f });
        MarkDirty(true);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
    {
        return true;
    }


    public static bool IsBlockAffecting(Block? block, out float? amount)
    {
        amount = 0;
        if (!IsBlockAffecting(block))
            return false;
        if (block == null)
            return false;
        var stabiliser = block.GetBehavior<BlockBehaviorInfusionStabiliser>();
        amount = stabiliser.Stability;
        return true;
    }
    public static bool IsBlockAffecting(Block? block) => block != null && !block.HasBlockBehavior<BlockBehaviorInfusionStabiliser>();

    public static Cuboidi BaseSymmetryArea => new(new Vec3i(-12, -10, -12), new Vec3i(12, 5, 12));
    public static Cuboidi BasePedestalArea => new(new Vec3i(-8, -10, -8), new Vec3i(8, 0, 8));
    public Cuboidi SymmetryArea => BaseSymmetryArea.Clone().Translate(Pos);
    public Cuboidi PedestalArea => BasePedestalArea.Clone().Translate(Pos);
    public BlockPos CenterPedestalPos => Pos.DownCopy(-2);
    
    public BlockEntityPedestal? CenterPedestal { get; private set; }
    
    
    
    
    public void CalculateSymmetry()
    {
        var area = SymmetryArea;
        var acc = Api.World.GetBlockAccessorPrefetch(false, false);
        acc.PrefetchBlocks(area.Start.AsBlockPos, area.End.AsBlockPos);
        CenterPedestal = acc.GetBlockEntity<BlockEntityPedestal>(CenterPedestalPos);
        var prevSym = Symmetry;
        var stuff = new List<BlockPos>();
        Pedestals.Clear();
        for (var xx = area.MinX; xx <= area.MaxX; ++xx)
        for (var zz = area.MinZ; zz <= area.MaxZ; ++zz)
        for (var yy = area.MinY; yy <= area.MaxY; ++yy)
        {
            if (xx == area.CenterX && zz == area.CenterZ) continue;
            var pos = new BlockPos(xx, yy, zz);
            if (!PedestalArea.Contains(xx,yy, zz)) continue;

            if (acc.GetBlockEntity<BlockEntityPedestal>(pos) is { } be)
            {
                Pedestals.Add(be);
                break;
            }
            
            if (IsBlockAffecting(acc.GetBlock(pos))) stuff.Add(pos);
        }

        Symmetry = 0;
        SymmetryPedestalChecker(acc);
        SymmetryBlockChecker(stuff, acc);

        if (Math.Abs(prevSym - Symmetry) > 0.001f)
        {
            SApi?.BroadcastMessageToAllGroups($"Instability {Instability}", EnumChatType.Notification);
        }
    }

    private void SymmetryBlockChecker(List<BlockPos> stuff, IBlockAccessorPrefetch acc)
    {
        var sym = 0f;
        foreach (var blockPos in stuff)
        {
            var opposite = Pos + (Pos - blockPos);
            var block = acc.GetBlock(blockPos);
            if (IsBlockAffecting(block))
            {
                var stabiliser = block.GetBehavior<BlockBehaviorInfusionStabiliser>();
                sym += 0.1f * (stabiliser?.Stability ?? 0);
            }
            var oppositeBlock = acc.GetBlock(opposite);
            if (!IsBlockAffecting(oppositeBlock)) continue;
            {
                var stabiliser = oppositeBlock.GetBehavior<BlockBehaviorInfusionStabiliser>();
                sym -= 0.2f * (stabiliser?.Stability ?? 0);
            }

        }

        Symmetry += sym;
    }

    private void SymmetryPedestalChecker(IBlockAccessorPrefetch acc)
    {
        foreach (var pedestal in Pedestals)
        {
            var opposite = Pos + (Pos - pedestal.Pos);
            opposite.Y = pedestal.Pos.Y;
            var items = false;
            Symmetry += 2;
            if (!pedestal.Slot.Empty)
            {
                ++Symmetry;
                items = true;
            }

            var oppositeBlock = acc.GetBlockEntity<BlockEntityPedestal>(opposite);
            if (oppositeBlock is null) continue;
            Symmetry -= 2;
            if (!oppositeBlock.Slot.Empty && items) --Symmetry;
        }
    }


    public void GenMesh( /*IEnumerable<Vec3d> points*/)
    {
        var block = Api.World.BlockAccessor.GetBlock(Pos);
        if (block.BlockId == 0)
            return;
        if (CApi is null)
            return;

        if (ModelData == null)
        {
            CApi.Tesselator.TesselateShape(block,
                Shape.TryGet(Api, $"thaumaturgy:shapes/block/matrix3.json"), out var modeldata);
            // modeldata.CustomFloats = GenInstanceData([]);
            ModelData = modeldata;
            MeshRef = CApi.Render.UploadMultiTextureMesh(modeldata);
        }

        // var modelData = ModelData.Clone();

        // modelData.CustomFloats = GenInstanceData(points);

        // CApi.Render.UpdateMesh(MeshRef, modelData);
    }


    public void Deactivate()
    {
        animUtil?.StopAnimation("end");
        Active = false;
        MarkDirty(true);
    }

    public override void OnBlockUnloaded()
    {
        base.OnBlockUnloaded();
        this.renderer?.Dispose();
    }

    public override void OnBlockRemoved()
    {
        base.OnBlockRemoved();
        this.renderer?.Dispose();
        this.renderer = null;
    }

    public Vec3d Position => Pos.ToVec3d().Add(0.5, 0.5, 0.5);
    public string Type => "infusion-matrix";
}