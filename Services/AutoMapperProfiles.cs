using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            /* De que tipo de dato a que tipo de dato se va a mapear */
            CreateMap<CuentaModel, CuentaCreacionModel>();
            CreateMap<TransaccionActualizacionModel, TransaccionModel>().ReverseMap();
        }
    }
}
