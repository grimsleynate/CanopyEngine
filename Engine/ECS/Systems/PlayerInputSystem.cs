using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;
using Platform.Abstractions;
using System.Numerics;

namespace ECS.Systems;

public sealed class PlayerInputSystem : AEntitySetSystem<float>
{
    private readonly IInput _input;
    private readonly float _moveSpeed;

    public PlayerInputSystem(World world, IInput input, float moveSpeed) 
        : base(world.GetEntities()
              .With<PlayerTag>()
              .With<Velocity>()
              .AsSet())
    {
        _input = input;
        _moveSpeed = moveSpeed;
    }

    protected override void Update(float delta, in Entity entity)
    {
        ref Velocity vel = ref entity.Get<Velocity>();

        // Read digital keyboard axes
        float kx = 0f;
        float ky = 0f;
        if (_input.IsKeyDown(KeyCode.A) || _input.IsKeyDown(KeyCode.Left)) kx -= 1f;
        if (_input.IsKeyDown(KeyCode.D) || _input.IsKeyDown(KeyCode.Right)) kx += 1f;
        if (_input.IsKeyDown(KeyCode.W) || _input.IsKeyDown(KeyCode.Up)) ky -= 1f;
        if (_input.IsKeyDown(KeyCode.S) || _input.IsKeyDown(KeyCode.Down)) ky += 1f;

        var dir = new Vector2(kx, ky);

        // Normalize digital diagonal so diagonal speed equals straight speed
        if (dir.LengthSquared() > 1f)
            dir = Vector2.Normalize(dir);

        vel.X = dir.X * _moveSpeed;
        vel.Y = dir.Y * _moveSpeed;

        // Read mouse for aiming or interactions
        Vector2 mousePos = _input.MousePosition;
        // store or use mousePos in another system (aiming, shooting)
    }
}