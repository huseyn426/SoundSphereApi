using FluentValidation;
using SoundSphereApi.DTOs.Playlist;

namespace SoundSphereApi.Validators
{
    public class PlaylistCreateDtoValidator : AbstractValidator<PlaylistCreateDto>
    {
        public PlaylistCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Playlist name is required.")
                .MaximumLength(100).WithMessage("Playlist name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
