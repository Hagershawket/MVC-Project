using LinkDev.IKEA.BLL.Models.Departments;
using LinkDev.IKEA.BLL.Models.Employees;
using LinkDev.IKEA.BLL.Services.Departments;
using LinkDev.IKEA.BLL.Services.Employees;
using LinkDev.IKEA.DAL.Entities.Departments;
using LinkDev.IKEA.DAL.Entities.Employees;
using LinkDev.IKEA.PL.ViewModels.Employees;
using Microsoft.AspNetCore.Mvc;

namespace LinkDev.IKEA.PL.Controllers
{
    public class EmployeeController : Controller
    {
        #region Services [Dependency Injection]

        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService? _departmentService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IWebHostEnvironment _environment;
        public EmployeeController(
            IEmployeeService employeeService, 
            ILogger<EmployeeController> logger, 
            IWebHostEnvironment environment
            )
        {
            _employeeService = employeeService;
            _logger = logger;
            _environment = environment;
        }

        #endregion

        #region Index

        [HttpGet] // GET: /Employee/Index
        public IActionResult Index(string search)
        {
            var employees = _employeeService.GetEmployees(search);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("Partials/_EmployeeListPartial", employees);

            return View(employees);
        }

        #endregion

        #region Details 

        [HttpGet] // /Employee/Details
        public IActionResult Details(int? id)
        {
            if (id is null)
                return BadRequest();

            var employee = _employeeService.GetEmployeeById(id.Value);
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
        public IActionResult Create(EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)            // Server-Side Validation
                return View(employeeVM);

            string message = string.Empty;
            try
            {
                var createdEmployee = new CreatedEmployeeDto()
                {
                    Name = employeeVM.Name,
                    Age = employeeVM.Age,
                    Address = employeeVM.Address,
                    Salary = employeeVM.Salary,
                    IsActive = employeeVM.IsActive,
                    Email = employeeVM.Email,
                    PhoneNumber = employeeVM.PhoneNumber,
                    HiringDate = employeeVM.HiringDate,
                    Gender = employeeVM.Gender,
                    EmployeeType = employeeVM.EmployeeType,
                    DepartmentId = employeeVM.DepartmentId,
                };
                var result = _employeeService.CreateEmployee(createdEmployee);
                if (result > 0)
                    return RedirectToAction(nameof(Index));
                else
                {
                    message = "Employee is not Created";
                    ModelState.AddModelError(string.Empty, message);
                    return View(employeeVM);
                }

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
        public IActionResult Edit(int? id)
        {
            if (id is null)
                return BadRequest();    // 400
        
            var employee = _employeeService.GetEmployeeById(id.Value);
        
            if (employee is null)
                return NotFound();      // 404
        
            return View(new EmployeeViewModel()
            {
                Name = employee.Name,
                Address = employee.Address,
                Email = employee.Email,
                Age = employee.Age,
                Salary = employee.Salary,
                PhoneNumber = employee.PhoneNumber,
                IsActive = employee.IsActive,
                EmployeeType = employee.EmployeeType,
                Gender = employee.Gender,
                HiringDate = employee.HiringDate,
                DepartmentId = employee.DepartmentId,
            });
        }
        
        [HttpPost]   // POST
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, UpdatedEmployeeDto employeeVM)
        {
            if (!ModelState.IsValid)           // Server-Side Validation
                return View(employeeVM);
        
            var message = string.Empty;
        
            try
            {
                //var employeeToUpdate = new UpdatedEmployeeDto()
                //{
                //    Name = employeeVM.Name,
                //    Age = employeeVM.Age,
                //    Address = employeeVM.Address,
                //    Salary = employeeVM.Salary,
                //    IsActive = employeeVM.IsActive,
                //    Email = employeeVM.Email,
                //    PhoneNumber = employeeVM.PhoneNumber,
                //    HiringDate = employeeVM.HiringDate,
                //    Gender = employeeVM.Gender,
                //    EmployeeType = employeeVM.EmployeeType,
                //    DepartmentId = employeeVM.DepartmentId,
                //};

                var updated = _employeeService.UpdateEmployee(employeeVM) > 0;

                if (updated)
                    message = "Employee has been Updated Successfully";
                else
                    message = "Employee is not Updated";

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
        public IActionResult Delete(int id)
        {
            var message = string.Empty;

            try
            {
                var deleted = _employeeService.DeleteEmployee(id);

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
