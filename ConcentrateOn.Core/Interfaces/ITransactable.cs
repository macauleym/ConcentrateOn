namespace ConcentrateOn.Core.Interfaces;

public interface ITransactable
{
    Task BeginAsync();
    Task EndAsync();
}
