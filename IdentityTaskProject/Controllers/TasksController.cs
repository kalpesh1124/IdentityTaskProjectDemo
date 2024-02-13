using ExcelDataReader;
using IdentityTaskProject.Areas.Identity.Data;
using IdentityTaskProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using System.Text;

namespace IdentityTaskProject.Controllers
{

    [Authorize]
    public class TasksController : Controller
    {
        private readonly IdentityProjectContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(IdentityProjectContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, Microsoft.AspNetCore.Identity.UserManager<IdentityTaskProject.Areas.Identity.Data.ApplicationUser> UserManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = UserManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            //string userId = User.Identity.GetUserId()
            if (user != null)
            {
                string userId = user.Id;
                if (_userManager.IsInRoleAsync(user, "Admin").Result)
                {
                    var allItems = _context.Tasks.ToList();
                    return View(allItems);
                }
                else
                {
                    var userItems = _context.Tasks.Where(item => item.Userid == userId).ToList();
                    return View(userItems);
                }
            }
            return RedirectToAction("Index", "Tasks");

            //return _context.Tasks != null ?
            //            View(await _context.Tasks.ToListAsync()) :
            //            Problem("Entity set 'IdentityProjectContext.Tasks'  is null.");
        }


        // GET: Tasks/Details/5
        //[Authorize(Roles ="USer,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        public IActionResult CreateManually()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateManually([Bind("Id,Title,Description,Date,Userid")] Models.Task task)
        {

            if (HttpContext.Session.GetString("UserID") != null)
            {
                task.Userid = HttpContext.Session.GetString("UserID").ToString();
            }
            _context.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Tasks");
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UploadFilerequest request)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (request.file != null && request.file.Length > 0)
            {
                var uploadFile = $"{Directory.GetCurrentDirectory()}\\wwwroot\\ExcelFile";
                if (!Directory.Exists(uploadFile))
                {
                    Directory.CreateDirectory(uploadFile);
                }

                var filePath = Path.Combine(uploadFile, request.file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.file.CopyToAsync(stream);
                }
                var userid = _userManager.GetUserId(User);//HttpContext.Session.GetString("UserID").ToString(); 
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var datalist = new List<Models.Task>();
                        do
                        {
                            while (reader.Read())
                            {
                                Models.Task t = new Models.Task();
                                t.Title = reader.GetValue(0).ToString();
                                t.Description = reader.GetValue(1).ToString();
                                t.Date = reader.GetDateTime(2);
                                t.Userid = userid;
                                datalist.Add(t);
                            }
                        } while (reader.NextResult());

                        _context.AddRange(datalist);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("Index", "Tasks");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create1(UploadFilerequest request)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (request.file != null && request.file.Length > 0)
            {
                var uploadFile = $"{Directory.GetCurrentDirectory()}\\wwwroot\\ExcelFile";
                if (!Directory.Exists(uploadFile))
                {
                    Directory.CreateDirectory(uploadFile);
                }

                var filePath = Path.Combine(uploadFile, request.file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.file.CopyToAsync(stream);
                }
                var userid = _userManager.GetUserId(User); 
                var datalist = new List<Models.Task>();
                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                //using (var package = new ExcelPackage(new FileInfo(filePath)))
                {

                    //var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    //if (workSheet != null)
                    //{
                    //    int rowCount = workSheet.Dimension.Rows;

                    //    for (int i = 1; i <= rowCount; i++)
                    //    {
                    //        var titleCell = workSheet.Cells[i, 1].Value;
                    //        var descriptionCell = workSheet.Cells[i, 2].Value;
                    //        var dateCell = workSheet.Cells[i, 3].Value;

                    //        // Check if any of the cells is null before accessing its value
                    //        if (titleCell != null && descriptionCell != null && dateCell != null)
                    //        {
                    //            datalist.Add(new Models.Task
                    //            {
                    //                Title = titleCell.ToString(),
                    //                Description = descriptionCell.ToString(),
                    //                Date = Convert.ToDateTime(dateCell.ToString())
                    //            });
                    //        }
                    //    }

                    //    _context.AddRange(datalist);
                    //    await _context.SaveChangesAsync();
                    //}

                    //2nd

                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    int i = 1;

                    do
                    {

                        datalist.Add(new Models.Task
                        {
                            Title = workSheet.Cells[i, 1].Value.ToString(),
                            Description = workSheet.Cells[i, 2].Value.ToString(),
                            Date = Convert.ToDateTime(workSheet.Cells[i, 3].Value.ToString())
                        });
                        i++;
                    } while (workSheet.Row != null);

                    _context.AddRange(datalist);
                    await _context.SaveChangesAsync();

                    //var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    //if (workSheet != null)
                    //{
                    //    int i = 1;
                    //    do
                    //    {
                    //        datalist.Add(new Models.Task
                    //        {
                    //            Title = workSheet.Cells[i, 1].Value?.ToString(),
                    //            Description = workSheet.Cells[i, 2].Value?.ToString(),
                    //            Date = workSheet.Cells[i, 3].Value != null
                    //                ? Convert.ToDateTime(workSheet.Cells[i, 3].Value.ToString())
                    //                : DateTime.MinValue // Handle null date
                    //        });
                    //        i++;
                    //    } while (workSheet.Row != null);
                    //}
                    //else
                    //{
                    //    return RedirectToAction("Index", "Tasks");
                    //    // Handle the case when the worksheet is null
                    //    // Log an error or return an appropriate response
                    //}

                    //_context.AddRange(datalist);
                    //await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "Tasks");
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Date,Userid")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        //GET: Tasks/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'IdentityProjectContext.Tasks'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
