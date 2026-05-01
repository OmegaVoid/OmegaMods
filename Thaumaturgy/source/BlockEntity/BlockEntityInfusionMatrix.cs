using Thaumaturgy.Renderer;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Thaumaturgy.BlockEntity;

public class BlockEntityInfusionMatrix : Vintagestory.API.Common.BlockEntity
{
    protected ICoreServerAPI? sapi;
    private BlockEntityAnimationUtil animUtil => GetBehavior<BEBehaviorAnimatable>().animUtil;

    public bool Active { get; set; } = true;
    public float Startup { get; set; }
    public float Instability = 0;
    public float Ticks = 0;

    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);
        RegisterGameTickListener(OnGameTick, 50);
        api.Logger.Event("Block InfusionMatrix Block Placed!");
        if (api.Side != EnumAppSide.Client)
            return;

        renderer = new InfusionMatrixRenderer((api as ICoreClientAPI)!, Pos, GenMesh());
        renderer.ShouldRender = true;
        renderer.ShouldRotateAutomated = true;

        (api as ICoreClientAPI)!.Event.RegisterRenderer(renderer, EnumRenderStage.Opaque, "infusion_matrix");

        InfusionMatrixMesh ??= GenMesh();
    }

    protected void OnRetesselated()
    {
        if (this.renderer == null)
            return;
        this.renderer.ShouldRender = true;
    }

    public void OnGameTick(float dt)
    {
        Ticks += dt;
        switch (Active)
        {
            case true when Math.Abs(Startup - 1f) > 0.00001f:
            {
                if (Startup < 1f)
                    Startup += MathF.Max(Startup / 10, 0.001f);
                if (Startup > 0.999) Startup = 1f;
                break;
            }
            case false when Startup > 0:
            {
                Startup -= Startup / 10;
                if (Startup < 0.001) Startup = 0f;
                break;
            }
        }

        if (renderer is not null)
        {
            renderer.Startup = Startup;
            renderer.Active = Active;
            renderer.Instability = Instability;
            renderer.Ticks = Ticks;
        }

        this.Api.World.BlockAccessor.MarkBlockDirty(this.Pos, new Action(this.OnRetesselated));
    }


    public void Activate()
    {
        if (Api == null) return;

        Active = true;
        animUtil?.StartAnimation(new AnimationMetaData()
            { Animation = "startup", Code = "startup", EaseInSpeed = 1, EaseOutSpeed = 2, AnimationSpeed = 1f });
        MarkDirty(true);
    }

    protected MeshData? InfusionMatrixMesh
    {
        get
        {
            Api.ObjectCache.TryGetValue("infusionmatrixmesh", out var quernBaseMesh);
            return (MeshData)quernBaseMesh!;
        }
        set => Api.ObjectCache["infusionmatrixmesh"] = value;
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
    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
    {
        if (Block == null)
            return false;
        for (var a = 0; a < 2; a++)
        for (var b = 0; b < 2; b++)
        for (var c = 0; c < 2; c++)
        {
            var b1 = 0.0f;
            var b2 = 0.0f;
            var b3 = 0.0f;
            if (Active)
            {
                b1 = MathF.Sin((Ticks * 200 + a * 10 * 16) / (15 - Instability / 2)) * 0.01f * Startup * Instability;
                b2 = MathF.Sin((Ticks * 200 + a * 10 * 16) / (14 - Instability / 2)) * 0.01f * Startup * Instability;
                b3 = MathF.Sin((Ticks * 200 + a * 10 * 16) / (13 - Instability / 2)) * 0.01f * Startup * Instability;
            }

            var aa = a == 0 ? -1 : 1;
            var bb = b == 0 ? -1 : 1;
            var cc = c == 0 ? -1 : 1;
            var obj = InfusionMatrixMesh?.Clone()
                .Scale(0.45f, 0.45f, 0.45f)
                .Rotate(a == 1 ? MathF.PI / 2 : 0, b == 1 ? MathF.PI / 2 : 0, c == 1 ? MathF.PI / 2 : 0)
                .Translate(b1 + aa * 0.25f, b2 + bb * 0.25f, b3 + cc * 0.25f)
                .Rotate(35 * MathF.PI / 180f, Ticks % 260 * Startup, 45 * MathF.PI / 180f);
            mesher.AddMeshData(obj);
        }

        // mesher.AddMeshData(this.infusionMatrixMesh);
        // if (this.quantityPlayersGrinding == 0 && !this.automated)
        // mesher.AddMeshData(this.quernTopMesh.Clone().Rotate(0.0f, this.renderer.AngleRad, 0.0f).Translate(0.0f, 11f / 16f, 0.0f));
        return true;
    }

    protected InfusionMatrixRenderer? renderer;


    protected MeshData? GenMesh(string type = "base")
    {
        var block = Api.World.BlockAccessor.GetBlock(Pos);
        if (block.BlockId == 0)
            return null;
        ((ICoreClientAPI)Api).Tesselator.TesselateShape(block,
            Shape.TryGet(Api, $"thaumaturgy:shapes/block/matrix3.json"), out var modeldata);
        return modeldata;
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