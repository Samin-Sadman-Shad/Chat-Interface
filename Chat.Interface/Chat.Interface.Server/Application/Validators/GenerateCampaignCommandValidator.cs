using FluentValidation;
using Chat.Interface.Server.Application.Commands;

namespace Chat.Interface.Server.Application.Validators;

public class GenerateCampaignCommandValidator : AbstractValidator<GenerateCampaignCommand>
{
    public GenerateCampaignCommandValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty()
            .WithMessage("Campaign prompt is required")
            .MinimumLength(10)
            .WithMessage("Prompt must be at least 10 characters")
            .MaximumLength(1000)
            .WithMessage("Prompt cannot exceed 1000 characters");

        RuleFor(x => x.DataSourceIds)
            .NotNull()
            .WithMessage("Data source IDs are required");

        RuleFor(x => x.PreferredChannels)
            .NotNull()
            .WithMessage("Preferred channels are required")
            .Must(channels => channels.Count > 0)
            .WithMessage("At least one channel must be selected");
    }
}