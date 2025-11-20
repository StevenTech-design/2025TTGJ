namespace TTGJ.Entity
{
    public interface IEntity
    {
       public uint EntityId{ get;}
       public EntityType EntityType{ get;}
    }
    public enum EntityType { 
        Player,
        AirShip
    }
}