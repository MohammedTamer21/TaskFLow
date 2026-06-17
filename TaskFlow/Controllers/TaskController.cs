using Microsoft.AspNetCore.Mvc;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    public class TaskController : Controller
    {
        private AppDbContext context;

        public TaskController(AppDbContext ctx)
        {
            context = ctx;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<TaskItem> tasks = context.Tasks.Where(t => t.UserId == userId).ToList();
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Action = "Add";
            return View("Edit", new TaskItem());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Action = "Edit";
            TaskItem task = context.Tasks.Find(id);
            return View(task);
        }

        [HttpPost]
        public IActionResult Edit(TaskItem task)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            task.UserId = userId.Value;

            if (task.TaskId == 0)
            {
                task.CreatedAt = DateTime.Now;
                context.Tasks.Add(task);
            }
            else
            {
                context.Tasks.Update(task);
            }

            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            TaskItem task = context.Tasks.Find(id);
            return View(task);
        }

        [HttpPost]
        public IActionResult Delete(TaskItem task)
        {
            context.Tasks.Remove(task);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}