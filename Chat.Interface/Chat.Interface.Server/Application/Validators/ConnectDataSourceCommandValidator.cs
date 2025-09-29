using FluentValidation;
using Chat.Interface.Server.Application.Commands;

namespace Chat.Interface.Server.Application.Validators;

public class ConnectDataSourceCommandValidator : AbstractValidator<ConnectDataSourceCommand>
{
    public ConnectDataSourceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Data source name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid data source type");

        RuleFor(x => x.Configuration)
            .NotNull()
            .WithMessage("Configuration is required");
    }
}