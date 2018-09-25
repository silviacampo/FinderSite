using AutoMapper;
using webGDPR.ViewModels;

namespace webGDPR.Infrastructure
{
	public class AutomapperBaseProfile: Profile
    {
		public AutomapperBaseProfile() {

			CreateMap<System.Tuple<Models.Base, Models.BaseStatus>, CustomWebSockets.Messages.Base>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.Item1.BaseId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.Item1.BaseNumber))
				.ForMember(dest => dest.HWId, opts => opts.MapFrom(src => src.Item1.HWId))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Item1.Description))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.Item2.IsConnected))
				.ForMember(dest => dest.ConnectedTo, opts => opts.MapFrom(src => src.Item2.ConnectedTo))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.Item2.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.Item2.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Item2.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.Item2.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Item2.Radio));

			CreateMap<CustomWebSockets.Messages.Base, webGDPR.Models.Base>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

			CreateMap<CustomWebSockets.Messages.Base, webGDPR.Models.Base>().ReverseMap();

			CreateMap<CustomWebSockets.Messages.Base, Models.BaseStatus>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.IsConnected))
				.ForMember(dest => dest.ConnectedTo, opts => opts.MapFrom(src => src.ConnectedTo))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Radio));

			CreateMap<CustomWebSockets.Messages.Base, Models.BaseStatus>().ReverseMap();

			CreateMap<webGDPR.Models.Base, CustomWebSockets.Messages.BaseCore>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.HWId, opts => opts.MapFrom(src => src.HWId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

			CreateMap<System.Tuple<webGDPR.Models.Base, Models.BaseStatus>, BaseViewModel>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.Item1.BaseId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.HWId, opts => opts.MapFrom(src => src.Item1.HWId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.Item1.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Item1.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Item1.Description))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Item1.UserId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.Item2.IsConnected))
				.ForMember(dest => dest.ConnectedTo, opts => opts.MapFrom(src => src.Item2.ConnectedTo))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.Item2.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.Item2.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Item2.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.Item2.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Item2.Radio));

			CreateMap<BaseViewModel, webGDPR.Models.Base>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description));

			CreateMap<BaseViewModel, webGDPR.Models.Base>().ReverseMap();

			CreateMap<BaseViewModel, Models.BaseStatus>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.IsConnected))
				.ForMember(dest => dest.ConnectedTo, opts => opts.MapFrom(src => src.ConnectedTo))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Radio));

			CreateMap<BaseViewModel, Models.BaseStatus>().ReverseMap();
		}
	}
}
