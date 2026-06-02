using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;
using Platform.Abstractions;

namespace ECS.Systems;

public sealed class PlayerInputSystem : AEntitySetSystem<float>
{
    private readonly IInput _input;

    public PlayerInputSystem(World world, IInput input) 
        : base(world.GetEntities()
              .With<PlayerTag>()
              .With<Velocity>()
              .AsSet())
    {
        _input = input;
    }

    protected override void Update(float delta, in Entity entity)
    {
        ref Velocity vel = ref entity.Get<Velocity>();

        float move = 0f;

        if (_input.IsKeyDown(KeyCode.A))
            move -= 1f;

        if (_input.IsKeyDown(KeyCode.D))
            move += 1f;

        vel.X = move * 200f; //pixels per second
    }
}