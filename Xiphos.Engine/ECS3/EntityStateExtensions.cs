using static Xiphos.ECS3.EntityState;

namespace Xiphos.ECS3;

public static partial class EntityStateExtensions
{
    public static Either<CreationError, (EntityIndex Index, Maybe<AccessError> Errs)> CreateEntityWith<T>(this EntityState @this, T component)
        where T : unmanaged, IComponent
        => @this.CreateEntity() switch
        {
            (Either.Left, var err, _) => new(err),
            (Either.Right, var index) => new((index, @this.SetComponent(index, component))),
            _ => throw new InvalidOperationException(),
        };

    public static Either<CreationError, (EntityIndex Index, Vec2<Maybe<AccessError>> Errs)> CreateEntityWith<T, U>(this EntityState @this, T comp1, U comp2)
        where T : unmanaged, IComponent
        where U : unmanaged, IComponent
        => @this.CreateEntity() switch
        {
            (Either.Left, var err, _) => new(err),
            (Either.Right, var index) => new((index, @this.SetComponents(index, (comp1, comp2)))),
            _ => throw new InvalidOperationException(),
        };

    public static Either<CreationError, (EntityIndex Index, Vec3<Maybe<AccessError>> Errs)> CreateEntityWith<T, U, V>(this EntityState @this, T comp1, U comp2, V comp3)
        where T : unmanaged, IComponent
        where U : unmanaged, IComponent
        where V : unmanaged, IComponent
        => @this.CreateEntity() switch
        {
            (Either.Left, var err, _) => new(err),
            (Either.Right, var index) => new((index, @this.SetComponents(index, (comp1, comp2, comp3)))),
            _ => throw new InvalidOperationException(),
        };

    public static Either<CreationError, (EntityIndex Index, Vec4<Maybe<AccessError>> Errs)> CreateEntityWith<T, U, V, W>(this EntityState @this, T comp1, U comp2, V comp3, W comp4)
        where T : unmanaged, IComponent
        where U : unmanaged, IComponent
        where V : unmanaged, IComponent
        where W : unmanaged, IComponent
        => @this.CreateEntity() switch
        {
            (Either.Left, var err, _) => new(err),
            (Either.Right, var index) => new((index, @this.SetComponents(index, (comp1, comp2, comp3, comp4)))),
            _ => throw new InvalidOperationException(),
        };

    public static Vec2<Maybe<AccessError>> SetComponents<T, U>(this EntityState @this, EntityIndex entity, (T, U) components)
        where T : unmanaged, IComponent
        where U : unmanaged, IComponent
        => (@this.SetComponent(entity, components.Item1),
        @this.SetComponent(entity, components.Item2));

    public static Vec3<Maybe<AccessError>> SetComponents<T, U, V>(this EntityState @this, EntityIndex entity, (T, U, V) components)
        where T : unmanaged, IComponent
        where U : unmanaged, IComponent
        where V : unmanaged, IComponent
        => (@this.SetComponents(entity, (components.Item1, components.Item2)),
        @this.SetComponent(entity, components.Item3));

    public static Vec4<Maybe<AccessError>> SetComponents<T, U, V, W>(this EntityState @this, EntityIndex entity, (T, U, V, W) components)
        where T : unmanaged, IComponent
        where U : unmanaged, IComponent
        where V : unmanaged, IComponent
        where W : unmanaged, IComponent
        => (@this.SetComponents(entity, (components.Item1, components.Item2, components.Item3)),
        @this.SetComponent(entity, components.Item4));
}
