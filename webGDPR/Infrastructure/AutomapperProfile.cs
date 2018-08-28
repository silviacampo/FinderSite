//https://dotnetcoretutorials.com/2017/09/23/using-automapper-asp-net-core/
//https://cpratt.co/using-automapper-creating-mappings/
//var test = mapper.Map<webGDPR.Infrastructure.CustomWebSockets.Messages.Base>(bases.First());
//var test2 = mapper.Map<Base>(test);

using AutoMapper;

namespace webGDPR.Infrastructure
{
    public class AutomapperProfile : Profile
    {
		public AutomapperProfile()
		{
			//DB to message
			/*
			BaseId = b.BaseId,
			HWId = b.HWId,
			Name = b.Name,
			BaseNumber = b.BaseNumber,
			Text = b.Text,
			Description = b.Description,
			UserId = b.UserId,
			IsConnected = bs.IsConnected,
			IsPlugged = bs.IsPlugged,
			IsCharging = bs.IsCharging,
			Battery = bs.Battery,
			HasBattery = bs.HasBattery,
			Radio = bs.Radio
			 */
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

			//BaseStatus baseStatus = new BaseStatus
			//{
			//	BaseId = b.BaseId,
			//	UserId = b.UserId,
			//	IsActive = true,
			//	CreationDate = DateTime.Now,
			//	IsConnected = b.IsConnected,
			//	IsPlugged = b.IsPlugged,
			//	IsCharging = b.IsCharging,
			//	Battery = b.Battery,
			//	HasBattery = b.HasBattery,
			//	Radio = b.Radio
			//};

			CreateMap<CustomWebSockets.Messages.Base, Models.BaseStatus>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.IsConnected))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.Radio));

			CreateMap<CustomWebSockets.Messages.Base, Models.BaseStatus>().ReverseMap();
		}
	}
}
