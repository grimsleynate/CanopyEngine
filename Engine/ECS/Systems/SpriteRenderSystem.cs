using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;
using Platform.Abstractions;

namespace ECS.Systems;

public sealed class SpriteRenderSystem : AEntitySetSystem<float>
{
    private readonly IRenderer _renderer;
    public SpriteRenderSystem(World world, IRenderer renderer)
        : base(world.GetEntities()
              .With<SpriteComponent>()
              .With<Position>()
              .AsSet())
    {
        _renderer = renderer;
    }
    protected override void Update(float delta, in Entity entity)
    {
        ref SpriteComponent spriteComp = ref entity.Get<SpriteComponent>();
        ref Position pos = ref entity.Get<Position>();
        _renderer.DrawSprite(
            spriteComp.Sprite,
            new System.Numerics.Vector2(pos.X, pos.Y),
            0f,
            Color.White
        );
    }
}