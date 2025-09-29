using MediatR;
using Chat.Interface.Server.Domain.Entities;
using Chat.Interface.Server.Infrastructure.Repositories;
using Chat.Interface.Server.Infrastructure.Services;

namespace Chat.Interface.Server.Application.Commands;

public record ConnectDataSourceCommand(
    DataSourceType Type,
    string Name,
    Dictionary<string, object> Configuration
) : IRequest<Guid>;

public class ConnectDataSourceCommandHandler : IRequestHandler<ConnectDataSourceCommand, Guid>
{
    private readonly IDataSourceRepository _repository;
    private readonly IDataSourceConnector _connector;

    public ConnectDataSourceCommandHandler(IDataSourceRepository repository, IDataSourceConnector connector)
    {
        _repository = repository;
        _connector = connector;
    }

    public async Task<Guid> Handle(ConnectDataSourceCommand request, CancellationToken cancellationToken)
    {
        var dataSource = new DataSource
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            Configuration = request.Configuration,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            var isConnected = await _connector.TestConnectionAsync(dataSource);
            dataSource.IsConnected = isConnected;
            
            if (isConnected)
            {
                dataSource.LastSyncAt = DateTime.UtcNow;
            }
        }
        catch
        {
            dataSource.IsConnected = false;
        }

        await _repository.AddAsync(dataSource);
        return dataSource.Id;
    }
}