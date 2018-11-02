using AutoMapper;
using webGDPR.ViewModels;

namespace webGDPR.Infrastructure
{
	public class AutomapperCollarProfile: Profile
    {
		public AutomapperCollarProfile() {

			CreateMap<System.Tuple<Models.Collar, Models.CollarStatus>, CustomWebSockets.Messages.Collar>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.Item1.CollarId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.Item1.BaseNumber))
				.ForMember(dest => dest.HWId, opts => opts.MapFrom(src => src.Item1.HWId))
				.ForMember(dest => dest.CollarNumber, opts => opts.MapFrom(src => src.Item1.CollarNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Item1.Description))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.Item2.IsConnected))
				.ForMember(dest => dest.ConnectedToName, opts => opts.MapFrom(src => src.Item2.BaseConnectedTo.Name))
				.ForMember(dest => dest.IsGPSConnected, opts => opts.MapFrom(src => src.Item2.IsGPSConnected))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Item2.Battery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Item2.Radio));

			CreateMap<CustomWebSockets.Messages.Collar, Models.Collar>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.CollarId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.CollarNumber, opts => opts.MapFrom(src => src.CollarNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

			CreateMap<CustomWebSockets.Messages.Collar, Models.Collar>().ReverseMap();

			CreateMap<CustomWebSockets.Messages.Collar, Models.CollarStatus>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.CollarId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.IsConnected))
				.ForMember(dest => dest.ConnectedTo, opts => opts.MapFrom(src => src.ConnectedTo))
				.ForMember(dest => dest.IsGPSConnected, opts => opts.MapFrom(src => src.IsGPSConnected))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Battery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Radio));

			CreateMap<CustomWebSockets.Messages.Collar, Models.CollarStatus>().ReverseMap();

			CreateMap<System.Tuple<Models.Collar, Models.CollarStatus>, CollarViewModel>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.Item1.CollarId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.Item1.BaseNumber))
				.ForMember(dest => dest.HWId, opts => opts.MapFrom(src => src.Item1.HWId))
				.ForMember(dest => dest.CollarNumber, opts => opts.MapFrom(src => src.Item1.CollarNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Item1.Description))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.Item2.IsConnected))
				.ForMember(dest => dest.ConnectedTo, opts => opts.MapFrom(src => src.Item2.ConnectedTo))
				.ForMember(dest => dest.IsGPSConnected, opts => opts.MapFrom(src => src.Item2.IsGPSConnected))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Item2.Battery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Item2.Radio));

			CreateMap<CollarViewModel, Models.Collar>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.CollarId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.CollarNumber, opts => opts.MapFrom(src => src.CollarNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

			CreateMap<CollarViewModel, Models.Collar>().ReverseMap();

			CreateMap<CollarViewModel, Models.CollarStatus>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.CollarId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.IsConnected))
				.ForMember(dest => dest.ConnectedTo, opts => opts.MapFrom(src => src.ConnectedTo))
				.ForMember(dest => dest.IsGPSConnected, opts => opts.MapFrom(src => src.IsGPSConnected))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Battery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Radio));

			CreateMap<CollarViewModel, Models.CollarStatus>().ReverseMap();

			CreateMap<EditCollarViewModel, Models.Collar>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.CollarId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

		}
	}
}
