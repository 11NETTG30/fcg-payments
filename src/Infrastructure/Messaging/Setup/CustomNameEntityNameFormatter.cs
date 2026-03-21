using MassTransit;

namespace Infrastructure.Messaging.Setup;

public class CustomNameEntityNameFormatter : IEntityNameFormatter
{
    public string FormatEntityName<T>()
    {
        return typeof(T).Name;
    }
}