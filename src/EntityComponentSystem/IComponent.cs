namespace PMDEvers.EntityComponentSystem
{
    public interface IComponent
    {
        IEntityRecord Record { get; set; }
    }
}