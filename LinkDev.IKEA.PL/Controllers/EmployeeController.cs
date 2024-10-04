using AutoMapper;
using LinkDev.IKEA.BLL.Models.Departments;
using LinkDev.IKEA.BLL.Models.Employees;
using LinkDev.IKEA.BLL.Services.Departments;
using LinkDev.IKEA.BLL.Services.Employees;
using LinkDev.IKEA.DAL.Entities.Departments;
using LinkDev.IKEA.DAL.Entities.Employees;
using LinkDev.IKEA.PL.ViewModels.Departments;
using LinkDev.IKEA.PL.ViewModels.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkDev.IKEA.PL.Controllers
{
	[Authorize]
	public class EmployeeController : Controller
    {
        #region Services [Dependency Injection]

        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService? _departmentService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public EmployeeController(
            IEmployeeService employeeService, 
            ILogger<EmployeeController> logger, 
            IWebHostEnvironment environment,
            IMapper mapper
            )
        {
            _employeeService = employeeService;
            _logger = logger;
            _environment = environment;
            _mapper = mapper;
        }

        #endregion

        #region Index

        [HttpGet] // GET: /Employee/Index
        public async Task<IActionResult> Index(string search)
        {
            var employees = await _employeeService.GetEmployeesAsync(search);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("Partials/_EmployeeListPartial", employees);

            return View(employees);
        }

        #endregion

        #region Details 

        [HttpGet] // /Employee/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return BadRequest();

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee is null)
                return NotFound();

            return View(employee);
        }

        #endregion

        #region Create

        [HttpGet] // GET: /Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] // POST
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)            // Server-Side Validation
                return View(employeeVM);

            string message = string.Empty;
            try
            {
                // Manual Mapping
                /// var createdEmployee = new CreatedEmployeeDto()
                /// {
                ///     Name = employeeVM.Name,
                ///     Age = employeeVM.Age,
                ///     Address = employeeVM.Address,
                ///     Salary = employeeVM.Salary,
                ///     IsActive = employeeVM.IsActive,
                ///     Email = employeeVM.Email,
                ///     PhoneNumber = employeeVM.PhoneNumber,
                ///     HiringDate = employeeVM.HiringDate,
                ///     Gender = employeeVM.Gender,
                ///     EmployeeType = employeeVM.EmployeeType,
                ///     DepartmentId = employeeVM.DepartmentId,
                ///     Image = employeeVM.Image,
                /// };

                var createdEmployee = _mapper.Map<CreatedEmployeeDto>(employeeVM);

                var result = await _employeeService.CreateEmployeeAsync(createdEmployee);
                if (result > 0)
                    message = "Employee has been Created Successfully.";                  
                else
                    message = "Failed to create employee.";

                TempData["Message"] = message;
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                // 1. Log Exception
                _logger.LogError(ex, ex.Message);

                // 2. Set Message
                message = _environment.IsDevelopment() ? ex.Message : "an error has occured during creating the Employee :(";

            }

            ModelState.AddModelError(string.Empty, message);
            return View(employeeVM);
        }

        #endregion

        #region Update

        [HttpGet]   // Employee/Edit/id?
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return BadRequest();    // 400
        
            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
        
            if (employee is null)
                return NotFound();      // 404

            // Manual Mapping 
            /// var departmentVM new EmployeeViewModel()
            /// {
            ///     Name = employee.Name,
            ///     Address = employee.Address,
            ///     Email = employee.Email,
            ///     Age = employee.Age,
            ///     Salary = employee.Salary,
            ///     PhoneNumber = employee.PhoneNumber,
            ///     IsActive = employee.IsActive,
            ///     EmployeeType = employee.EmployeeType,
            ///     Gender = employee.Gender,
            ///     HiringDate = employee.HiringDate,
            ///     DepartmentId = employee.DepartmentId,
            /// };
            
            var employeeVM = _mapper.Map<EmployeeDetailsDto, EmployeeViewModel>(employee);

            return View(employeeVM);
        }
        
        [HttpPost]   // POST
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)           // Server-Side Validation
                return View(employeeVM);
        
            var message = string.Empty;
        
            try
            {
                // Manual Mapping
                /// var employeeToUpdate = new UpdatedEmployeeDto()
                /// {
                ///     Id = id,
                ///     Name = employeeVM.Name,
                ///     Age = employeeVM.Age,
                ///     Address = employeeVM.Address,
                ///     Salary = employeeVM.Salary,
                ///     IsActive = employeeVM.IsActive,
                ///     Email = employeeVM.Email,
                ///     PhoneNumber = employeeVM.PhoneNumber,
                ///     HiringDate = employeeVM.HiringDate,
                ///     Gender = employeeVM.Gender,
                ///     EmployeeType = employeeVM.EmployeeType,
                ///     DepartmentId = employeeVM.DepartmentId,
                /// };

                var employeeToUpdate = _mapper.Map<UpdatedEmployeeDto>(employeeVM);

                employeeToUpdate.Id = id;

                var updated = await _employeeService.UpdateEmployeeAsync(employeeToUpdate) > 0;

                if (updated)
                    message = "Employee has been Updated Successfully";
                else
                    message = "Failed to update employee.";

                TempData["Message"] = message;
                return RedirectToAction(nameof(Index));
        
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                _logger.LogError(ex, ex.Message);
        
                // 2. Set Message
                message = _environment.IsDevelopment() ? ex.Message : "an error has occured during updating the Employee :(";
            }
        
            ModelState.AddModelError(string.Empty, message);
            return View(employeeVM);
        }

        #endregion

        #region Delete

        [HttpPost]  // POST
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = string.Empty;

            try
            {
                var deleted = await _employeeService.DeleteEmployeeAsync(id);

                if (deleted)
                    return RedirectToAction(nameof(Index));

                message = "an error has occured during deleting the Employee :(";
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                _logger.LogError(ex, ex.Message);

                // 2. Set Message
                message = _environment.IsDevelopment() ? ex.Message : "an error has occured during deleting the Employee :(";
            }

            //ModelState.AddModelError(string.Empty, message);
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
