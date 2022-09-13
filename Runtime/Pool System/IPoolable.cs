public interface IPoolable
{
    public void OnGet();
    public void OnReleased();
    public void OnDestroyed();
}
