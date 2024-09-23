using LinkDev.IKEA.BLL.Models.Employees;
using LinkDev.IKEA.BLL.Services.Employees;
using Microsoft.AspNetCore.Mvc;

namespace LinkDev.IKEA.PL.Controllers
{
    public class EmployeeController : Controller
    {
        #region Services [Dependency Injection]

        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IWebHostEnvironment _environment;
        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger, IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _logger = logger;
            _environment = environment;
        }

        #endregion

        #region Index

        [HttpGet] // GET: /Employee/Index
        public IActionResult Index()
        {
            var employees = _employeeService.GetAllEmployees();
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
        public IActionResult Create(CreatedEmployeeDto employee)
        {
            if (!ModelState.IsValid)            // Server-Side Validation
                return View(employee);

            string message = string.Empty;
            try
            {
                var result = _employeeService.CreateEmployee(employee);
                if (result > 0)
                    return RedirectToAction(nameof(Index));
                else
                {
                    message = "Employee is not Created";
                    ModelState.AddModelError(string.Empty, message);
                    return View(employee);
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
            return View(employee);
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
        
            return View(new UpdatedEmployeeDto()
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
            });
        }
        
        [HttpPost]   // POST
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, UpdatedEmployeeDto employee)
        {
            if (!ModelState.IsValid)           // Server-Side Validation
                return View(employee);
        
            var message = string.Empty;
        
            try
            {
                var updated = _employeeService.UpdateEmployee(employee) > 0;
        
                if (updated)
                    return RedirectToAction(nameof(Index));
        
                message = "an error has occured during updating the Employee :(";
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                _logger.LogError(ex, ex.Message);
        
                // 2. Set Message
                message = _environment.IsDevelopment() ? ex.Message : "an error has occured during updating the Employee :(";
            }
        
            ModelState.AddModelError(string.Empty, message);
            return View(employee);
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
