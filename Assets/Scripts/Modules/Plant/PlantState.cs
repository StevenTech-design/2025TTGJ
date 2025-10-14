namespace TTGJ.Plant
{
    public enum PlantState
    {
        Seed = 0,
        Germination = 1,
        Mature = 2,
        Harvest = 3,
    }
    public class PlantSpacialParam
    {

    }
    public class PlantSpacialParam<T> : PlantSpacialParam
    {
        public T param;
    }
}