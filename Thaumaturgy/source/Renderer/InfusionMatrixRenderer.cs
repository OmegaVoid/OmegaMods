using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Thaumaturgy.Renderer;

public class InfusionMatrixRenderer(ICoreClientAPI coreClientApi, BlockPos pos, MeshData? mesh) : IRenderer, IDisposable
{
    internal bool ShouldRender;
    internal bool ShouldRotateManual;
    internal bool ShouldRotateAutomated;
    private MeshRef? _meshref = coreClientApi.Render.UploadMesh(mesh);
    public Matrixf ModelMat = new();
    
    
    public bool Active { get; set; } = true;
    public float Startup { get; set; }
    public float Instability = 6;
    public float Ticks = 0;

    public double RenderOrder => 0.5;

    public int RenderRange => 24;
    
    protected MeshData InfusionMatrixMesh
    {
        get
        {
            coreClientApi.ObjectCache.TryGetValue("infusionmatrixmesh", out var quernBaseMesh);
            return (MeshData) quernBaseMesh!;
        }
    }

    public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
    {
        if (_meshref == null || !ShouldRender)
            return;
        
        
        
        var render = coreClientApi.Render;
        var cameraPos = coreClientApi.World.Player.Entity.CameraPos;
        
        
        render.GlDisableCullFace();
        render.GlToggleBlend(true, EnumBlendMode.Glow);
        var standardShaderProgram =
            render.PreparedStandardShader(pos.X, pos.Y, pos.Z);
        standardShaderProgram.Tex2D = coreClientApi.BlockTextureAtlas.AtlasTextures[0].TextureId;
        
        render.GlPushMatrix();
        render.GlTranslate(pos.X - cameraPos.X, pos.Y - cameraPos.Y,
            pos.Z - cameraPos.Z);
        render.GlRotate(Ticks%360*Startup, 0, 1f, 0);
        render.GlRotate(35f*Startup, 1f, 0, 0);
        render.GlRotate(45f*Startup, 0, 0, 1f);

        if (Active)
        {
            render.GlPushMatrix();
            for (var a = 0; a < 2; a++)
            for (var b = 0; b < 2; b++)
            for (var c = 0; c < 2; c++)
            {
                var b1 = 0.0f;
                var b2 = 0.0f;
                var b3 = 0.0f;
                if (Active)
                {
                    b1 = (MathF.Sin((Ticks*200 + a * 10*16) / (15 - Instability / 2))+(Random.Shared.NextSingle()*2-1)*2) * 0.01f * Startup * Instability;
                    b2 = (MathF.Sin((Ticks*200 + a * 10*16) / (14 - Instability / 2))+(Random.Shared.NextSingle()*2-1)*2) * 0.01f * Startup * Instability;
                    b3 = (MathF.Sin((Ticks*200 + a * 10*16) / (13 - Instability / 2))+(Random.Shared.NextSingle()*2-1)*2) * 0.01f * Startup * Instability;
                }
                var aa = a == 0 ? -1 : 1;
                var bb = b == 0 ? -1 : 1;
                var cc = c == 0 ? -1 : 1;
                render.GlPushMatrix();
                render.GlTranslate(b1+aa*0.25f,b2+bb*0.25f,b3+cc*0.25f);
                if (a>0) render.GlRotate(90f,a,0,0);
                if (b>0) render.GlRotate(90f,0,b,0);
                if (c>0) render.GlRotate(90f,0,0,c);
                render.GlScale(0.45f,0.45f,0.45f);
                render.RenderMesh(_meshref);
                render.GlPopMatrix();
            
            }
            render.GlPopMatrix();
        }
        render.GlPopMatrix();
        
        
        
        // standardShaderProgram.ModelMatrix = ModelMat.Identity()
        //     .Translate(pos.X - cameraPos.X, pos.Y - cameraPos.Y,
        //         pos.Z - cameraPos.Z).Translate(0.5f, 11f / 16f, 0.5f).RotateY(Startup)
        //     .Translate(-0.5f, 0.0f, -0.5f).Values;
        standardShaderProgram.ViewMatrix = render.CameraMatrixOriginf;
        standardShaderProgram.ProjectionMatrix = render.CurrentProjectionMatrix;
        standardShaderProgram.ExtraGodray = 1;
        // render.RenderMesh(_meshref);
        standardShaderProgram.Stop();
        // if (ShouldRotateManual)
        //     Startup += (float)(deltaTime * 40.0 * (Math.PI / 180.0));
        // if (!ShouldRotateAutomated)
            // return;
    }

    public void Dispose()
    {
        coreClientApi.Event.UnregisterRenderer((IRenderer)this, EnumRenderStage.Opaque);
        _meshref?.Dispose();
    }
}