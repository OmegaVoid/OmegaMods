using HarmonyLib;
using OpenTK.Graphics.Egl;
using OpenTK.Graphics.OpenGL4;
using Thaumaturgy.BlockEntity;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace Thaumaturgy.Renderer;

public class InfusionMatrixRenderer : IRenderer, IDisposable
{
    public Matrixf ModelMat = new();

    public LightiningPointLight CraftingLight;
    public bool CraftingLightAdded = false;
    public bool Active => _blockEntityInfusionMatrix.Active;
    public float Startup => _blockEntityInfusionMatrix.Startup;
    public float Instability => _blockEntityInfusionMatrix.Instability;
    public float Ticks => _blockEntityInfusionMatrix.Ticks;
    public float RenderTicks { get; set; } = 0;
    public bool Crafting => _blockEntityInfusionMatrix.Crafting;
    public int CraftCount => _blockEntityInfusionMatrix.CraftCount;

    private MultiTextureMeshRef? MeshRef => _blockEntityInfusionMatrix.MeshRef;
    private BlockPos Pos => _blockEntityInfusionMatrix.Pos;

    private TextureAtlasPosition _texPos;
    private LoadedTexture _tex;

    public double RenderOrder => 0.37;

    public int RenderRange => 24;

    public MeshData InfusionMatrixMesh
    {
        get
        {
            _coreClientApi.ObjectCache.TryGetValue("infusionmatrixmesh", out var quernBaseMesh);
            return (MeshData)quernBaseMesh!;
        }
    }

    public void Initialize()
    {
        CraftingLight = new LightiningPointLight(new Vec3f(0.8f, 0f, 1f), _blockEntityInfusionMatrix.Pos.ToVec3d());
    }

    public float Scale = 1;
    private readonly ICoreClientAPI _coreClientApi;
    private readonly BlockEntityInfusionMatrix _blockEntityInfusionMatrix;

    public InfusionMatrixRenderer(ICoreClientAPI coreClientApi, BlockEntityInfusionMatrix blockEntityInfusionMatrix)
    {
        _coreClientApi = coreClientApi;
        _blockEntityInfusionMatrix = blockEntityInfusionMatrix;
        
        _tex = new LoadedTexture(coreClientApi);

        var loc = new AssetLocation("thaumaturgy", "block/infuser"); 
        coreClientApi.Render.GetOrLoadTexture(loc, ref _tex);
        
    }


    public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
    {
        if (MeshRef == null)
            return;


        RenderTicks += deltaTime;


        var render = _coreClientApi.Render;
        var cameraPos = _coreClientApi.World.Player.Entity.CameraPos;

        if (!CraftingLightAdded)
        {
            render.AddPointLight(CraftingLight);
            CraftingLightAdded = true;
        }
        
        
        render.GlDisableCullFace();
        render.GlToggleBlend(true);
        var prog = render.PreparedStandardShader(Pos.X, Pos.Y, Pos.Z);
        prog.Use();
        prog.ViewMatrix = render.CameraMatrixOriginf;
        prog.ProjectionMatrix = render.CurrentProjectionMatrix;


        var baseMat = ModelMat.Identity()
                .Translate(Pos.X - cameraPos.X, Pos.Y - cameraPos.Y, Pos.Z - cameraPos.Z)
            // .Translate(1,1,1)
            // .RotateDeg(new Vec3f(35f * Startup, 20*RenderTicks % 360 * Startup, 45f * Startup))
            // .RotateDeg(new Vec3f(0, 20*RenderTicks % 360, 0))
            ;


        if (Active)
        {
            for (var a = 0; a < 2; a++)
            for (var b = 0; b < 2; b++)
            for (var c = 0; c < 2; c++)
                RenderSubCube(a, b, c, baseMat, prog, render);
        }
        // standardShaderProgram.ModelMatrix = ModelMat.Identity()
        //     .Translate(pos.X - cameraPos.X, pos.Y - cameraPos.Y,
        //         pos.Z - cameraPos.Z).Translate(0.5f, 11f / 16f, 0.5f).RotateY(Startup)
        //     .Translate(-0.5f, 0.0f, -0.5f).Values;
        // prog.ModelMatrix = ModelMat.Identity().Translate(pos.X - cameraPos.X,pos.Y - cameraPos.Y, pos.Z - cameraPos.Z).RotateDeg(new Vec3f(35f*Startup,Ticks%360*Startup,45f*Startup)).Values;
        // render.AddPointLight(CraftingLight);


        prog.ViewMatrix = render.CameraMatrixOriginf;
        prog.ProjectionMatrix = render.CurrentProjectionMatrix;


        // RenderHalo(prog, render, baseMat);


        // prog.ExtraGodray = 1;
        // prog.NormalShaded = 1;
        // render.RenderMesh(_meshref);
        prog.Stop();
        // if (ShouldRotateManual)
        //     Startup += (float)(deltaTime * 40.0 * (Math.PI / 180.0));
        // if (!ShouldRotateAutomated)
        // return;
    }

