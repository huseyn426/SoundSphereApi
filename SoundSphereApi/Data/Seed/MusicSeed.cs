using SoundSphereApi.Data.Context;
using SoundSphereApi.Models.Music;
using SoundSphereApi.Models.Payment;

namespace SoundSphereApi.Data.Seed
{
    public static class MusicSeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (context.Genres.Any())
            {
                return; // Already seeded
            }

            // --- Genres ---
            var genres = new List<Genre>
            {
                new Genre { Name = "Pop" },
                new Genre { Name = "Rock" },
                new Genre { Name = "Hip-Hop" },
                new Genre { Name = "Electronic" },
                new Genre { Name = "Jazz" },
                new Genre { Name = "R&B" },
                new Genre { Name = "Classical" },
                new Genre { Name = "Indie" }
            };
            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();

            // --- Artists ---
            var artists = new List<Artist>
            {
                new Artist
                {
                    Name = "Luna Wave",
                    Bio = "Electronic pop artist blending futuristic synths with dreamy vocals.",
                    ImageUrl = "https://picsum.photos/seed/luna/300/300"
                },
                new Artist
                {
                    Name = "The Midnight Echo",
                    Bio = "Alternative rock band known for atmospheric soundscapes and powerful lyrics.",
                    ImageUrl = "https://picsum.photos/seed/midnight/300/300"
                },
                new Artist
                {
                    Name = "Jay Phoenix",
                    Bio = "Hip-hop artist with a signature flow and conscious storytelling.",
                    ImageUrl = "https://picsum.photos/seed/jayphoenix/300/300"
                },
                new Artist
                {
                    Name = "Aria Skye",
                    Bio = "Jazz vocalist with a modern twist on classic standards.",
                    ImageUrl = "https://picsum.photos/seed/ariaskye/300/300"
                },
                new Artist
                {
                    Name = "Neon District",
                    Bio = "Electronic duo creating high-energy beats for the dancefloor.",
                    ImageUrl = "https://picsum.photos/seed/neondistrict/300/300"
                }
            };
            await context.Artists.AddRangeAsync(artists);
            await context.SaveChangesAsync();

            // --- Albums ---
            var albums = new List<Album>
            {
                new Album
                {
                    Title = "Dreamscape",
                    ArtistId = artists[0].Id,
                    ReleaseDate = new DateTime(2025, 3, 15),
                    CoverImageUrl = "https://picsum.photos/seed/dreamscape/300/300"
                },
                new Album
                {
                    Title = "Shadows & Light",
                    ArtistId = artists[1].Id,
                    ReleaseDate = new DateTime(2024, 11, 8),
                    CoverImageUrl = "https://picsum.photos/seed/shadows/300/300"
                },
                new Album
                {
                    Title = "Street Poetry",
                    ArtistId = artists[2].Id,
                    ReleaseDate = new DateTime(2025, 1, 20),
                    CoverImageUrl = "https://picsum.photos/seed/streetpoetry/300/300"
                },
                new Album
                {
                    Title = "Velvet Nights",
                    ArtistId = artists[3].Id,
                    ReleaseDate = new DateTime(2024, 7, 5),
                    CoverImageUrl = "https://picsum.photos/seed/velvetnights/300/300"
                },
                new Album
                {
                    Title = "Pulse",
                    ArtistId = artists[4].Id,
                    ReleaseDate = new DateTime(2025, 5, 1),
                    CoverImageUrl = "https://picsum.photos/seed/pulse/300/300"
                }
            };
            await context.Albums.AddRangeAsync(albums);
            await context.SaveChangesAsync();

