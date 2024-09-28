using AutoMapper;
using LinkDev.IKEA.BLL.Models.Departments;
using LinkDev.IKEA.BLL.Services.Departments;
using LinkDev.IKEA.DAL.Entities.Departments;
using LinkDev.IKEA.PL.ViewModels.Common;
using LinkDev.IKEA.PL.ViewModels.Departments;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LinkDev.IKEA.PL.Controllers
{
    /// Inheritance: DepartmentController is  a Controller
    /// Composition: DepartmentController has a IDepartmentService
    public class DepartmentController : Controller
    {
        #region Services [Dependency Injection]

        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IWebHostEnvironment _environment;
        public DepartmentController(IDepartmentService departmentService, IMapper mapper, ILogger<DepartmentController> logger, IWebHostEnvironment environment)
        {
            _departmentService = departmentService;
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
        }

        #endregion

        #region Index

        [HttpGet] // GET: /Department/Index
        public IActionResult Index()
        {
            // View's Dictionary: Pass Data from Controller[Action] to View (from View -> [PartialView, Layout])

            /// 1. ViewData is a Dictionary Type Property (Introduced in ASP.NET Framework 3.5)
            ///    => It helps us to transfer the data from the controller[Action] to View

             ViewData["Obj"] = "Hello ViewData";

            /// 2. ViewBag is a Dynamic Type Property (Introduced in ASP.NET Framework 4.0 based on dynamic Feature)
            ///    => It helps us to transfer the data from the controller[Action] to View

            ViewBag.Obj = "Hello ViewBag";
            // ViewBag.Obj = new { Id = 10, Name = "Ahmed"};

            var departments = _departmentService.GetAllDepartments();
            return View(departments);
        }

        #endregion

        #region Details 

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

        #endregion

        #region Create

        [HttpGet] // GET: /Department/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] // POST
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentViewModel departmentVM)
        {
            if (!ModelState.IsValid)            // Server-Side Validation
                return View(departmentVM);

            string message = string.Empty;
            try
            {
                // Manual Mapping
                /// var createdDepartment = new CreatedDepartmentDto()
                /// {
                ///     Name = departmentVM.Name,
                ///     Code = departmentVM.Code,
                ///     Description = departmentVM.Description,
                ///     CreationDate = departmentVM.CreationDate,
                /// };

                var createdDepartment = _mapper.Map<CreatedDepartmentDto>(departmentVM);
                var created = _departmentService.CreateDepartment(createdDepartment) > 0;

                // 3. TempData: is a property of Type Dictionary Object (introduced in ASP.NET Framework 3.5)
                //            : used for transfering the data between 2 consuctive Requests

                if (created)
                    message = "Department has been Created Successfully";
                else
                    message = "Department is not Created";

                TempData["Message"] = message;
                return RedirectToAction(nameof(Index));
                

            }
            catch (Exception ex)
            {
                // 1. Log Exception
                _logger.LogError(ex, ex.Message);

                // 2. Set Message
                message = _environment.IsDevelopment() ? ex.Message : "an error has occured during creating the department :(";

                TempData["Message"] = message;

            }

            ModelState.AddModelError(string.Empty, message);
            return View(departmentVM);

        }

        #endregion

        #region Update

        [HttpGet]   // Department/Edit/id?
        public IActionResult Edit(int? id)
        {
            if (id is null)
                return BadRequest();    // 400

            var department = _departmentService.GetDepartmentById(id.Value);

            if (department is null)
                return NotFound();      // 404

            var departmentVM = _mapper.Map<DepartmentDetailsDto, DepartmentViewModel>(department);

            return View(departmentVM);
        }

        [HttpPost]   // POST
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, DepartmentViewModel department)
        {
            if (!ModelState.IsValid)           // Server-Side Validation
                return View(department);

            var message = string.Empty;

            try
            {
                // Manual Mapping
                var departmentToUpdate = new UpdatedDepartmentDto()
                {
                    Id = id,
                    Name = department.Name,
                    Code = department.Code,
                    Description = department.Description,
                    CreationDate = department.CreationDate,
                };

                //var departmentToUpdate = _mapper.Map<UpdatedDepartmentDto>(department);

                var updated = _departmentService.UpdateDepartment(departmentToUpdate) > 0;

                if (updated)
                        message = "Department has been Updated Successfully";
                    else
                        message = "Department is not Updated";

                TempData["Message"] = message;
                return RedirectToAction(nameof(Index));
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

        #endregion

        #region Delete

        [HttpGet]   // Department/Delete/id?
        public IActionResult Delete(int? id)
        {
            if (id is null)
                return BadRequest();

            var department = _departmentService.GetDepartmentById(id.Value);

            if (department is null)
                return NotFound();

            return View(department);
        }

        [HttpPost]  // POST
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var message = string.Empty;

            try
            {
                var deleted = _departmentService.DeleteDepartment(id);

                if (deleted)
                    return RedirectToAction(nameof(Index));

                message = "an error has occured during deleting the department :(";
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                _logger.LogError(ex, ex.Message);

                // 2. Set Message
                message = _environment.IsDevelopment() ? ex.Message : "an error has occured during deleting the department :(";
            }

            //ModelState.AddModelError(string.Empty, message);
            return RedirectToAction(nameof(Index));
        } 

        #endregion
    }
}
