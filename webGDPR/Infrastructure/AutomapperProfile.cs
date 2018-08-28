//https://dotnetcoretutorials.com/2017/09/23/using-automapper-asp-net-core/
//https://cpratt.co/using-automapper-creating-mappings/
//var test = mapper.Map<webGDPR.Infrastructure.CustomWebSockets.Messages.Base>(bases.First());
//var test2 = mapper.Map<Base>(test);

using AutoMapper;
using webGDPR.ViewModels;

namespace webGDPR.Infrastructure
{
    public class AutomapperProfile : Profile
    {
		public AutomapperProfile()
		{
			#region Base
			CreateMap<System.Tuple<AgendaSignalR.Infrastructure.Base, Models.BaseStatus>, CustomWebSockets.Messages.Base>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.Item1.BaseId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.Item1.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Item1.Description))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.Item2.IsConnected))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.Item2.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.Item2.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Item2.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.Item2.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Item2.Radio));

			CreateMap<CustomWebSockets.Messages.Base, AgendaSignalR.Infrastructure.Base>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

			CreateMap<CustomWebSockets.Messages.Base, AgendaSignalR.Infrastructure.Base>().ReverseMap();

			CreateMap<CustomWebSockets.Messages.Base, Models.BaseStatus>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.IsConnected))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Radio));

			CreateMap<CustomWebSockets.Messages.Base, Models.BaseStatus>().ReverseMap();
			#endregion

			#region Collar
			CreateMap<System.Tuple<AgendaSignalR.Infrastructure.Collar, Models.CollarStatus>, CustomWebSockets.Messages.Collar>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.Item1.CollarId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.Item1.BaseNumber))
				.ForMember(dest => dest.CollarNumber, opts => opts.MapFrom(src => src.Item1.CollarNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Item1.Description))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.Item2.IsConnected))
				.ForMember(dest => dest.IsGPSConnected, opts => opts.MapFrom(src => src.Item2.IsGPSConnected))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Item2.Battery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Item2.Radio));

			CreateMap<CustomWebSockets.Messages.Collar, AgendaSignalR.Infrastructure.Collar>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.CollarId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.CollarNumber, opts => opts.MapFrom(src => src.CollarNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

			CreateMap<CustomWebSockets.Messages.Collar, AgendaSignalR.Infrastructure.Collar>().ReverseMap();

			CreateMap<CustomWebSockets.Messages.Collar, Models.CollarStatus>()
				.ForMember(dest => dest.CollarId, opts => opts.MapFrom(src => src.CollarId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.IsConnected))
				.ForMember(dest => dest.IsGPSConnected, opts => opts.MapFrom(src => src.IsGPSConnected))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Battery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Radio));

			CreateMap<CustomWebSockets.Messages.Collar, Models.CollarStatus>().ReverseMap();
			#endregion

			#region BaseViewModel
			CreateMap<System.Tuple<AgendaSignalR.Infrastructure.Base, Models.BaseStatus>, BaseViewModel>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.Item1.BaseId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.HWId, opts => opts.MapFrom(src => src.Item1.HWId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.Item1.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Item1.Description))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.Item2.IsConnected))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.Item2.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.Item2.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Item2.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.Item2.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Item2.Radio));

			CreateMap<BaseViewModel, AgendaSignalR.Infrastructure.Base>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

			CreateMap<BaseViewModel, AgendaSignalR.Infrastructure.Base>().ReverseMap();

			CreateMap<BaseViewModel, Models.BaseStatus>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.IsConnected))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Radio));

			CreateMap<BaseViewModel, Models.BaseStatus>().ReverseMap();
			#endregion

		}
	}
}
