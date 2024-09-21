using LinkDev.IKEA.BLL.Models.Departments;
using LinkDev.IKEA.BLL.Services.Departments;
using LinkDev.IKEA.PL.ViewModels.Common;
using LinkDev.IKEA.PL.ViewModels.Departments;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LinkDev.IKEA.PL.Controllers
{
    // Inheritance: DepartmentController is  a Controller
    // Composition: DepartmentController has a IDepartmentService
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IWebHostEnvironment _environment;
        public DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger, IWebHostEnvironment environment)
        {
            _departmentService = departmentService;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet] // GET: /Department/Index
        public IActionResult Index()
        {
            var departments = _departmentService.GetAllDepartments();
            return View(departments);
        }

        [HttpGet] // /Department/Details
        public IActionResult Details(int? id)
        {
            if (id is null)
                return BadRequest();

            var department = _departmentService.GetDepartmentById(id.Value);
            if (department is null)
                return NotFound();

            return View(department);
        }

        [HttpGet] // GET: /Department/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] // POST
        public IActionResult Create(CreatedDepartmentDto department)
        {
            if (!ModelState.IsValid)            // Server-Side Validation
                return View(department);

            string message = string.Empty;
            try
            {
                var result = _departmentService.CreateDepartment(department);
                if (result > 0)
                    return RedirectToAction(nameof(Index));
                else
                {
                    message = "Department is not Created";
                    ModelState.AddModelError(string.Empty, message);
                    return View(department);
                }

            }
            catch (Exception ex)
            {
                // 1. Log Exception
                _logger.LogError(ex, ex.Message);

                // 2. Set Message
                message = _environment.IsDevelopment() ? ex.Message : "an error has occured during creating the department :(";

            }

            ModelState.AddModelError(string.Empty, message);
            return View(department);
        }

        [HttpGet]   // Department/Edit/id?
        public IActionResult Edit(int? id)
        {
            if(id is null)
                return BadRequest();    // 400

            var department = _departmentService.GetDepartmentById(id.Value);

            if(department is null)
                return NotFound();      // 404

            return View(new DepartmentEditViewModel()
            {
                Name = department.Name,
                Code = department.Code,
                Description = department.Description,
                CreationDate = department.CreationDate,
            });
        }

        [HttpPost]   // POST
        public IActionResult Edit([FromRoute] int id, DepartmentEditViewModel department)
        {
            if(!ModelState.IsValid)           // Server-Side Validation
                return View(department);

            var message = string.Empty;

            try
            {
                var departmentToUpdate = new UpdatedDepartmentDto()
                {
                    Id = id,
                    Name = department.Name,
                    Code = department.Code,
                    Description = department.Description,
                    CreationDate = department.CreationDate,
                };

                var updated = _departmentService.UpdateDepartment(departmentToUpdate) > 0;

                if (updated)
                    return RedirectToAction(nameof(Index));

                message = "an error has occured during updating the department :(";
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                _logger.LogError(ex, ex.Message);

                // 2. Set Message
                message = _environment.IsDevelopment() ? ex.Message : "an error has occured during updating the department :(";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(department);
        }
    }
}