    private void RenderSubCube(int a, int b, int c, Matrixf baseMat, IStandardShaderProgram prog, IRenderAPI render)
    {
        var instability = Math.Min(6, 1 + Instability * 0.66f * (Math.Min(CraftCount, 50) / 50f));
        instability = 0;
        var b1 = 0.0f;
        var b2 = 0.0f;
        var b3 = 0.0f;
        if (Active)
        {
            b1 = (MathF.Sin((RenderTicks * 200 + a * 10 * 16) / (15 - instability / 2)) +
                  (Random.Shared.NextSingle() * 2 - 1) * 2) * 0.01f * Startup * instability;
            b2 = (MathF.Sin((RenderTicks * 200 + a * 10 * 16) / (14 - instability / 2)) +
                  (Random.Shared.NextSingle() * 2 - 1) * 2) * 0.01f * Startup * instability;
            b3 = (MathF.Sin((RenderTicks * 200 + a * 10 * 16) / (13 - instability / 2)) +
                  (Random.Shared.NextSingle() * 2 - 1) * 2) * 0.01f * Startup * instability;
        }

        var aa = a == 0 ? -1 : 1;
        var bb = b == 0 ? -1 : 1;
        var cc = c == 0 ? -1 : 1;
        var mat = baseMat.Clone().Identity();

        // prog.RgbaTint = new Vec4f(a, b, c, 1);


        var pivotMatrix = new Matrixf().Translate(0.5, 0.5, 0.5);
        var inversePivotMatrix = pivotMatrix.Clone();
        inversePivotMatrix.Invert();


        mat.Scale(0.4f, 0.4f, 0.4f);
        mat.Translate((b1 + aa * 0.25f) / 0.4f, (b2 + bb * 0.25f) / 0.4f, (b3 + cc * 0.25f) / 0.4f);


        var localRot = new Matrixf().RotateDeg(new Vec3f(a > 0 ? 90 : 0, b > 0 ? 90 : 0, c > 0 ? 90 : 0));
        var globalRot =
            new Matrixf().RotateDeg(new Vec3f(35 * Startup, 20 * RenderTicks % 360 * Startup, 45 * Startup));
        mat = pivotMatrix.Clone().Mul(mat);
        mat.Mul(localRot);
        mat.Mul(inversePivotMatrix);

        globalRot = pivotMatrix.Clone().Mul(globalRot).Mul(inversePivotMatrix);

        mat.ReverseMul(globalRot.Values);


        mat.ReverseMul(baseMat.Values);


        prog.ModelMatrix = mat
            .Values;
        prog.ExtraGlow = 0;
        render.RenderMultiTextureMesh(MeshRef, "tex", _tex.TextureId);
        
        if (!Active) return;
        prog.ExtraGlow = 1;
        prog.RgbaGlowIn = new Vec4f(0.8f, 0.1f, 1f,
            (MathF.Sin((RenderTicks + a * 2 + b * 3 + c * 4) / 4f) * .1f + .2f) * Startup);
        
        // render.RenderMultiTextureMesh(MeshRef, "tex2dOverlay", _tex.TextureId);
    }

    private void RenderHalo(IStandardShaderProgram prog, IRenderAPI render, Matrixf baseMat)
    {
        var mat = baseMat.Clone();
        prog.AlphaTest = 1f;
        render.GLDepthMask(false);
        render.GlToggleBlend(true, EnumBlendMode.PremultipliedAlpha);
        render.GlEnableCullFace();
        var rng = new Random(255);
        var f1 = CraftCount / 500f;
        var f3 = .9f;
        var f2 = 0f;

        for (var i = 0; i < 20; i++)
        {
            mat.RotateDeg(new Vec3f(rng.NextSingle() * 360, rng.NextSingle() * 360, rng.NextSingle() * 360));
            mat.RotateDeg(new Vec3f(rng.NextSingle() * 360, rng.NextSingle() * 360, rng.NextSingle() * 360 + f1 * 360));
        }
    }

    public void Dispose()
    {
        _coreClientApi.Event.UnregisterRenderer(this, EnumRenderStage.Opaque);

        if (CraftingLightAdded)
        {
            // _coreClientApi.Render.RemovePointLight(CraftingLight);
            CraftingLightAdded = false;
        }

        MeshRef?.Dispose();
    }
}