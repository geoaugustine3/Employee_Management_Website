
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Migrations;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;


namespace EmployeeManagement.Controllers
{
    // [Route("[controller]")]
    [Route("[controller]/[action]")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;

        public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment
            ,ILogger<HomeController> logger)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        

        [Route("~/")]
        [Route("")]
        [Route("~/home")]
        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployees();
            return View(model);

        }


        [Route("{id?}")]
        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            //  throw new Exception("Error in details view");

            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");


            Employee employee = _employeeRepository.GetEmployee(id.Value);
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id ?? 1),
                PageTitle = "Details",
            };
          
            return View(homeDetailsViewModel);

        }

        [HttpGet]
        public  ViewResult Create()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)   // Gets the 'id' from the URL originated from the 'id' value set 'Edit' button.
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel()
            {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Department = employee.Department,
            ExistingPhotoPath = employee.PhotoPath

            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {

            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model); 

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                    
                };
                
                

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();

        }


        // Through model binding, the action method parameter
        // EmployeeEditViewModel receives the posted edit form data

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        { 
            // Check if the provided data is valid, if not rerender the edit view
            // so the user can correct and resubmit the edit form

            if (ModelState.IsValid)
            {
                // Retrieve the employee being edited from the database
                Employee employee = _employeeRepository.GetEmployee(model.Id);

                // Update the employee object with the data in the model object
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;


                // If the user wants to change the photo, a new photo will be
                // uploaded and the Photo property on the model object receives
                // the uploaded photo. If the Photo property is null, user did
                // not upload a new photo and keeps his existing photo

                if (model.Photo != null)
                {  // If a new photo is uploaded, the existing photo must be
                   // deleted. So check if there is an existing photo and delete

                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }

                    // Save the new photo in wwwroot/images folder and update
                    // PhotoPath property of the employee object which will be
                    // eventually saved in the database
                    employee.PhotoPath = ProcessUploadedFile(model);
                    
                }

                // Call update method on the repository service passing it the
                // employee object to update the data in the database table
                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }

            return View();

        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;

            // If the Photos property on the incoming model object is not null and if count > 0,
            // then the user has selected at least one file to upload

            if (model.Photo != null)
            {
                // Loop through each selected file
                
                    // The file must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the injected
                    // IHostingEnvironment service provided by ASP.NET Core
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");

                    // To make sure the file name is unique we are appending a new
                    // GUID value and and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Use CopyTo() method provided by IFormFile interface to
                // copy the file to wwwroot/images folder
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }



        public IActionResult Delete(int id)
        {
                     
            _employeeRepository.Delete(id);
            return RedirectToAction("index");
            
        }

    }
}
 