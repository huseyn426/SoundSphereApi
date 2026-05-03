using FluentValidation;
using SoundSphereApi.DTOs.Music;

namespace SoundSphereApi.Validators
{
    public class TrackCreateDtoValidator : AbstractValidator<TrackCreateDto>
    {
        public TrackCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Track title is required.")
                .MaximumLength(150).WithMessage("Track title cannot exceed 150 characters.");

            RuleFor(x => x.Duration)
                .NotEmpty().WithMessage("Duration is required.");

            RuleFor(x => x.ArtistId)
                .GreaterThan(0).WithMessage("ArtistId must be greater than 0.");

            RuleFor(x => x.AlbumId)
                .GreaterThan(0).WithMessage("AlbumId must be greater than 0.");

            RuleFor(x => x.GenreId)
                .GreaterThan(0).WithMessage("GenreId must be greater than 0.");
        }
    }
}
