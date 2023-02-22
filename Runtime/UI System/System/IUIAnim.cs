using System;

public interface IUIAnim
{
    public void Play();
    public event Action OnFinished;
}
