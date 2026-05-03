using Thaumaturgy.Renderer;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace Thaumaturgy.BlockEntity;

public class BlockEntityInfusionMatrix : Vintagestory.API.Common.BlockEntity
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
        if (api.Side != EnumAppSide.Client)
            return;

        renderer = new InfusionMatrixRenderer((api as ICoreClientAPI)!, this);

        renderer.Initialize();

        CApi!.Event.RegisterRenderer(renderer, EnumRenderStage.Opaque, "infusion_matrix");

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

    //
    // protected MeshData quernTopMesh
    // {
    //     get
    //     {
    //         object quernTopMesh;
    //         this.Api.ObjectCache.TryGetValue("querntopmesh-" + this.Material, out quernTopMesh);
    //         return (MeshData) quernTopMesh;
    //     }
    //     set => this.Api.ObjectCache["querntopmesh-" + this.Material] = (object) value;
    // }
    // public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
    // {
    //     if (Block == null)
    //         return false;
    //
    //     for (var a = 0; a < 2; a++)
    //     for (var b = 0; b < 2; b++)
    //     for (var c = 0; c < 2; c++)
    //     {
    //         var b1 = 0.0f;
    //         var b2 = 0.0f;
    //         var b3 = 0.0f;
    //         if (Active)
    //         {
    //             b1 = MathF.Sin((Ticks * 200 + a * 10 * 16) / (15 - Instability / 2)) * 0.01f * Startup * Instability;
    //             b2 = MathF.Sin((Ticks * 200 + a * 10 * 16) / (14 - Instability / 2)) * 0.01f * Startup * Instability;
    //             b3 = MathF.Sin((Ticks * 200 + a * 10 * 16) / (13 - Instability / 2)) * 0.01f * Startup * Instability;
    //         }
    //
    //         var aa = a == 0 ? -1 : 1;
    //         var bb = b == 0 ? -1 : 1;
    //         var cc = c == 0 ? -1 : 1;
    //         var obj = InfusionMatrixMesh?.Clone()
    //             .Scale(0.45f, 0.45f, 0.45f)
    //             .Rotate(a == 1 ? MathF.PI / 2 : 0, b == 1 ? MathF.PI / 2 : 0, c == 1 ? MathF.PI / 2 : 0)
    //             .Translate(b1 + aa * 0.25f, b2 + bb * 0.25f, b3 + cc * 0.25f)
    //             .Rotate(35 * MathF.PI / 180f, Ticks % 360 * Startup, 45 * MathF.PI / 180f);
    //         mesher.AddMeshData(obj);
    //     }
    //
    //     // mesher.AddMeshData(this.infusionMatrixMesh);
    //     // if (this.quantityPlayersGrinding == 0 && !this.automated)
    //     // mesher.AddMeshData(this.quernTopMesh.Clone().Rotate(0.0f, this.renderer.AngleRad, 0.0f).Translate(0.0f, 11f / 16f, 0.0f));
    //     return true;
    // }


    public static bool IsBlockAffecting(Block? block)
    {
        if (block == null)
            return false;

        return true;
    }

    public void CalculateSymmetry()
    {
        var prevSym = Symmetry;
        var stuff = new List<BlockPos>();
        Pedestals.Clear();
        for (var xx = -12; xx <= 12; ++xx)
        for (var zz = -12; zz <= 12; ++zz)
        for (var yy = -5; yy <= 10; ++yy)
        {
            if (xx == 0 && zz == 0) continue;

            var pos = Pos + new BlockPos(xx, -yy, zz);

            if (yy <= 0 || Math.Abs(xx) > 8 || Math.Abs(zz) > 8) continue;

            var be = Api.World.BlockAccessor.GetBlockEntity<BlockEntityPedestal>(pos);

            if (be is not null)
            {
                Pedestals.Add(be);
                break;
            }

            var block = Api.World.BlockAccessor.GetBlock(pos);

            if (IsBlockAffecting(block)) stuff.Add(pos);
        }

        Symmetry = 0;
        foreach (var pedestal in Pedestals)
        {
            var opposite = Pos + (Pos - pedestal.Pos);
            var items = false;
            Symmetry += 2;
            if (!pedestal.Slot.Empty)
            {
                ++Symmetry;
                items = true;
            }

            var oppositeBlock = Api.World.BlockAccessor.GetBlockEntity<BlockEntityPedestal>(opposite);
            if (oppositeBlock is null) continue;
            Symmetry -= 2;
            if (!pedestal.Slot.Empty && items) --Symmetry;
        }

        var sym = 0f;

        foreach (var blockPos in stuff)
        {
            var opposite = Pos + (Pos - blockPos);
            if (IsBlockAffecting(Api.World.BlockAccessor.GetBlock(blockPos)))
                sym += 0.1f;
            if (IsBlockAffecting(Api.World.BlockAccessor.GetBlock(opposite)))
                sym -= 0.2f;
        }

        Symmetry += sym;

        if (Math.Abs(prevSym - Symmetry) > 0.001f)
        {
            Api.Logger.Chat("Instability {0}", Instability);
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
}