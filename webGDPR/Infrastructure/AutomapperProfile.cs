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
			CreateMap<AgendaSignalR.Infrastructure.Base, CustomWebSockets.Messages.Base>()
				.ForMember(dest => dest.BaseNumber, opts => opts.MapFrom(src => src.BaseId))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name)); 
		}
	}
}
