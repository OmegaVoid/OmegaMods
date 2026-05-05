using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Thaumaturgy.BlockBehaviour;

public class BlockBehaviorInfusionStabiliser(Block block) : BlockBehavior(block)
{
    public float Stability { get; private set; }
    public override void Initialize(JsonObject properties)
    {
        base.Initialize(properties);
        var token = properties["stability"].Token;
        if (token == null)
            return;
        Stability = token.ToObject<float>();
    }
}