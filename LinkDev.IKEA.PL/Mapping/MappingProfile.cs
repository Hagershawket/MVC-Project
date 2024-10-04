using AutoMapper;
using LinkDev.IKEA.BLL.Models.Departments;
using LinkDev.IKEA.BLL.Models.Employees;
using LinkDev.IKEA.PL.ViewModels.Departments;
using LinkDev.IKEA.PL.ViewModels.Employees;

namespace LinkDev.IKEA.PL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Employee

            CreateMap<EmployeeDetailsDto, EmployeeViewModel>();
            CreateMap<EmployeeViewModel, UpdatedEmployeeDto>();
            CreateMap<EmployeeViewModel, CreatedEmployeeDto>();

            #endregion

            #region Department

            CreateMap<DepartmentDetailsDto, DepartmentViewModel>()
                /*.ForMember(dest => dest.NameX, config => config.MapFrom(src => src.Name))*/
                //.ReverseMap()
                /*.ForMember(dest => dest.Name, config => config.MapFrom(src => src.NameX))*/;

            CreateMap<DepartmentViewModel, UpdatedDepartmentDto>();
            CreateMap<DepartmentViewModel, CreatedDepartmentDto>();

            #endregion
        }
    }
}
