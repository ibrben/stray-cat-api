using Microsoft.EntityFrameworkCore;
using StrayCat.Domain.Entities;
using StrayCat.Domain.Enums;

namespace StrayCat.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(StrayCatDbContext context)
        {
            // Check if TripDates have been seeded
            if (await context.TripDates.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Get existing trips
            var existingTrips = await context.Trips.ToListAsync();
            
            if (!existingTrips.Any())
            {
                return; // No trips to seed data for
            }

            // Add TripDates for existing trips
            var tripDates = new List<TripDate>
            {
                new TripDate
                {
                    TripId = existingTrips.FirstOrDefault(t => t.Title.Contains("Mountain"))?.Id ?? 1,
                    StartDate = DateTime.UtcNow.AddDays(30),
                    EndDate = DateTime.UtcNow.AddDays(37),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TripDate
                {
                    TripId = existingTrips.FirstOrDefault(t => t.Title.Contains("Tropical"))?.Id ?? 2,
                    StartDate = DateTime.UtcNow.AddDays(45),
                    EndDate = DateTime.UtcNow.AddDays(50),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TripDate
                {
                    TripId = existingTrips.FirstOrDefault(t => t.Title.Contains("Safari"))?.Id ?? 4,
                    StartDate = DateTime.UtcNow.AddDays(60),
                    EndDate = DateTime.UtcNow.AddDays(66),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.TripDates.AddRangeAsync(tripDates);

            // Add TripTags for existing trips
            var tripTags = new List<TripTag>();
            
            foreach (var trip in existingTrips)
            {
                if (trip.Title.Contains("Mountain"))
                {
                    tripTags.AddRange(new[]
                    {
                        new TripTag { TripId = trip.Id, Name = "Adventure", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Mountains", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Hiking", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    });
                }
                else if (trip.Title.Contains("Tropical"))
                {
                    tripTags.AddRange(new[]
                    {
                        new TripTag { TripId = trip.Id, Name = "Beach", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Snorkeling", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Tropical", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    });
                }
                else if (trip.Title.Contains("Cultural"))
                {
                    tripTags.AddRange(new[]
                    {
                        new TripTag { TripId = trip.Id, Name = "Cultural", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Temples", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "History", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    });
                }
                else if (trip.Title.Contains("Safari"))
                {
                    tripTags.AddRange(new[]
                    {
                        new TripTag { TripId = trip.Id, Name = "Wildlife", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Safari", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Big Five", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    });
                }
                else if (trip.Title.Contains("Northern"))
                {
                    tripTags.AddRange(new[]
                    {
                        new TripTag { TripId = trip.Id, Name = "Aurora", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Northern Lights", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new TripTag { TripId = trip.Id, Name = "Arctic", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    });
                }
            }

            await context.TripTags.AddRangeAsync(tripTags);
            await context.SaveChangesAsync();
        }
    }
}
