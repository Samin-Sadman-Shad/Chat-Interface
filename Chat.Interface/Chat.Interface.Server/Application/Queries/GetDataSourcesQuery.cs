using MediatR;
using Chat.Interface.Server.Domain.Entities;
using Chat.Interface.Server.Infrastructure.Repositories;

namespace Chat.Interface.Server.Application.Queries;

public record GetDataSourcesQuery : IRequest<List<DataSource>>;

public class GetDataSourcesQueryHandler : IRequestHandler<GetDataSourcesQuery, List<DataSource>>
{
    private readonly IDataSourceRepository _repository;

    public GetDataSourcesQueryHandler(IDataSourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DataSource>> Handle(GetDataSourcesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}