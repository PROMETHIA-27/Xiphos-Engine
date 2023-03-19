namespace Xiphos.ECS2
{
  partial struct Player
  {
    public void GetPartRef(out PartRef<Xiphos.ECS2.Transform> part)
      => part = Xiphos.ECS2.IEntity.GetPartRef(ref this.trans);
  }
}