            // --- Tracks ---
            var tracks = new List<Track>
            {
                // Luna Wave — Dreamscape (Pop / Electronic)
                new Track { Title = "Crystal Horizon", ArtistId = artists[0].Id, AlbumId = albums[0].Id, GenreId = genres[0].Id, Duration = TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(42)), PlayCount = 1240, AudioUrl = null, CoverImageUrl = albums[0].CoverImageUrl },
                new Track { Title = "Neon Heartbeat", ArtistId = artists[0].Id, AlbumId = albums[0].Id, GenreId = genres[3].Id, Duration = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(15)), PlayCount = 980, AudioUrl = null, CoverImageUrl = albums[0].CoverImageUrl },
                new Track { Title = "Floating Away", ArtistId = artists[0].Id, AlbumId = albums[0].Id, GenreId = genres[0].Id, Duration = TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(58)), PlayCount = 870, AudioUrl = null, CoverImageUrl = albums[0].CoverImageUrl },
                new Track { Title = "Digital Rain", ArtistId = artists[0].Id, AlbumId = albums[0].Id, GenreId = genres[3].Id, Duration = TimeSpan.FromMinutes(5).Add(TimeSpan.FromSeconds(10)), PlayCount = 650, AudioUrl = null, CoverImageUrl = albums[0].CoverImageUrl },

                // The Midnight Echo — Shadows & Light (Rock)
                new Track { Title = "Burning Bridges", ArtistId = artists[1].Id, AlbumId = albums[1].Id, GenreId = genres[1].Id, Duration = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(30)), PlayCount = 2100, AudioUrl = null, CoverImageUrl = albums[1].CoverImageUrl },
                new Track { Title = "Echoes of Tomorrow", ArtistId = artists[1].Id, AlbumId = albums[1].Id, GenreId = genres[1].Id, Duration = TimeSpan.FromMinutes(5).Add(TimeSpan.FromSeconds(5)), PlayCount = 1850, AudioUrl = null, CoverImageUrl = albums[1].CoverImageUrl },
                new Track { Title = "Silent Storm", ArtistId = artists[1].Id, AlbumId = albums[1].Id, GenreId = genres[7].Id, Duration = TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(50)), PlayCount = 1430, AudioUrl = null, CoverImageUrl = albums[1].CoverImageUrl },

                // Jay Phoenix — Street Poetry (Hip-Hop)
                new Track { Title = "City Lights", ArtistId = artists[2].Id, AlbumId = albums[2].Id, GenreId = genres[2].Id, Duration = TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(20)), PlayCount = 3200, AudioUrl = null, CoverImageUrl = albums[2].CoverImageUrl },
                new Track { Title = "Concrete Dreams", ArtistId = artists[2].Id, AlbumId = albums[2].Id, GenreId = genres[2].Id, Duration = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(0)), PlayCount = 2900, AudioUrl = null, CoverImageUrl = albums[2].CoverImageUrl },
                new Track { Title = "Rise Above", ArtistId = artists[2].Id, AlbumId = albums[2].Id, GenreId = genres[2].Id, Duration = TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(45)), PlayCount = 2500, AudioUrl = null, CoverImageUrl = albums[2].CoverImageUrl },
                new Track { Title = "Midnight Flow", ArtistId = artists[2].Id, AlbumId = albums[2].Id, GenreId = genres[5].Id, Duration = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(22)), PlayCount = 1800, AudioUrl = null, CoverImageUrl = albums[2].CoverImageUrl },

                // Aria Skye — Velvet Nights (Jazz)
                new Track { Title = "Blue Satin", ArtistId = artists[3].Id, AlbumId = albums[3].Id, GenreId = genres[4].Id, Duration = TimeSpan.FromMinutes(5).Add(TimeSpan.FromSeconds(30)), PlayCount = 720, AudioUrl = null, CoverImageUrl = albums[3].CoverImageUrl },
                new Track { Title = "Whisper in the Wind", ArtistId = artists[3].Id, AlbumId = albums[3].Id, GenreId = genres[4].Id, Duration = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(45)), PlayCount = 610, AudioUrl = null, CoverImageUrl = albums[3].CoverImageUrl },
                new Track { Title = "Moonlit Serenade", ArtistId = artists[3].Id, AlbumId = albums[3].Id, GenreId = genres[4].Id, Duration = TimeSpan.FromMinutes(6).Add(TimeSpan.FromSeconds(12)), PlayCount = 540, AudioUrl = null, CoverImageUrl = albums[3].CoverImageUrl },

                // Neon District — Pulse (Electronic)
                new Track { Title = "Overdrive", ArtistId = artists[4].Id, AlbumId = albums[4].Id, GenreId = genres[3].Id, Duration = TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(55)), PlayCount = 4100, AudioUrl = null, CoverImageUrl = albums[4].CoverImageUrl },
                new Track { Title = "Synthwave Sunset", ArtistId = artists[4].Id, AlbumId = albums[4].Id, GenreId = genres[3].Id, Duration = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(35)), PlayCount = 3800, AudioUrl = null, CoverImageUrl = albums[4].CoverImageUrl },
                new Track { Title = "Bass Reactor", ArtistId = artists[4].Id, AlbumId = albums[4].Id, GenreId = genres[3].Id, Duration = TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(18)), PlayCount = 3500, AudioUrl = null, CoverImageUrl = albums[4].CoverImageUrl },
                new Track { Title = "Electric Dreams", ArtistId = artists[4].Id, AlbumId = albums[4].Id, GenreId = genres[0].Id, Duration = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(50)), PlayCount = 2200, AudioUrl = null, CoverImageUrl = albums[4].CoverImageUrl },
            };
            await context.Tracks.AddRangeAsync(tracks);
            await context.SaveChangesAsync();

            // --- Subscription Plans ---
            if (!context.SubscriptionPlans.Any())
            {
                var plans = new List<SubscriptionPlan>
                {
                    new SubscriptionPlan { Name = "Free", Price = 0, DurationInDays = 36500, Description = "Basic access with ads. Limited skips." },
                    new SubscriptionPlan { Name = "Premium", Price = 9.99m, DurationInDays = 30, Description = "Ad-free listening, unlimited skips, offline mode, high quality audio." },
                    new SubscriptionPlan { Name = "Family", Price = 14.99m, DurationInDays = 30, Description = "Premium for up to 6 family members. Separate profiles included." }
                };
                await context.SubscriptionPlans.AddRangeAsync(plans);
                await context.SaveChangesAsync();
            }
        }
    }
}
