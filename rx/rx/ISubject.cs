namespace rx
{
    public interface ISubject<T> : IBobserver<T>, IBobservable<T>
    {
    }
}