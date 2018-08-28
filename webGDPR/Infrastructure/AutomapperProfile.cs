using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			CreateMap<AgendaSignalR.Infrastructure.Base, CustomWebSockets.Messages.Base>()
				.ForMember(dest => dest.BaseId, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseNumber))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description))
				.ForMember(dest => dest.IsConnected, opts => opts.MapFrom(src => src.LastStatus.IsConnected))
				.ForMember(dest => dest.IsPlugged, opts => opts.MapFrom(src => src.LastStatus.IsPlugged))
				.ForMember(dest => dest.IsCharging, opts => opts.MapFrom(src => src.LastStatus.IsCharging))
				.ForMember(dest => dest.Battery, opts => opts.MapFrom(src => src.LastStatus.Battery))
				.ForMember(dest => dest.HasBattery, opts => opts.MapFrom(src => src.LastStatus.HasBattery))
				.ForMember(dest => dest.Radio, opts => opts.MapFrom(src => src.LastStatus.Radio));

			CreateMap<AgendaSignalR.Infrastructure.Base, CustomWebSockets.Messages.Base>().ReverseMap();

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
		}
	}
}
