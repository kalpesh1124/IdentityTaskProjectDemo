using IdentityTaskProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityTaskProject.Controllers
{
    [Authorize]
    public class TaskApiController : Controller
    {
        private readonly IdentityProjectContext _context;
        public TaskApiController(IdentityProjectContext context)
        {
            _context = context;
        }

        public IActionResult ApiTask()
        {
            return View();
        }

        [HttpGet]
        [Route("api/List")]
        public List<Models.Task> TaskList()
        {
            var data = _context.Tasks.ToList();
            return (data);
        }

        //[HttpPost]
        //[Route("api/AddTask")]
        //public JsonResult AddTask(Models.Task taskApi)
        //{
        //    _context.Tasks.Add(taskApi);
        //    _context.SaveChanges();
        //    return Json(taskApi);
        //    //Models.Task(e);
        //    //Models.Task.SaveChanges();
        //    //return "Employee Added Successfully!!!!";
        //}

        [HttpPost]
        [Route("api/AddTask")]
        public string Add(Models.Task taskApi)
        {
            _context.Tasks.Add(taskApi);
            _context.SaveChanges();
            //return Json(taskApi);
            //Models.Task(e);
            //Models.Task.SaveChanges();
            return "Employee Added Successfully!!!!";

            //try
            //{
            //    // Debugging: Set a breakpoint here to inspect taskApi
            //    if (taskApi == null)
            //    {
            //        // Log or throw an exception to indicate the null value
            //        return Json(new { ResponseCode = 1, ResponseMessage = "Task data is null" });
            //    }

            //    _context.Tasks.Add(taskApi);
            //    _context.SaveChanges();
            //    return Json(new { ResponseCode = 0, ResponseMessage = "Task added successfully!" });
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception for further investigation
            //    return Json(new { ResponseCode = 1, ResponseMessage = $"Error adding task: {ex.Message}" });
            //}
        }

        [HttpGet]
        [Route("api/GetTask/{id}")]
        public JsonResult GetEmpById(int id)
        {
            var e = _context.Tasks.Find(id);
            return Json(e);
        }

        [HttpGet]
        [Route("api/DeleteTask/{id}")]
        public string Delete(int id)
        {
            var e = _context.Tasks.Find(id);
            _context.Tasks.Remove(e);
            _context.SaveChanges();
            return "Employee Deleted Successfully!!!!";
        }

        [HttpPost]
        [Route("api/UpdateTask")]
        public string Update(Models.Task taskApi)
        {
            _context.Tasks.Entry(taskApi).State = EntityState.Modified;
            _context.SaveChanges();
            return "Employee Updated Successfully!!!!";
        }

    }
}
