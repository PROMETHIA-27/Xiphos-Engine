namespace Xiphos.ECS2
{
  partial struct Player
  {
    public void GetPartRef(out PartRef<Xiphos.ECS2.Physics> part)
      => part = Xiphos.ECS2.IEntity.GetPartRef(ref this.phys);
  }
}