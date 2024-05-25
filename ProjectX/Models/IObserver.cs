namespace ProjectX.Models;

public interface IObserver
{
    void Update(object? data);
}

public interface IObservable
{
    void AddObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void NotifyObservers(object? data);
}
