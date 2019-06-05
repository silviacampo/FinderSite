//https://dotnetcoretutorials.com/2017/09/23/using-automapper-asp-net-core/
//https://cpratt.co/using-automapper-creating-mappings/

using AutoMapper;
using webGDPR.ViewModels;

namespace webGDPR.Infrastructure
{
    public class AutomapperPetProfile : Profile
    {
		public AutomapperPetProfile()
		{
			CreateMap<System.Tuple<Models.Pet, Models.PetTrackingInfo>, PetViewModel>()
				.ForMember(dest => dest.PetId, opts => opts.MapFrom(src => src.Item1.PetId))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Age, opts => opts.MapFrom(src => src.Item1.Age))
				.ForMember(dest => dest.Breeding, opts => opts.MapFrom(src => src.Item1.Breeding))
				.ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Item1.Type))
				.ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Item1.Color))
				.ForMember(dest => dest.HealthComments, opts => opts.MapFrom(src => src.Item1.HealthComments))
				.ForMember(dest => dest.DefaultMode, opts => opts.MapFrom(src => src.Item1.DefaultMode))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.Latitude, opts => opts.MapFrom(src => src.Item2.Latitude))
				.ForMember(dest => dest.Longitude, opts => opts.MapFrom(src => src.Item2.Longitude))
				.ForMember(dest => dest.Acceleration, opts => opts.MapFrom(src => src.Item2.Acceleration))
				.ForMember(dest => dest.LocationCreationDate, opts => opts.MapFrom(src => src.Item2.CreationDate));

			CreateMap<PetViewModel, Models.Pet>()
				.ForMember(dest => dest.PetId, opts => opts.MapFrom(src => src.PetId))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Age, opts => opts.MapFrom(src => src.Age))
				.ForMember(dest => dest.Breeding, opts => opts.MapFrom(src => src.Breeding))
				.ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type))
				.ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Color))
				.ForMember(dest => dest.HealthComments, opts => opts.MapFrom(src => src.HealthComments))
				.ForMember(dest => dest.DefaultMode, opts => opts.MapFrom(src => src.DefaultMode))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId));

			CreateMap<PetViewModel, Models.Pet>().ReverseMap();

			CreateMap<PetViewModel, Models.PetTrackingInfo>()
				.ForMember(dest => dest.Latitude, opts => opts.MapFrom(src => src.Latitude))
				.ForMember(dest => dest.Longitude, opts => opts.MapFrom(src => src.Longitude))
				.ForMember(dest => dest.Acceleration, opts => opts.MapFrom(src => src.Acceleration))
				.ForMember(dest => dest.CreationDate, opts => opts.MapFrom(src => src.LocationCreationDate));

			CreateMap<PetViewModel, Models.PetTrackingInfo>().ReverseMap();

			CreateMap<Models.EditPetViewModel, Models.Pet>()
	.ForMember(dest => dest.PetId, opts => opts.MapFrom(src => src.PetId))
	.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
	.ForMember(dest => dest.Age, opts => opts.MapFrom(src => src.Age))
	.ForMember(dest => dest.Breeding, opts => opts.MapFrom(src => src.Breeding))
	.ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type))
	.ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Color))
	.ForMember(dest => dest.HealthComments, opts => opts.MapFrom(src => src.HealthComments))
	.ForMember(dest => dest.LastCollarId, opts => opts.MapFrom(src =>src.CollarId))
	.ForMember(dest => dest.AgeUnit, opts => opts.MapFrom(src => src.AgeUnit))
	.ForMember(dest => dest.Birthdate, opts => opts.MapFrom(src => src.Birthdate))
	.ForMember(dest => dest.Weigth, opts => opts.MapFrom(src => src.Weigth))
	.ForMember(dest => dest.WeigthUnit, opts => opts.MapFrom(src => src.WeigthUnit))
	.ForMember(dest => dest.Gender, opts => opts.MapFrom(src => src.Gender))
	.ForMember(dest => dest.DefaultMode, opts => opts.MapFrom(src => src.DefaultMode));

			CreateMap<Models.EditPetViewModel, Models.Pet>().ReverseMap();

			CreateMap<System.Tuple<Models.Pet, Models.PetTrackingInfo>, PetStatsModel>()
				.ForMember(dest => dest.PetId, opts => opts.MapFrom(src => src.Item1.PetId))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Age, opts => opts.MapFrom(src => src.Item1.Age))
				.ForMember(dest => dest.AgeUnit, opts => opts.MapFrom(src => src.Item1.AgeUnit))
				.ForMember(dest => dest.Birthdate, opts => opts.MapFrom(src => src.Item1.Birthdate))
				.ForMember(dest => dest.Gender, opts => opts.MapFrom(src => src.Item1.Gender))
				.ForMember(dest => dest.Weigth, opts => opts.MapFrom(src => src.Item1.Weigth))
				.ForMember(dest => dest.WeigthUnit, opts => opts.MapFrom(src => src.Item1.WeigthUnit))
				.ForMember(dest => dest.Breeding, opts => opts.MapFrom(src => src.Item1.Breeding))
				.ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Item1.Type))
				.ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Item1.Color))
				.ForMember(dest => dest.HealthComments, opts => opts.MapFrom(src => src.Item1.HealthComments))
				.ForMember(dest => dest.DefaultMode, opts => opts.MapFrom(src => src.Item1.DefaultMode))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.Latitude, opts => opts.MapFrom(src => src.Item2.Latitude))
				.ForMember(dest => dest.Longitude, opts => opts.MapFrom(src => src.Item2.Longitude))
				.ForMember(dest => dest.Acceleration, opts => opts.MapFrom(src => src.Item2.Acceleration))
				.ForMember(dest => dest.LocationCreationDate, opts => opts.MapFrom(src => src.Item2.CreationDate));

			CreateMap<PetStatsModel, Models.Pet>()
				.ForMember(dest => dest.PetId, opts => opts.MapFrom(src => src.PetId))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Age, opts => opts.MapFrom(src => src.Age))
				.ForMember(dest => dest.Breeding, opts => opts.MapFrom(src => src.Breeding))
				.ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type))
				.ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Color))
				.ForMember(dest => dest.HealthComments, opts => opts.MapFrom(src => src.HealthComments))
				.ForMember(dest => dest.DefaultMode, opts => opts.MapFrom(src => src.DefaultMode))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId));

			CreateMap<PetStatsModel, Models.Pet>().ReverseMap();

			CreateMap<PetStatsModel, Models.PetTrackingInfo>()
				.ForMember(dest => dest.Latitude, opts => opts.MapFrom(src => src.Latitude))
				.ForMember(dest => dest.Longitude, opts => opts.MapFrom(src => src.Longitude))
				.ForMember(dest => dest.Acceleration, opts => opts.MapFrom(src => src.Acceleration))
				.ForMember(dest => dest.CreationDate, opts => opts.MapFrom(src => src.LocationCreationDate));

			CreateMap<PetStatsModel, Models.PetTrackingInfo>().ReverseMap();
		}
	}
}
